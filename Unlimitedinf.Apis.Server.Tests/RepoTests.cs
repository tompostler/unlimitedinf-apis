using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [Fact]
        public async Task CreateBadUsername()
        {
            var tok = await H.T.Create();

            var rep = new Repo
            {
                username = H.CreateUniqueAccountName(),
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
            Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
        }

        [Fact]
        public async Task Get()
        {
            var rep1 = await this.CreateRepoForTest();
            var rep2 = await this.CreateRepoForTest();

            var req = new HttpRequestMessage(HttpMethod.Get, C.U.Repo);
            req.AddAuthorization((await H.T.Create()).token);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

            var con = await res.Content.ReadAsStringAsync();
            var act = JsonConvert.DeserializeObject<List<Repo>>(con);
            TAssert.Equal(rep1, act.Single(_ => rep1.name.Equals(_.name)));
            TAssert.Equal(rep2, act.Single(_ => rep2.name.Equals(_.name)));
        }

        [Fact]
        public async Task Remove()
        {
            var rep = await this.CreateRepoForTest();

            var req = new HttpRequestMessage(HttpMethod.Delete, C.U.Repo + "/" + rep.name);
            req.AddAuthorization((await H.T.Create()).token);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

            var con = await res.Content.ReadAsStringAsync();
            var act = JsonConvert.DeserializeObject<Repo>(con);
            TAssert.Equal(rep, act);
        }

        [Fact]
        public async Task RemoveNotFound()
        {
            var req = new HttpRequestMessage(HttpMethod.Delete, C.U.Repo + "/" + H.CreateUniqueName());
            req.AddAuthorization((await H.T.Create()).token);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }
    }
}
