using Microsoft.Web.Http;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Unlimitedinf.Apis.Auth;
using Unlimitedinf.Apis.Contracts;
using Unlimitedinf.Apis.Models;

namespace Unlimitedinf.Apis.Controllers.v1
{
    [RequireHttps, ApiVersion("1.0")]
    [RoutePrefix("catans")]
    public class CatansController : BaseController
    {
        private async Task<Catan> GetCatanInternal(string username, string catanName)
        {
            // All catan games are publicly gettable
            var retrieve = TableOperation.Retrieve<CatanEntity>(username.ToLowerInvariant(), catanName);
            var result = await TableStorage.Catans.ExecuteAsync(retrieve);
            return (Catan)(CatanEntity)result.Result;
        }

        [Route, HttpGet]
        public async Task<IHttpActionResult> GetCatan(string username, string catanName)
        {
            // All catan games are publicly gettable
            var result = await this.GetCatanInternal(username, catanName);

            if (result == null)
                return NotFound();
            return Ok(result);
        }

        [Route, HttpGet]
        public async Task<IHttpActionResult> GetCatans(string username)
        {
            // All catan games are publicly gettable
            var catanEntitiesQuery = new TableQuery<CatanEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, username.ToLowerInvariant()));
            var catans = new List<Catan>();
            foreach (CatanEntity catanEntity in await TableStorage.Catans.ExecuteQueryAsync(catanEntitiesQuery))
                catans.Add(catanEntity);

            return Ok(catans);
        }

        [Route, HttpPost, TokenWall]
        public async Task<IHttpActionResult> InsertCatan(Catan catan)
        {
            // Check username
            if (catan.username != this.User.Identity.Name)
                return this.Unauthorized();

            // Add the catan
            var insert = TableOperation.Insert(new CatanEntity(catan), true);
            var result = await TableStorage.Catans.ExecuteAsync(insert);

            return Content((HttpStatusCode)result.HttpStatusCode, (Catan)(CatanEntity)result.Result);
        }

        [Route, HttpDelete, TokenWall]
        public async Task<IHttpActionResult> RemoveCatan(string catanName)
        {
            // Get
            var retrieve = TableOperation.Retrieve<CatanEntity>(this.User.Identity.Name, catanName.ToLowerInvariant());
            var result = await TableStorage.Catans.ExecuteAsync(retrieve);
            var catanEntity = (CatanEntity)result.Result;
            if (catanEntity == null)
                return StatusCode((HttpStatusCode)result.HttpStatusCode);

            // Remove
            var delete = TableOperation.Delete(catanEntity);
            result = await TableStorage.Catans.ExecuteAsync(delete);

            // Annoying
            var returnCode = (HttpStatusCode)result.HttpStatusCode;
            if (returnCode == HttpStatusCode.NoContent)
                returnCode = HttpStatusCode.OK;

            return Content(returnCode, (Catan)(CatanEntity)result.Result);
        }

        [Route("stats"), HttpGet]
        public async Task<IHttpActionResult> GetCatanStats(string username, string catanName)
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