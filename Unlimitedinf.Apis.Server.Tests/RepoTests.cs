using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Unlimitedinf.Apis.Contracts;
using Xunit;

namespace Unlimitedinf.Apis.Server.IntTests
{
    public class RepoTests
    {
        private static readonly HttpClient client = new HttpClient();

        private async Task<Repo> CreateRepoForTest()
        {
            var tok = await H.T.Create();

            var rep = new Repo
            {
                username = tok.username,
                name = H.CreateUniqueName(),
                repo = new Uri("https://github.com/tompostler/unlimitedinf-apis.git"),
                gitusername = "Tom Postler",
                gituseremail = "tom@postler.me"
            };

            var req = new HttpRequestMessage(HttpMethod.Post, C.U.Repo)
            {
                Content = H.JsonContent(rep)
            };
            req.AddAuthorization(tok.token);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.Created, res.StatusCode);

            var con = await res.Content.ReadAsStringAsync();
            var act = JsonConvert.DeserializeObject<Repo>(con);
            TAssert.Equal(rep, act);

            return act;
        }

        [Fact]
        public async Task Create()
        {
            await this.CreateRepoForTest();
        }
    }
}
