using Microsoft.Web.Http;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Unlimitedinf.Apis.Auth;
using Unlimitedinf.Apis.Contracts.Notes;
using Unlimitedinf.Apis.Models.Notes;

namespace Unlimitedinf.Apis.Controllers.v1.Notes
{
    [RequireHttps, ApiVersion("1.0")]
    [RoutePrefix("notes/repos")]
    public class ReposController : BaseController
    {
        [Route, HttpGet, TokenWall]
        public async Task<IHttpActionResult> GetRepos()
        {
            // Only a user can get their own list of repos
            var repoEntitiesQuery = new TableQuery<RepoEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, this.User.Identity.Name));
            var repos = new List<Repo>();
            foreach (RepoEntity repoEntity in await TableStorage.NotesRepos.ExecuteQueryAsync(repoEntitiesQuery))
                repos.Add(repoEntity);

            return Ok(repos);
        }

        [Route, HttpPost, TokenWall]
        public async Task<IHttpActionResult> InsertRepo(Repo repo)
        {
            // Check username
            if (repo.username != this.User.Identity.Name)
                return this.Unauthorized();

            // Add the repo
            var insert = TableOperation.Insert(new RepoEntity(repo), true);
            var result = await TableStorage.NotesRepos.ExecuteAsync(insert);

            return Content((HttpStatusCode)result.HttpStatusCode, (Repo)(RepoEntity)result.Result);
        }

        [Route, HttpPut, TokenWall]
        public async Task<IHttpActionResult> UpdateRepo(Repo repo)
        {
            // Check username
            if (repo.username != this.User.Identity.Name)
                return this.Unauthorized();

            // Get the existing repo
            var result = await TableStorage.NotesRepos.ExecuteAsync(repo.GetExistingOperation());
            var repoEntity = (RepoEntity)result.Result;
            if (repoEntity == null)
                return StatusCode((HttpStatusCode)result.HttpStatusCode);

            repoEntity.Uri = repo.repo.AbsoluteUri;
            repoEntity.GitUserName = repo.gitusername;
            repoEntity.GitUserEmail = repo.gituseremail;

            // Replace
            var replace = TableOperation.Replace(repoEntity);
            result = await TableStorage.NotesRepos.ExecuteAsync(replace);

            // Annoying
            var returnCode = (HttpStatusCode)result.HttpStatusCode;
            if (returnCode == HttpStatusCode.NoContent)
                returnCode = HttpStatusCode.OK;

            return Content(returnCode, (Repo)(RepoEntity)result.Result);
        }

        [Route, HttpDelete, TokenWall]
        public async Task<IHttpActionResult> RemoveRepo(string repoName)
        {
            // Get
            var retrieve = TableOperation.Retrieve<RepoEntity>(this.User.Identity.Name, repoName.ToLowerInvariant());
            var result = await TableStorage.NotesRepos.ExecuteAsync(retrieve);
            var repoEntity = (RepoEntity)result.Result;
            if (repoEntity == null)
                return StatusCode((HttpStatusCode)result.HttpStatusCode);

            // Remove
            var delete = TableOperation.Delete(repoEntity);
            result = await TableStorage.NotesRepos.ExecuteAsync(delete);

            // Annoying
            var returnCode = (HttpStatusCode)result.HttpStatusCode;
            if (returnCode == HttpStatusCode.NoContent)
                returnCode = HttpStatusCode.OK;

            return Content(returnCode, (Repo)(RepoEntity)result.Result);
        }
    }
}