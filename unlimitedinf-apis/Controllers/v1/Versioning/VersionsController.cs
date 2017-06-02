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
    [RoutePrefix("versioning/versions")]
    public class VersionsController : ApiController
    {
        [Route, HttpGet]
        public async Task<IHttpActionResult> GetVersion(string username, string versionName)
        {
            // All versions are publicly gettable
            var retrieve = TableOperation.Retrieve<VersionEntity>(username.ToLowerInvariant(), versionName.ToLowerInvariant());
            var result = await TableStorage.Versioning.ExecuteAsync(retrieve);
            return Content((HttpStatusCode)result.HttpStatusCode, (Version)(VersionEntity)result.Result);
        }

        [Route, HttpGet]
        public async Task<IHttpActionResult> GetVersions(string username)
        {
            // All versions are publicly gettable
            var versionEntitiesQuery = new TableQuery<VersionEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, username.ToLowerInvariant()));
            var versions = new List<Version>();
            foreach (VersionEntity versionEntity in await TableStorage.Versioning.ExecuteQueryAsync(versionEntitiesQuery))
                versions.Add(versionEntity);

            return Ok(versions);
        }

        [Route, HttpPost, TokenWall]
        public async Task<IHttpActionResult> InsertVersion(Version version)
        {
            // Check username
            if (version.username != this.User.Identity.Name)
                return this.Unauthorized();

            // Add the version
            var insert = TableOperation.Insert(new VersionEntity(version), true);
            var result = await TableStorage.Versioning.ExecuteAsync(insert);

            return Content((HttpStatusCode)result.HttpStatusCode, (Version)(VersionEntity)result.Result);
        }

        [Route, HttpPut, TokenWall]
        public async Task<IHttpActionResult> UpdateVersion(VersionIncrement versionIncrement)
        {
            // Check username
            if (versionIncrement.username != this.User.Identity.Name)
                return this.Unauthorized();

            // Get the existing version
            var result = await TableStorage.Versioning.ExecuteAsync(versionIncrement.GetExistingOperation());
            var versionEntity = (VersionEntity)result.Result;
            if (versionEntity == null)
                return StatusCode((HttpStatusCode)result.HttpStatusCode);

            // Update
            switch (versionIncrement.inc)
            {
                case VersionIncrementOption.major:
                    if (versionIncrement.reset)
                        versionEntity.Version = versionEntity.Version.IncrementMajor();
                    else
                        versionEntity.Version = new Tools.SemVer(versionEntity.Version.Major + 1, versionEntity.Version.Minor, versionEntity.Version.Patch);

                    break;
                case VersionIncrementOption.minor:
                    if (versionIncrement.reset)
                        versionEntity.Version = versionEntity.Version.IncrementMinor();
                    else
                        versionEntity.Version = new Tools.SemVer(versionEntity.Version.Major, versionEntity.Version.Minor + 1, versionEntity.Version.Patch);

                    break;
                case VersionIncrementOption.patch:
                    versionEntity.Version = versionEntity.Version.IncrementPatch();
                    break;
            }

            // Replace
            var replace = TableOperation.Replace(versionEntity);
            result = await TableStorage.Versioning.ExecuteAsync(replace);

            // Annoying
            var returnCode = (HttpStatusCode)result.HttpStatusCode;
            if (returnCode == HttpStatusCode.NoContent)
                returnCode = HttpStatusCode.OK;

            return Content(returnCode, (Version)(VersionEntity)result.Result);
        }

        [Route, HttpDelete, TokenWall]
        public async Task<IHttpActionResult> RemoveVersion(string username, string versionName)
        {
            // Check username
            if (username != this.User.Identity.Name)
                return this.Unauthorized();

            // Get
            var retrieve = TableOperation.Retrieve<VersionEntity>(username.ToLowerInvariant(), versionName.ToLowerInvariant());
            var result = await TableStorage.Versioning.ExecuteAsync(retrieve);
            var versionEntity = (VersionEntity)result.Result;
            if (versionEntity == null)
                return StatusCode((HttpStatusCode)result.HttpStatusCode);

            // Remove
            var delete = TableOperation.Delete(versionEntity);
            result = await TableStorage.Versioning.ExecuteAsync(delete);

            // Annoying
            var returnCode = (HttpStatusCode)result.HttpStatusCode;
            if (returnCode == HttpStatusCode.NoContent)
                returnCode = HttpStatusCode.OK;

            return Content(returnCode, (Version)(VersionEntity)result.Result);
        }
    }
}