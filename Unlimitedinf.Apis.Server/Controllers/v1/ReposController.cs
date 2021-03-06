﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unlimitedinf.Apis.Contracts;
using Unlimitedinf.Apis.Server.Auth;
using Unlimitedinf.Apis.Server.Filters;
using Unlimitedinf.Apis.Server.Models;
using Unlimitedinf.Apis.Server.Util;

namespace Unlimitedinf.Apis.Server.Controllers.v1
{
    [RequireHttpsNonLocalhostAttribute, ApiVersion("1.0")]
    [Route("repos"), TokenWall]
    public class ReposController : Controller
    {
        private TableStorage TableStorage;
        public ReposController(TableStorage ts)
        {
            this.TableStorage = ts;
        }

        private async Task<List<Repo>> GetRepoList()
        {
            // Only a user can get their own list of repos
            var repoEntitiesQuery = new TableQuery<RepoEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, this.User.Identity.Name));
            var repos = new List<Repo>();
            foreach (RepoEntity repoEntity in await TableStorage.Repos.ExecuteQueryAsync(repoEntitiesQuery))
                repos.Add(repoEntity);
            return repos;
        }

        [HttpGet]
        public async Task<IActionResult> GetRepos()
        {
            return Ok(await this.GetRepoList());
        }

        [HttpPost]
        public async Task<IActionResult> InsertRepo([FromBody] Repo repo)
        {
            // Check username
            if (this.IsBadUsername(repo.username))
                return this.Unauthorized();

            // Add the repo
            var insert = TableOperation.Insert(new RepoEntity(repo), true);
            var result = await TableStorage.Repos.ExecuteAsync(insert);

            return this.TableResultStatus(result.HttpStatusCode, (Repo)(RepoEntity)result.Result);
        }

        [HttpDelete("{repoName}")]
        public async Task<IActionResult> RemoveRepo(string repoName)
        {
            // Get
            var retrieve = TableOperation.Retrieve<RepoEntity>(this.User.Identity.Name.ToLowerInvariant(), repoName);
            var result = await TableStorage.Repos.ExecuteAsync(retrieve);
            var repoEntity = (RepoEntity)result.Result;
            if (repoEntity == null)
                return this.NotFound();

            // Remove
            var delete = TableOperation.Delete(repoEntity);
            result = await TableStorage.Repos.ExecuteAsync(delete);

            return this.TableResultStatus(result.HttpStatusCode, (Repo)(RepoEntity)result.Result);
        }

        [HttpGet("ps-script")]
        public async Task<IActionResult> GetRepoPsScript()
        {
            var repos = await this.GetRepoList();
            if (repos.Count == 0)
                return Ok("Write-Output 'No repos'");

            return Ok(@"# Go to the place with the stuff
$srcrep = ""$env:USERPROFILE\Source\Repos"";
New-Item -ItemType Directory -Path $srcrep -Force
cd $srcrep

# List of the repos
Add-Type -Language CSharp @""
public class Repo
{
    public string Name;
    public string Path;
    public string RepoUri;
    public string Gitusername;
    public string Gituseremail;
}
""@
function New-Repo {
    param
    (
        [string]$Name,
        [string]$Path,
        [string]$RepoUri,
        [string]$Gitusername,
        [string]$Gituseremail
    )
    $repo = New-Object Repo
    $repo.Name = $Name
    $repo.Path = $Path
    $repo.RepoUri = $RepoUri
    $repo.Gitusername = $Gitusername
    $repo.Gituseremail = $Gituseremail
    return $repo
}
$grepos = @(
    " + string.Join(",\r\n    ", repos.Select(_ => $"(New-Repo '{_.name}' '{_.path}' '{_.repo.AbsoluteUri}' '{_.gitusername}' '{_.gituseremail}')")) + @"
)

# Need git > 2.15?
$env:GIT_REDIRECT_STDERR = '2>&1'

# git clone all the grepos
for ($i=0; $i -lt $grepos.length; $i++) {
    $grepo = $grepos[$i]
    $loc = $grepo.Path
    if ([string]::IsNullOrEmpty($grepo.Path)) { $loc = $grepo.Name }
    $topbot = '+' + ('-'*150).SubString(0, $loc.Length + 14) + '+'
    if (-not (Test-Path $loc)) {
        Write-Host -ForegroundColor White -BackgroundColor Blue $topbot
        Write-Host -ForegroundColor White -BackgroundColor Blue ('| Clone ' + ($i+1).ToString().PadLeft(2) + '/' + $grepos.Length.ToString().PadRight(2) + ' ' + $loc + ' |')
        Write-Host -ForegroundColor White -BackgroundColor Blue $topbot
        git clone --recursive $grepo.RepoUri $loc
        Push-Location $loc
        git config --local user.name $grepo.Gitusername
        git config --local user.email $grepo.Gituseremail
        Pop-Location
    } else {
        Write-Host -ForegroundColor DarkGray -BackgroundColor Blue $topbot
        Write-Host -ForegroundColor DarkGray -BackgroundColor Blue ('| Exist ' + ($i+1).ToString().PadLeft(2) + '/' + $grepos.Length.ToString().PadRight(2) + ' '  + $loc + ' |')
        Write-Host -ForegroundColor DarkGray -BackgroundColor Blue $topbot
    }
    Write-Host
}
");
        }
    }
}