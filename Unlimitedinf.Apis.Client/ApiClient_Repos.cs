using System.Collections.Generic;
using System.Threading.Tasks;
using CN = Unlimitedinf.Apis.Contracts;

namespace Unlimitedinf.Apis.Client
{
    public sealed class ApiClient_Repos
    {
        private HttpCommunicator Communicator { get; set; }

        private ApiClient_Repos() { }

        internal ApiClient_Repos(HttpCommunicator communicator)
        {
            this.Communicator = communicator;
        }



        public async Task<CN.Repo> Create(CN.Repo repo)
        {
            return await this.Communicator.Post<CN.Repo, CN.Repo>(Curl.Repo, repo);
        }

        public async Task<List<CN.Repo>> Read()
        {
            return await this.Communicator.Get<List<CN.Repo>>(Curl.Repo);
        }
        public async Task<string> ReadPsScript()
        {
            return await this.Communicator.Get<string>(Curl.RepoPsScript);
        }

        public async Task<CN.Repo> Update(CN.Repo repo)
        {
            return await this.Communicator.Put<CN.Repo, CN.Repo>(Curl.Repo, repo);
        }

        public async Task<CN.Repo> Delete(string repoName)
        {
            return await this.Communicator.Delete<CN.Repo>(Curl.Repo + $"?repoName={repoName}");
        }
    }
}
