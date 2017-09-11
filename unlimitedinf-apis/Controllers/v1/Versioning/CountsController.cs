using Microsoft.Web.Http;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Unlimitedinf.Apis.Auth;
using Unlimitedinf.Apis.Contracts.Versioning;
using Unlimitedinf.Apis.Models.Versioning;

namespace Unlimitedinf.Apis.Controllers.v1.Versioning
{
    [RequireHttps, ApiVersion("1.0")]
    [RoutePrefix("versioning/counts")]
    public class CountsController : BaseController
    {
        [Route, HttpGet]
        public async Task<IHttpActionResult> GetCount(string username, string countName)
        {
            // All counts are publicly gettable
            var retrieve = TableOperation.Retrieve<CountEntity>(username.ToLowerInvariant(), countName.ToLowerInvariant());
            var result = await TableStorage.Versioning.ExecuteAsync(retrieve);

            if (result.Result == null)
                return NotFound();
            return Ok((Count)(CountEntity)result.Result);
        }

        [Route, HttpGet]
        public async Task<IHttpActionResult> GetCounts(string username)
        {
            // All counts are publicly gettable
            var countEntitiesQuery = new TableQuery<CountEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, username.ToLowerInvariant()));
            var counts = new List<Count>();
            foreach (CountEntity countEntity in await TableStorage.Versioning.ExecuteQueryAsync(countEntitiesQuery))
                counts.Add(countEntity);

            return Ok(counts);
        }

        [Route, HttpPost, TokenWall]
        public async Task<IHttpActionResult> InsertCount(Count count)
        {
            // Check username
            if (count.username != this.User.Identity.Name)
                return this.Unauthorized();

            // Add the version
            var insert = TableOperation.Insert(new CountEntity(count), true);
            var result = await TableStorage.Versioning.ExecuteAsync(insert);

            return Content((HttpStatusCode)result.HttpStatusCode, (Count)(CountEntity)result.Result);
        }

        [Route, HttpPut, TokenWall]
        public async Task<IHttpActionResult> UpdateCount(CountChange countChange)
        {
            // Check username
            if (countChange.username != this.User.Identity.Name)
                return this.Unauthorized();

            // Get the existing version
            var result = await TableStorage.Versioning.ExecuteAsync(countChange.GetExistingOperation());
            var countEntity = (CountEntity)result.Result;
            if (countEntity == null)
                return StatusCode((HttpStatusCode)result.HttpStatusCode);

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

            // Annoying
            var returnCode = (HttpStatusCode)result.HttpStatusCode;
            if (returnCode == HttpStatusCode.NoContent)
                returnCode = HttpStatusCode.OK;

            return Content(returnCode, (Count)(CountEntity)result.Result);
        }

        [Route, HttpDelete, TokenWall]
        public async Task<IHttpActionResult> RemoveCount(string username, string countName)
        {
            // Check username
            if (username != this.User.Identity.Name)
                return this.Unauthorized();

            // Get
            var retrieve = TableOperation.Retrieve<CountEntity>(username.ToLowerInvariant(), countName.ToLowerInvariant());
            var result = await TableStorage.Versioning.ExecuteAsync(retrieve);
            var countEntity = (CountEntity)result.Result;
            if (countEntity == null)
                return StatusCode((HttpStatusCode)result.HttpStatusCode);

            // Remove
            var delete = TableOperation.Delete(countEntity);
            result = await TableStorage.Versioning.ExecuteAsync(delete);

            // Annoying
            var returnCode = (HttpStatusCode)result.HttpStatusCode;
            if (returnCode == HttpStatusCode.NoContent)
                returnCode = HttpStatusCode.OK;

            return Content(returnCode, (Count)(CountEntity)result.Result);
        }
    }
}