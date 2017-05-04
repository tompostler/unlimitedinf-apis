using Microsoft.Web.Http;
using Microsoft.WindowsAzure.Storage.Table;
using System;
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
    [RoutePrefix("auth/tokens")]
    public class TokensController : ApiController
    {
        [Route, HttpPost]
        public async Task<IHttpActionResult> CreateToken(TokenCreate token)
        {
            // Validate user
            var result = await TableStorage.Auth.ExecuteAsync(AccountExtensions.GetExistingOperation(token.username));
            if (result.Result == null)
                return this.NotFound();
            if (!BCrypt.Net.BCrypt.Verify(token.secret, ((Account)(AccountEntity)result.Result).secret))
                return this.Unauthorized();

            // Check for existing
            var existing = (await TableStorage.Auth.ExecuteQueryAsync(token.GetExistingOperation())).SingleOrDefault();
            if (existing != null)
                return this.Conflict();

            // Create new
            var entity = new TokenEntity(token);
            var insert = TableOperation.Insert(entity, true);
            result = await TableStorage.Auth.ExecuteAsync(insert);

            return this.Content((HttpStatusCode)result.HttpStatusCode, (Token)(TokenEntity)result.Result);
        }

        [Route, HttpDelete]
        public async Task<IHttpActionResult> DeleteToken(TokenDelete token)
        {
            // Validate user
            var result = await TableStorage.Auth.ExecuteAsync(AccountExtensions.GetExistingOperation(token.username));
            if (result.Result == null)
                return this.NotFound();

            // Get existing
            var existing = await TableStorage.Auth.ExecuteAsync(TokenExtensions.GetExistingOperation(token.username, token.token));
            if (existing.Result == null)
                return this.NotFound();
            var entity = (TokenEntity)existing.Result;
            if (entity.Username != token.username)
                return this.Unauthorized();

            // Delete
            result = await TableStorage.Auth.ExecuteAsync(TableOperation.Delete(entity));
            return this.Content((HttpStatusCode)result.HttpStatusCode, (Token)(TokenEntity)result.Result);
        }

        [Route("all"), HttpDelete]
        public async Task<IHttpActionResult> DeleteExpiredTokens()
        {
            var query = new TableQuery<TokenEntity>().Where(
                TableQuery.CombineFilters(
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition(
                            "PartitionKey",
                            QueryComparisons.GreaterThanOrEqual,
                            TokenExtensions.PartitionKey),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition(
                            "PartitionKey",
                            QueryComparisons.LessThan,
                            TokenExtensions.PartionKeyLessThan)),
                    TableOperators.And,
                    TableQuery.GenerateFilterConditionForDate(
                        nameof(TokenEntity.Expiration),
                        QueryComparisons.LessThanOrEqual,
                        new DateTimeOffset(DateTime.UtcNow))))
                .Select(new string[] { "RowKey" });

            var batch = new TableBatchOperation();
            foreach (var entity in await TableStorage.Auth.ExecuteQueryAsync(query))
                batch.Delete(entity);
            var batchResult = await TableStorage.Auth.ExecuteBatchAsync(batch);
            // Make sure it all worked
            if (batchResult.Any(r => r.HttpStatusCode != (int)HttpStatusCode.NoContent))
                return Content((HttpStatusCode)batchResult.First(r => r.HttpStatusCode != (int)HttpStatusCode.NoContent).HttpStatusCode, "Failed to delete all expired tokens.");

            return this.StatusCode(HttpStatusCode.NoContent);
        }
    }
}