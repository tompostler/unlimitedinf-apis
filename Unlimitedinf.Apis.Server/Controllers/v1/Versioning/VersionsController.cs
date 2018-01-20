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
    [Route("versioning/versions")]
    public class VersionsController : Controller
    {
        private TableStorage TableStorage;
        public VersionsController(TableStorage ts)
        {
            this.TableStorage = ts;
        }

        [HttpGet("{username}/{versionName}")]
        public async Task<IActionResult> GetVersion(string username, string versionName)
        {
            // All versions are publicly gettable
            var retrieve = TableOperation.Retrieve<VersionEntity>(username.ToLowerInvariant() + VersionEntity.PartitionKeySuffix, versionName);
            var result = await TableStorage.Versioning.ExecuteAsync(retrieve);

            if (result.Result == null)
                return this.NotFound();
            return this.Ok((Version)(VersionEntity)result.Result);
        }

        [HttpGet("{username}")]
        public async Task<IActionResult> GetVersions(string username)
        {
            // All versions are publicly gettable
            var versionEntitiesQuery = new TableQuery<VersionEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, username.ToLowerInvariant() + VersionEntity.PartitionKeySuffix));
            var versions = new List<Version>();
            foreach (VersionEntity versionEntity in await TableStorage.Versioning.ExecuteQueryAsync(versionEntitiesQuery))
                versions.Add(versionEntity);

            // TODO: Should this only return 404 if the username was not found? or like it is where no versions is a 404?
            if (versions.Count == 0)
                return this.NotFound();
            else
                return this.Ok(versions);
        }

        [HttpPost, TokenWall]
        public async Task<IActionResult> InsertVersion([FromBody] Version version)
        {
            // Check username
            if (this.IsBadUsername(version.username))
                return this.Unauthorized();

            // Only keep the major.minor.patch
            version.version = new Tools.SemVer(version.version.Major, version.version.Minor, version.version.Patch);

            // Add the version
            var insert = TableOperation.Insert(new VersionEntity(version), true);
            var result = await TableStorage.Versioning.ExecuteAsync(insert);

            return this.TableResultStatus(result.HttpStatusCode, (Version)(VersionEntity)result.Result);
        }

        [HttpPatch("{username}/{versionName}"), TokenWall]
        public async Task<IActionResult> UpdateVersion(string username, string versionName, [FromBody] VersionIncrement versionIncrement)
        {
            // Check username
            if (this.IsBadUsername(username))
                return this.Unauthorized();

            // Get the existing version
            var result = await TableStorage.Versioning.ExecuteAsync(VersionExtensions.GetExistingOperation(username, versionName));
            var versionEntity = (VersionEntity)result.Result;
            if (versionEntity == null)
                return this.NotFound();

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

            return this.TableResultStatus(result.HttpStatusCode, (Version)(VersionEntity)result.Result);
        }

        [HttpDelete("{username}/{versionName}"), TokenWall]
        public async Task<IActionResult> RemoveVersion(string username, string versionName)
        {
            // Check username
            if (this.IsBadUsername(username))
                return this.Unauthorized();

            // Get
            var result = await TableStorage.Versioning.ExecuteAsync(VersionExtensions.GetExistingOperation(username, versionName));
            var versionEntity = (VersionEntity)result.Result;
            if (versionEntity == null)
                return this.NotFound();

            // Remove
            var delete = TableOperation.Delete(versionEntity);
            result = await TableStorage.Versioning.ExecuteAsync(delete);

            return this.TableResultStatus(result.HttpStatusCode, (Version)(VersionEntity)result.Result);
        }
    }
}