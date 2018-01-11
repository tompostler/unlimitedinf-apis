using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unlimitedinf.Apis.Contracts.Versioning;
using Unlimitedinf.Apis.Server.Auth;
using Unlimitedinf.Apis.Server.Filters;
using Unlimitedinf.Apis.Server.Models.Versioning;
using Unlimitedinf.Apis.Server.Util;

namespace Unlimitedinf.Apis.Server.Controllers.v1.Versioning
{
    [RequireHttpsNonLocalhostAttribute, ApiVersion("1.0")]
    [Route("versioning/counts")]
    public class CountsController : Controller
    {
        private TableStorage TableStorage;
        public CountsController(TableStorage ts)
        {
            this.TableStorage = ts;
        }

        [HttpGet("{username}/{countName}")]
        public async Task<IActionResult> GetCount(string username, string countName)
        {
            // All counts are publicly gettable
            var retrieve = TableOperation.Retrieve<CountEntity>(username.ToLowerInvariant() + CountEntity.PartitionKeySuffix, countName.ToLowerInvariant());
            var result = await TableStorage.Versioning.ExecuteAsync(retrieve);

            if (result.Result == null)
                return this.NotFound();
            return this.Ok((Count)(CountEntity)result.Result);
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetCounts(string username)
        {
            // All counts are publicly gettable
            var countEntitiesQuery = new TableQuery<CountEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, username.ToLowerInvariant() + CountEntity.PartitionKeySuffix));
            var counts = new List<Count>();
            foreach (CountEntity countEntity in await TableStorage.Versioning.ExecuteQueryAsync(countEntitiesQuery))
                counts.Add(countEntity);

            return this.Ok(counts);
        }

        [HttpPost, TokenWall]
        public async Task<IActionResult> InsertCount([FromBody] Count count)
        {
            // Check username
            if (count.username != this.User.Identity.Name)
                return this.Unauthorized();

            // Add the version
            var insert = TableOperation.Insert(new CountEntity(count), true);
            var result = await TableStorage.Versioning.ExecuteAsync(insert);

            return this.TableResultStatus(result.HttpStatusCode, (Count)(CountEntity)result.Result);
        }

        [HttpPatch("{username}/{countName}"), TokenWall]
        public async Task<IActionResult> UpdateCount(string username, string countName, [FromBody] CountChange countChange)
        {
            // Check username
            if (!username.Equals(this.User.Identity.Name, System.StringComparison.OrdinalIgnoreCase))
                return this.Unauthorized();

            // Get the existing version
            var result = await TableStorage.Versioning.ExecuteAsync(CountExtensions.GetExistingOperation(username, countName));
            var countEntity = (CountEntity)result.Result;
            if (countEntity == null)
                return this.NotFound();

            // Update
            switch (countChange.type)
            {
                case CountChangeOption.inc:
                    countEntity.Count++;
                    break;

                case CountChangeOption.dec:
                    countEntity.Count--;
                    break;

                case CountChangeOption.res:
                    countEntity.Count = 0;
                    break;
            }

            // Replace
            var replace = TableOperation.Replace(countEntity);
            result = await TableStorage.Versioning.ExecuteAsync(replace);

            return this.TableResultStatus(result.HttpStatusCode, (Count)(CountEntity)result.Result);
        }

        [HttpDelete("{username}/{countName}"), TokenWall]
        public async Task<IActionResult> RemoveCount(string username, string countName)
        {
            // Check username
            if (!username.Equals(this.User.Identity.Name, System.StringComparison.OrdinalIgnoreCase))
                return this.Unauthorized();

            // Get
            var result = await TableStorage.Versioning.ExecuteAsync(CountExtensions.GetExistingOperation(username, countName));
            var countEntity = (CountEntity)result.Result;
            if (countEntity == null)
                return this.NotFound();

            // Remove
            var delete = TableOperation.Delete(countEntity);
            result = await TableStorage.Versioning.ExecuteAsync(delete);
            
            return this.TableResultStatus(result.HttpStatusCode, (Count)(CountEntity)result.Result);
        }
    }
}