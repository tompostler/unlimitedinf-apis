using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Unlimitedinf.Apis.Contracts.Versioning;
using Unlimitedinf.Tools;
using Xunit;

namespace Unlimitedinf.Apis.Server.IntTests
{
    public class VCountTests
    {
        private static readonly HttpClient client = new HttpClient();

        private async Task<Count> CreateCountForTests()
        {
            var tok = await H.T.Create();
            var cnt = new Count
            {
                username = tok.username,
                name = H.CreateUniqueName(),
                count = 42
            };

            var req = new HttpRequestMessage(HttpMethod.Post, C.U.VeCount)
            {
                Content = H.JsonContent(cnt)
            };
            req.AddAuthorization(tok.token);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.Created, res.StatusCode);

            return cnt;
        }

        [Fact]
        public async Task Create()
        {
            await CreateCountForTests();
        }

        [Fact]
        public async Task CreateBadUsername()
        {
            var tok = await H.T.Create();
            var cnt = new Count
            {
                username = tok.username + "1",
                name = H.CreateUniqueName(),
                count = 42
            };

            var req = new HttpRequestMessage(HttpMethod.Post, C.U.VeCount)
            {
                Content = H.JsonContent(cnt)
            };
            req.AddAuthorization(tok.token);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
        }

        [Fact]
        public async Task Get()
        {
            var cnt = await CreateCountForTests();

            var res = await client.GetAsync(C.U.VeCount + $"/{cnt.username}/{cnt.name}");
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

            var con = await res.Content.ReadAsStringAsync();
            var act = JsonConvert.DeserializeObject<Count>(con);
            TAssert.Equal(cnt, act);
        }

        [Fact]
        public async Task GetNotFound()
        {
            var cnt = await CreateCountForTests();

            // Bad username
            var res = await client.GetAsync(C.U.VeCount + $"/{cnt.username}1/{cnt.name}");
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);

            // Bad versionName
            res = await client.GetAsync(C.U.VeCount + $"/{cnt.username}/{cnt.name}1");
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task Gets()
        {
            var cnt1 = await CreateCountForTests();
            var cnt2 = await CreateCountForTests();

            var res = await client.GetAsync(C.U.VeCount + $"/{cnt1.username}");
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

            var con = await res.Content.ReadAsStringAsync();
            var act = JsonConvert.DeserializeObject<List<Count>>(con);
            TAssert.Equal(cnt1, act.Single(_ => _.name.Equals(cnt1.name)));
            TAssert.Equal(cnt2, act.Single(_ => _.name.Equals(cnt2.name)));
        }

        [Fact]
        public async Task GetNotFounds()
        {
            var cnt1 = await CreateCountForTests();
            var cnt2 = await CreateCountForTests();

            var res = await client.GetAsync(C.U.VeCount + $"/{cnt1.username}1");
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Theory]
        [InlineData(CountChangeOption.inc, 43)]
        [InlineData(CountChangeOption.dec, 41)]
        [InlineData(CountChangeOption.res, 0)]
        public async Task Increment(CountChangeOption cntOpt, long exp)
        {
            var cnt = await CreateCountForTests();
            var inc = new CountChange { type = cntOpt };

            var req = new HttpRequestMessage(new HttpMethod("PATCH"), C.U.VeCount + $"/{cnt.username}/{cnt.name}")
            {
                Content = H.JsonContent(inc)
            };
            req.AddAuthorization((await H.T.Create()).token);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

            cnt.count = exp;
            var con = await res.Content.ReadAsStringAsync();
            var act = JsonConvert.DeserializeObject<Count>(con);
            TAssert.Equal(cnt, act);
        }

        [Fact]
        public async Task Delete()
        {
            var cnt = await CreateCountForTests();

            var req = new HttpRequestMessage(HttpMethod.Delete, C.U.VeCount + $"/{cnt.username}/{cnt.name}");
            req.AddAuthorization((await H.T.Create()).token);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

            var con = await res.Content.ReadAsStringAsync();
            var act = JsonConvert.DeserializeObject<Count>(con);
            TAssert.Equal(cnt, act);
        }

        [Fact]
        public async Task DeleteBadUsername()
        {
            var cnt = await CreateCountForTests();

            var req = new HttpRequestMessage(HttpMethod.Delete, C.U.VeCount + $"/{cnt.username}1/{cnt.name}");
            req.AddAuthorization((await H.T.Create()).token);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
        }

        [Fact]
        public async Task DeleteBadVerName()
        {
            var cnt = await CreateCountForTests();

            var req = new HttpRequestMessage(HttpMethod.Delete, C.U.VeCount + $"/{cnt.username}/{cnt.name}1");
            req.AddAuthorization((await H.T.Create()).token);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }
    }
}
