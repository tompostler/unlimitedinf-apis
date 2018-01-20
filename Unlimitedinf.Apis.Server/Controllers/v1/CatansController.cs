using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unlimitedinf.Apis.Contracts;
using Unlimitedinf.Apis.Server.Auth;
using Unlimitedinf.Apis.Server.Filters;
using Unlimitedinf.Apis.Server.Models;
using Unlimitedinf.Apis.Server.Util;

namespace Unlimitedinf.Apis.Server.Controllers.v1
{
    [RequireHttpsNonLocalhostAttribute, ApiVersion("1.0")]
    [Route("catans")]
    public class CatansController : Controller
    {
        private TableStorage TableStorage;
        public CatansController(TableStorage ts)
        {
            this.TableStorage = ts;
        }

        private async Task<Catan> GetCatanInternal(string username, string catanName)
        {
            // All catan games are publicly gettable
            var retrieve = TableOperation.Retrieve<CatanEntity>(username.ToLowerInvariant(), catanName);
            var result = await TableStorage.Catans.ExecuteAsync(retrieve);
            return (Catan)(CatanEntity)result.Result;
        }

        [HttpGet("{username}/{catanName}")]
        public async Task<IActionResult> GetCatan(string username, string catanName)
        {
            // All catan games are publicly gettable
            var result = await this.GetCatanInternal(username, catanName);

            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetCatans(string username)
        {
            // All catan games are publicly gettable
            var catanEntitiesQuery = new TableQuery<CatanEntity>().Where(TableQuery.GenerateFilterCondition(C.TS.PK, QueryComparisons.Equal, username.ToLowerInvariant()));
            var catans = new List<Catan>();
            foreach (CatanEntity catanEntity in await TableStorage.Catans.ExecuteQueryAsync(catanEntitiesQuery))
                catans.Add(catanEntity);

            return Ok(catans);
        }

        [HttpPost, TokenWall]
        public async Task<IActionResult> InsertCatan([FromBody] Catan catan)
        {
            // Check username
            if (this.IsBadUsername(catan.username))
                return this.Unauthorized();

            // Add the catan
            var insert = TableOperation.Insert(new CatanEntity(catan), true);
            var result = await TableStorage.Catans.ExecuteAsync(insert);

            return this.TableResultStatus(result.HttpStatusCode, (Catan)(CatanEntity)result.Result);
        }

        [HttpDelete("{catanName}"), TokenWall]
        public async Task<IActionResult> RemoveCatan(string catanName)
        {
            var catEnt = new CatanEntity() { PartitionKey = this.User.Identity.Name.ToLowerInvariant(), RowKey = catanName, ETag = "*" };
            var op = TableOperation.Delete(catEnt);
            try
            {
                var result = await TableStorage.Messages.ExecuteAsync(op);

                // Since we're issuing a request to just delete it, we'll get nothing back. So just forward through the 204.
                return this.StatusCode(result.HttpStatusCode);
            }
            catch (StorageException ex)
            {
                return this.StatusCode(ex.RequestInformation.HttpStatusCode);
            }
        }

        [HttpGet("{username}/{catanName}/stats")]
        public async Task<IActionResult> GetCatanStats(string username, string catanName)
        {
            // All catan games are publicly gettable
            var result = await this.GetCatanInternal(username, catanName);

            if (result == null)
                return NotFound();

            var stats = new CatanStats(result);
            return Ok(stats.ToString());
        }
    }
}