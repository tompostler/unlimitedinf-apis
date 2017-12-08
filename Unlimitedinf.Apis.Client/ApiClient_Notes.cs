using System.Collections.Generic;
using System.Threading.Tasks;
using CN = Unlimitedinf.Apis.Contracts.Notes;

namespace Unlimitedinf.Apis.Client
{
    public sealed class ApiClient_Notes
    {
        private string Token { get; set; }
        private HttpCommunicator Communicator { get; set; }

        private ApiClient_Notes() { }

        internal ApiClient_Notes(string token, HttpCommunicator communicator)
        {
            this.Token = token;
            this.Communicator = communicator;
        }



        public async Task<CN.Catan> CatanCreate(CN.Catan catan)
        {
            return await this.Communicator.Post<CN.Catan, CN.Catan>(Curl.NRepo, catan);
        }

        public static async Task<CN.Catan> CatanRead(string username, string catanName)
        {
            return await StaticHttpCommunicator.Get<CN.Catan>(Curl.NCatan + $"?username={username}&catanName={catanName}");
        }
        public static async Task<string> CatanReadStats(string username, string catanName)
        {
            return await StaticHttpCommunicator.Get<string>(Curl.NCatanStats + $"?username={username}&catanName={catanName}");
        }
        public string CatanReadStats(CN.Catan catan)
        {
            return new CN.CatanStats(catan).ToString();
        }

        public async Task<CN.Catan> CatanDelete(string catanName)
        {
            return await this.Communicator.Delete<CN.Catan>(Curl.NCatan + $"?catanName={catanName}");
        }



        public async Task<CN.Repo> RepoCreate(CN.Repo repo)
        {
            return await this.Communicator.Post<CN.Repo, CN.Repo>(Curl.NRepo, repo);
        }

        public async Task<List<CN.Repo>> RepoRead()
        {
            return await this.Communicator.Get<List<CN.Repo>>(Curl.NRepo);
        }
        public async Task<string> RepoReadPsScript()
        {
            return await this.Communicator.Get<string>(Curl.NRepoPsScript);
        }

        public async Task<CN.Repo> RepoUpdate(CN.Repo repo)
        {
            return await this.Communicator.Put<CN.Repo, CN.Repo>(Curl.NRepo, repo);
        }

        public async Task<CN.Repo> RepoDelete(string repoName)
        {
            return await this.Communicator.Delete<CN.Repo>(Curl.NRepo + $"?repoName={repoName}");
        }
    }
}
