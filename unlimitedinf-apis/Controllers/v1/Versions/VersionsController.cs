using Microsoft.Web.Http;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Unlimitedinf.Apis.Auth;
using Unlimitedinf.Apis.Models.Versions;

namespace Unlimitedinf.Apis.Controllers.v1.Versions
{
    [RequireHttps, ApiVersion("1.0")]
    [RoutePrefix("version/versions")]
    public class VersionsController : ApiController
    {
        [Route, HttpGet]
        public async Task<IHttpActionResult> GetVersion(string accountName, string versionName)
        {
            // All versions are publicly gettable
            var retrieve = TableOperation.Retrieve<VersionEntity>(accountName.ToLowerInvariant(), versionName.ToLowerInvariant());
            var result = await TableStorage.Version.ExecuteAsync(retrieve);
            return Content((HttpStatusCode)result.HttpStatusCode, (VersionApi)(VersionEntity)result.Result);
        }

        [Route, HttpGet]
        public async Task<IHttpActionResult> GetVersions(string accountName)
        {
            // All versions are publicly gettable
            var versionEntitiesQuery = new TableQuery<VersionEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, accountName.ToLowerInvariant()));
            var versions = new List<VersionApi>();
            foreach (VersionEntity versionEntity in await TableStorage.Version.ExecuteQueryAsync(versionEntitiesQuery))
                versions.Add(versionEntity);

            return Ok(versions);
        }

        [Route, HttpPost]
        public async Task<IHttpActionResult> InsertVersion(VersionApi version)
        {
            // Check the auth
            var authResult = await AuthorizeHeader.Check(ActionContext, version.username);
            if (!string.IsNullOrEmpty(authResult))
                return Content(HttpStatusCode.Forbidden, authResult);

            // Add the version
            var insert = TableOperation.Insert((VersionEntity)version, true);
            var result = await TableStorage.Version.ExecuteAsync(insert);

            return Content((HttpStatusCode)result.HttpStatusCode, (VersionApi)(VersionEntity)result.Result);
        }

        [Route, HttpPut]
        public async Task<IHttpActionResult> UpdateVersion(VersionApiIncrement versionIncrement)
        {
            // Check the auth
            var authResult = await AuthorizeHeader.Check(ActionContext, versionIncrement.username);
            if (!string.IsNullOrEmpty(authResult))
                return Content(HttpStatusCode.Forbidden, authResult);

            // Get the existing version
            var result = await TableStorage.Version.ExecuteAsync(versionIncrement.GetExistingOperation());
            var versionEntity = (VersionEntity)result.Result;
            if (versionEntity == null)
                return StatusCode((HttpStatusCode)result.HttpStatusCode);

            // Update
            switch (versionIncrement.inc)
            {
                case VersionApiIncrementOption.Major:
                    if (versionIncrement.reset)
                        versionEntity.Version = versionEntity.Version.IncrementMajor();
                    else
                        versionEntity.Version = new Tools.SemVer(versionEntity.Version.Major + 1, versionEntity.Version.Minor, versionEntity.Version.Patch);

                    break;
                case VersionApiIncrementOption.Minor:
                    if (versionIncrement.reset)
                        versionEntity.Version = versionEntity.Version.IncrementMinor();
                    else
                        versionEntity.Version = new Tools.SemVer(versionEntity.Version.Major, versionEntity.Version.Minor + 1, versionEntity.Version.Patch);

                    break;
                case VersionApiIncrementOption.Patch:
                    versionEntity.Version = versionEntity.Version.IncrementPatch();
                    break;
            }

            // Replace
            var replace = TableOperation.Replace(versionEntity);
            result = await TableStorage.Version.ExecuteAsync(replace);

            // Annoying
            var returnCode = (HttpStatusCode)result.HttpStatusCode;
            if (returnCode == HttpStatusCode.NoContent)
                returnCode = HttpStatusCode.OK;

            return Content(returnCode, (VersionApi)(VersionEntity)result.Result);
        }

        [Route, HttpDelete]
        public async Task<IHttpActionResult> RemoveVersion(string accountName, string versionName)
        {
            // Check the auth
            var authResult = await AuthorizeHeader.Check(ActionContext, accountName);
            if (!string.IsNullOrEmpty(authResult))
                return Content(HttpStatusCode.Forbidden, authResult);

            // Get
            var retrieve = TableOperation.Retrieve<VersionEntity>(accountName.ToLowerInvariant(), versionName.ToLowerInvariant());
            var result = await TableStorage.Version.ExecuteAsync(retrieve);
            var versionEntity = (VersionEntity)result.Result;
            if (versionEntity == null)
                return StatusCode((HttpStatusCode)result.HttpStatusCode);

            // Remove
            var delete = TableOperation.Delete(versionEntity);
            result = await TableStorage.Version.ExecuteAsync(delete);

            return Content((HttpStatusCode)result.HttpStatusCode, (VersionApi)(VersionEntity)result.Result);
        }
    }
}