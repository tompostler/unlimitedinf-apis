using System.Threading.Tasks;
using CN = Unlimitedinf.Apis.Contracts;

namespace Unlimitedinf.Apis.Client
{
    public sealed class ApiClient_Catans
    {
        private HttpCommunicator Communicator { get; set; }

        private ApiClient_Catans() { }

        internal ApiClient_Catans(HttpCommunicator communicator)
        {
            this.Communicator = communicator;
        }



        public async Task<CN.Catan> Create(CN.Catan catan)
        {
            return await this.Communicator.Post<CN.Catan, CN.Catan>(Curl.Repo, catan);
        }

        public static async Task<CN.Catan> Read(string username, string catanName)
        {
            return await StaticHttpCommunicator.Get<CN.Catan>(Curl.Catan + $"?username={username}&catanName={catanName}");
        }
        public static async Task<string> ReadStats(string username, string catanName)
        {
            return await StaticHttpCommunicator.Get<string>(Curl.CatanStats + $"?username={username}&catanName={catanName}");
        }
        public static string ReadStats(CN.Catan catan)
        {
            return new CN.CatanStats(catan).ToString();
        }

        public async Task<CN.Catan> Delete(string catanName)
        {
            return await this.Communicator.Delete<CN.Catan>(Curl.Catan + $"?catanName={catanName}");
        }
    }
}
