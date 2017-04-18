using Microsoft.Web.Http;
using Microsoft.WindowsAzure.Storage.Table;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Unlimitedinf.Apis.Auth;
using Unlimitedinf.Apis.Contracts.Auth;
using Unlimitedinf.Apis.Models.Auth;

namespace Unlimitedinf.Apis.Controllers.v1.Auth
{
    [RequireHttps, ApiVersion("1.0")]
    [RoutePrefix("auth/accounts")]
    public class AccountsController : ApiController
    {
        [Route, HttpPost]
        public async Task<IHttpActionResult> CreateAccount(Account account)
        {
            // Check for existence
            var result = await TableStorage.Auth.ExecuteAsync(account.GetExistingOperation());
            if (result.Result != null)
                return Conflict();

            // Hash secret
            account.secret = BCrypt.Net.BCrypt.HashPassword(account.secret, 12);

            // Insert
            var insert = TableOperation.Insert(new AccountEntity(account), true);
            result = await TableStorage.Auth.ExecuteAsync(insert);
            account = (Account)(AccountEntity)result.Result;
            account.secret = null;

            return Content((HttpStatusCode)result.HttpStatusCode, account);
        }

        [Route, HttpGet]
        public async Task<IHttpActionResult> ReadAccount(string username)
        {
            // Get existing
            var retreive = TableOperation.Retrieve<AccountEntity>(AccountExtensions.PartitionKey, username.ToLowerInvariant());
            var result = await TableStorage.Auth.ExecuteAsync(retreive);
            if (result.Result == null)
                return NotFound();

            // Remove secret
            var account = (Account)(AccountEntity)result.Result;
            account.secret = null;

            return Ok(account);
        }

        [Route, HttpPut]
        public async Task<IHttpActionResult> UpdateAccount(AccountUpdate account)
        {
            // Oldsecret and secret cannot be the same
            if (account.oldsecret == account.secret)
                return this.BadRequest("You must use a different secret.");

            // Get/check for existence
            var result = await TableStorage.Auth.ExecuteAsync(account.GetExistingOperation());
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
            result = await TableStorage.Auth.ExecuteAsync(upsert);
            var upserted = (Account)(AccountEntity)result.Result;
            upserted.secret = null;

            return Content((HttpStatusCode)result.HttpStatusCode, upserted);
        }

        [Route, HttpDelete]
        public async Task<IHttpActionResult> DeleteAccount(Account account)
        {
            // Get/Check for existence
            var result = await TableStorage.Auth.ExecuteAsync(account.GetExistingOperation());
            if (result.Result == null)
                return NotFound();

            // Verify secret
            if (!BCrypt.Net.BCrypt.Verify(account.secret, ((Account)(AccountEntity)result.Result).secret))
                return Unauthorized();

            // Clear tokens first
            var query = new TableQuery<DynamicTableEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, account.username.ToLowerInvariant()))
                .Select(new string[] { "RowKey" });
            var batch = new TableBatchOperation();
            foreach (var token in await TableStorage.Auth.ExecuteQueryAsync(query))
                batch.Delete(token);
            var tokenBatchResult = await TableStorage.Auth.ExecuteBatchAsync(batch);
            // Make sure it all worked
            if (tokenBatchResult.Any(r => r.HttpStatusCode != (int)HttpStatusCode.NoContent))
                return Content((HttpStatusCode)tokenBatchResult.First(r => r.HttpStatusCode != (int)HttpStatusCode.NoContent).HttpStatusCode, "Failed to delete all tokens.");

            // Account
            var delete = TableOperation.Delete((AccountEntity)result.Result);
            result = await TableStorage.Auth.ExecuteAsync(delete);
            var deleted = (Account)(AccountEntity)result.Result;
            deleted.secret = null;

            return Content((HttpStatusCode)result.HttpStatusCode, deleted);

            //TODO: write something that will go through and cascade the deletion to other tables
        }
    }
}