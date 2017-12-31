using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Table;
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

            return this.StatusCode(result.HttpStatusCode, account);
        }

        [Route("{username?}"), HttpGet]
        public async Task<IActionResult> ReadAccount(string username)
        {
            // Get existing
            var retreive = TableOperation.Retrieve<AccountEntity>(AccountExtensions.PartitionKey, username.ToLowerInvariant());
            var result = await this.TableStorage.Auth.ExecuteAsync(retreive);
            if (result.Result == null)
                return this.NotFound();

            // Remove secret
            var account = (Account)(AccountEntity)result.Result;
            account.secret = null;

            return this.Ok(account);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateAccount([FromBody] AccountUpdate account)
        {
            // Oldsecret and secret cannot be the same
            if (account.oldsecret == account.secret)
                return this.BadRequest("You must use a different secret.");

            // Get/check for existence
            var result = await this.TableStorage.Auth.ExecuteAsync(account.GetExistingOperation());
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
        public async Task<IActionResult> DeleteAccount(string username, [FromBody] AccountDelete secret)
        {
            // Get/Check for existence
            var result = await this.TableStorage.Auth.ExecuteAsync(AccountExtensions.GetExistingOperation(username));
            if (result.Result == null)
                return NotFound();

            // Verify secret
            if (!BCrypt.Net.BCrypt.Verify(secret.secret, ((Account)(AccountEntity)result.Result).secret))
                return Unauthorized();

            // Clear tokens first
            var query = new TableQuery<DynamicTableEntity>()
                .Where(TableQuery.GenerateFilterCondition(C.TS.PK, QueryComparisons.Equal, username.ToLowerInvariant()))
                .Select(C.TS.PRKF);
            var batch = new TableBatchOperation();
            foreach (var token in await this.TableStorage.Auth.ExecuteQueryAsync(query))
                batch.Delete(token);
            var tokenBatchResult = await this.TableStorage.Auth.ExecuteBatchAsync(batch);
            // Make sure it all worked
            if (tokenBatchResult.Any(r => r.HttpStatusCode != (int)HttpStatusCode.NoContent))
                return this.StatusCode(tokenBatchResult.First(r => r.HttpStatusCode != (int)HttpStatusCode.NoContent).HttpStatusCode, "Failed to delete all tokens.");

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