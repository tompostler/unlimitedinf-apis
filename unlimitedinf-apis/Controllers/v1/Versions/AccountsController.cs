using Microsoft.Web.Http;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Unlimitedinf.Apis.Auth;
using Unlimitedinf.Apis.Models.Versions;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Controllers.v1.Versions
{
    [RequireHttps, ApiVersion("1.0")]
    [RoutePrefix("version/accounts")]
    public class AccountsController : ApiController
    {
        [Route, HttpPost]
        public async Task<IHttpActionResult> RegisterNewUser(AccountApi account)
        {
            var result = await TableStorage.Version.ExecuteAsync(account.GetExistingOperation());
            if (result.Result != null)
                return Conflict();

            var insertNewAccount = TableOperation.Insert((AccountEntity)account, true);
            result = await TableStorage.Version.ExecuteAsync(insertNewAccount);

            return Content((HttpStatusCode)result.HttpStatusCode, (AccountApi)(AccountEntity)result.Result);
        }

        [Route, HttpPut]
        public async Task<IHttpActionResult> UpdateAccountSecret(AccountApi account)
        {
            if (string.IsNullOrWhiteSpace(account.oldsecret))
                return BadRequest("account.oldsecret required.");

            var result = await TableStorage.Version.ExecuteAsync(account.GetExistingOperation());
            if (result.Result == null)
                return NotFound();

            if (!account.oldsecret.GetHashCodeSha512().Equals(((AccountEntity)result.Result).Secret))
                return StatusCode(HttpStatusCode.Forbidden);

            var upsertAccount = TableOperation.Replace((AccountEntity)account);
            result = await TableStorage.Version.ExecuteAsync(upsertAccount);

            return Content((HttpStatusCode)result.HttpStatusCode, (AccountApi)(AccountEntity)result.Result);
        }

        [Route, HttpDelete]
        public async Task<IHttpActionResult> DeleteAccountAndAllVersions(AccountApi account)
        {
            // Check if account exists
            var result = await TableStorage.Version.ExecuteAsync(account.GetExistingOperation());
            if (result.Result == null)
                return NotFound();

            // Verify they have permission to do this
            var accountEntity = (AccountEntity)result.Result;
            if (!account.secret.GetHashCodeSha512().Equals(accountEntity.Secret))
                return StatusCode(HttpStatusCode.Forbidden);

            // Delete all associated version entries
            var versionEntitiesQuery = new TableQuery<VersionEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, accountEntity.RowKey));
            var versionEntitiesDropBatch = new TableBatchOperation();
            foreach (VersionEntity versionEntity in await TableStorage.Version.ExecuteQueryAsync(versionEntitiesQuery))
                versionEntitiesDropBatch.Delete(versionEntity);
            var dropResults = await TableStorage.Version.ExecuteBatchAsync(versionEntitiesDropBatch);
            var results = new DeleteAccountAndAllVersionsResult();
            foreach (var dropResult in dropResults)
                results.VersionsDropResults.Add(((VersionEntity)result.Result).Name, result.HttpStatusCode);

            // Delete the account
            result = await TableStorage.Version.ExecuteAsync(TableOperation.Delete(accountEntity));
            results.AccountDropResult = (AccountEntity)result.Result;
            return Content((HttpStatusCode)result.HttpStatusCode, results);
        }

        private class DeleteAccountAndAllVersionsResult
        {
            public Dictionary<string, int> VersionsDropResults { get; set; } = new Dictionary<string, int>();
            public AccountApi AccountDropResult { get; set; }
        }
    }
}