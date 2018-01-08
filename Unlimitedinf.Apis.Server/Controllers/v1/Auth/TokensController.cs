using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Unlimitedinf.Apis.Contracts.Auth;
using Unlimitedinf.Apis.Server.Filters;
using Unlimitedinf.Apis.Server.Models.Auth;
using Unlimitedinf.Apis.Server.Util;

namespace Unlimitedinf.Apis.Server.Controllers.v1.Auth
{
    [RequireHttpsNonDebugLocalhost, ApiVersion("1.0")]
    [Route("auth/tokens")]
    public class TokensController : Controller
    {
        private TableStorage TableStorage;
        public TokensController(TableStorage ts)
        {
            this.TableStorage = ts;
        }

        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] TokenCreate token)
        {
            // Validate user
            var result = await this.TableStorage.Auth.ExecuteAsync(AccountExtensions.GetExistingOperation(token.username));
            if (result.Result == null)
                return this.NotFound();
            if (!BCrypt.Net.BCrypt.Verify(token.secret, ((Account)(AccountEntity)result.Result).secret))
                return this.Unauthorized();

            // Check for existing
            var existing = (await this.TableStorage.Auth.ExecuteQueryAsync(token.GetExistingOperation())).SingleOrDefault();
            if (existing != null)
                return this.StatusCode((int)HttpStatusCode.Conflict);

            // Create new
            var entity = new TokenEntity(token);
            var insert = TableOperation.Insert(entity, true);
            result = await this.TableStorage.Auth.ExecuteAsync(insert);

            return this.TableResultStatus(result.HttpStatusCode, (Token)(TokenEntity)result.Result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteToken([FromBody] TokenDelete token)
        {
            // Validate user
            var result = await this.TableStorage.Auth.ExecuteAsync(AccountExtensions.GetExistingOperation(token.username));
            if (result.Result == null)
                return this.NotFound();

            // Get existing
            var existing = await this.TableStorage.Auth.ExecuteAsync(TokenExtensions.GetExistingOperation(token.username, token.token));
            if (existing.Result == null)
                return this.NotFound();
            var entity = (TokenEntity)existing.Result;
            if (entity.Username != token.username)
                return this.Unauthorized();

            // Delete
            result = await this.TableStorage.Auth.ExecuteAsync(TableOperation.Delete(entity));
            return this.TableResultStatus(result.HttpStatusCode, (Token)(TokenEntity)result.Result);
        }

        [Route("/all"), HttpDelete]
        public async Task<IActionResult> DeleteExpiredTokens()
        {
            var query = new TableQuery<TokenEntity>().Where(
                TableQuery.CombineFilters(
                    TableQuery.CombineFilters(
                        TableQuery.GenerateFilterCondition(
                            C.TS.PK,
                            QueryComparisons.GreaterThanOrEqual,
                            TokenExtensions.PartitionKey),
                        TableOperators.And,
                        TableQuery.GenerateFilterCondition(
                            C.TS.PK,
                            QueryComparisons.LessThan,
                            TokenExtensions.PartionKeyLessThan)),
                    TableOperators.And,
                    TableQuery.GenerateFilterConditionForDate(
                        nameof(TokenEntity.Expiration),
                        QueryComparisons.LessThanOrEqual,
                        DateTimeOffset.UtcNow)))
                .Select(C.TS.PRKF);

            var batch = new TableBatchOperation();
            foreach (var entity in await this.TableStorage.Auth.ExecuteQueryAsync(query))
                batch.Delete(entity);
            var batchResult = await this.TableStorage.Auth.ExecuteBatchAsync(batch);
            // Make sure it all worked
            if (batchResult.Any(r => r.HttpStatusCode != (int)HttpStatusCode.NoContent))
                return this.StatusCode(batchResult.First(r => r.HttpStatusCode != (int)HttpStatusCode.NoContent).HttpStatusCode, "Failed to delete all expired tokens.");

            return this.NoContent();
        }
    }
}