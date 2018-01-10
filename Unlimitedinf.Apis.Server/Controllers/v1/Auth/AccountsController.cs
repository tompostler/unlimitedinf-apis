using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Unlimitedinf.Apis.Contracts.Auth;
using Unlimitedinf.Apis.Server.Auth;
using Unlimitedinf.Apis.Server.Filters;
using Unlimitedinf.Apis.Server.Models.Auth;
using Unlimitedinf.Apis.Server.Util;

namespace Unlimitedinf.Apis.Server.Controllers.v1.Auth
{
    [RequireHttpsNonDebugLocalhost, ApiVersion("1.0")]
    [Route("auth/accounts")]
    public class AccountsController : Controller
    {
        private TableStorage TableStorage;
        public AccountsController(TableStorage ts)
        {
            this.TableStorage = ts;
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] Account account)
        {
            // Check for existence
            var result = await this.TableStorage.Auth.ExecuteAsync(account.GetExistingOperation());
            if (result.Result != null)
                return this.StatusCode((int)HttpStatusCode.Conflict);

            // Hash secret
            account.secret = BCrypt.Net.BCrypt.HashPassword(account.secret, 12);

            // Insert
            var insert = TableOperation.Insert(new AccountEntity(account), true);
            result = await this.TableStorage.Auth.ExecuteAsync(insert);
            account = (Account)(AccountEntity)result.Result;
            account.secret = null;

            return this.TableResultStatus(result.HttpStatusCode, account);
        }

        [Route("{username?}"), HttpGet]
        public async Task<IActionResult> ReadAccount(string username)
        {
            // Get existing
            var result = await this.TableStorage.Auth.ExecuteAsync(AccountExtensions.GetExistingOperation(username));
            if (result.Result == null)
                return this.NotFound();

            // Remove secret
            var account = (Account)(AccountEntity)result.Result;
            account.secret = null;

            return this.Ok(account);
        }

        [Route("{username?}"), HttpPut]
        public async Task<IActionResult> UpdateAccount(string username, [FromBody] AccountUpdate account)
        {
            // Oldsecret and secret cannot be the same
            if (account.oldsecret == account.secret)
                return this.BadRequest("You must use a different secret.");

            // Get/check for existence
            var result = await this.TableStorage.Auth.ExecuteAsync(AccountExtensions.GetExistingOperation(username));
            if (result.Result == null)
                return NotFound();

            // Compare current secrets
            if (!BCrypt.Net.BCrypt.Verify(account.oldsecret, ((Account)(AccountEntity)result.Result).secret))
                return Unauthorized();

            // Hash new secret
            var entity = (AccountEntity)result.Result;
            entity.Secret = BCrypt.Net.BCrypt.HashPassword(account.secret, 12);

            // Upsert
            var upsert = TableOperation.InsertOrMerge(entity);
            result = await this.TableStorage.Auth.ExecuteAsync(upsert);
            var upserted = (Account)(AccountEntity)result.Result;
            upserted.secret = null;

            return this.TableResultStatus(result.HttpStatusCode, upserted);
        }

        [Route("{username?}"), HttpDelete, TokenWall]
        public async Task<IActionResult> DeleteAccount(string username, [FromBody] AccountDelete account)
        {
            // Verify token matches username
            if (!this.User.Identity.Name.Equals(username, StringComparison.OrdinalIgnoreCase))
                return this.BadRequest("'username' does not match the token.");

            // Get/Check for existence
            var result = await this.TableStorage.Auth.ExecuteAsync(AccountExtensions.GetExistingOperation(username));
            if (result.Result == null)
                return this.NotFound();

            // Verify secret
            if (!BCrypt.Net.BCrypt.Verify(account.secret, ((Account)(AccountEntity)result.Result).secret))
                return this.Unauthorized();

            // Account
            var delete = TableOperation.Delete((AccountEntity)result.Result);
            result = await this.TableStorage.Auth.ExecuteAsync(delete);
            var deleted = (Account)(AccountEntity)result.Result;
            deleted.secret = null;

            return this.TableResultStatus(result.HttpStatusCode, deleted);

            //TODO: write something that will go through and cascade the deletion to other tables
        }
    }
}