using System.Collections.Generic;
using System.Threading.Tasks;
using CV = Unlimitedinf.Apis.Contracts.Versioning;

namespace Unlimitedinf.Apis.Client
{
    public sealed partial class ApiClient_Versioning
    {
        private string Token { get; set; }
        private HttpCommunicator Communicator { get; set; }

        private ApiClient_Versioning() { }

        internal ApiClient_Versioning(string token, HttpCommunicator communicator)
        {
            this.Token = token;
            this.Communicator = communicator;
        }

        public async Task<CV.Version> Create(CV.Version version)
        {
            return await this.Communicator.Post<CV.Version, CV.Version>(Curl.VVersion, version);
        }

        public async Task<List<CV.Version>> Read(string username)
        {
            return await this.Communicator.Get<List<CV.Version>>(Curl.VVersion + $"?accountName={username}");
        }
        public async Task<CV.Version> Read(string username, string versionName)
        {
            return await this.Communicator.Get<CV.Version>(Curl.VVersion + $"?accountName={username}&versionName={versionName}");
        }

        public async Task<CV.Version> Update(CV.VersionIncrement version)
        {
            return await this.Communicator.Put<CV.VersionIncrement, CV.Version>(Curl.VVersion, version);
        }

        public async Task<CV.Version> Delete(string username, string versionName)
        {
            return await this.Communicator.Delete<CV.Version>(Curl.VVersion + $"?username={username}&versionName={versionName}");
        }
    }
}
