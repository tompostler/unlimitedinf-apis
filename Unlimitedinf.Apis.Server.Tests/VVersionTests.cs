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
    public class VVersionTests
    {
        private static readonly HttpClient client = new HttpClient();

        private async Task<Version> CreateVersionForTest()
        {
            var tok = await H.T.Create();
            var ver = new Version
            {
                username = tok.username,
                name = H.CreateUniqueName(),
                version = SemVer.Parse("1.2.3")
            };

            var req = new HttpRequestMessage(HttpMethod.Post, C.U.VeVersion)
            {
                Content = H.JsonContent(ver)
            };
            req.AddAuthorization(tok.token);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.Created, res.StatusCode);

            return ver;
        }

        [Fact]
        public async Task Create()
        {
            await CreateVersionForTest();
        }

        [Fact]
        public async Task CreateBadUsername()
        {
            var tok = await H.T.Create();
            var ver = new Version
            {
                username = tok.username + "1",
                name = H.CreateUniqueName(),
                version = SemVer.Parse("1.2.3")
            };

            var req = new HttpRequestMessage(HttpMethod.Post, C.U.VeVersion)
            {
                Content = H.JsonContent(ver)
            };
            req.AddAuthorization(tok.token);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
        }

        [Fact]
        public async Task Get()
        {
            var ver = await CreateVersionForTest();

            var res = await client.GetAsync(C.U.VeVersion + $"/{ver.username}/{ver.name}");
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

            var con = await res.Content.ReadAsStringAsync();
            var act = JsonConvert.DeserializeObject<Version>(con);
            TAssert.Equal(ver, act);
        }

        [Fact]
        public async Task GetNotFound()
        {
            var ver = await CreateVersionForTest();

            // Bad username
            var res = await client.GetAsync(C.U.VeVersion + $"/{ver.username}1/{ver.name}");
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);

            // Bad versionName
            res = await client.GetAsync(C.U.VeVersion + $"/{ver.username}/{ver.name}1");
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task Gets()
        {
            var ver1 = await CreateVersionForTest();
            var ver2 = await CreateVersionForTest();

            var res = await client.GetAsync(C.U.VeVersion + $"/{ver1.username}");
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

            var con = await res.Content.ReadAsStringAsync();
            var act = JsonConvert.DeserializeObject<List<Version>>(con);
            TAssert.Equal(ver1, act.Single(_ => _.name.Equals(ver1.name)));
            TAssert.Equal(ver2, act.Single(_ => _.name.Equals(ver2.name)));
        }

        [Fact]
        public async Task GetNotFounds()
        {
            var ver1 = await CreateVersionForTest();
            var ver2 = await CreateVersionForTest();

            var res = await client.GetAsync(C.U.VeVersion + $"/{ver1.username}1");
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Theory]
        [InlineData(VersionIncrementOption.major, false, "2.2.3")]
        [InlineData(VersionIncrementOption.major, true, "2.0.0")]
        [InlineData(VersionIncrementOption.minor, false, "1.3.3")]
        [InlineData(VersionIncrementOption.minor, true, "1.3.0")]
        [InlineData(VersionIncrementOption.patch, false, "1.2.4")]
        [InlineData(VersionIncrementOption.patch, true, "1.2.4")]
        public async Task Increment(VersionIncrementOption incOpt, bool reset, string exp)
        {
            var ver = await CreateVersionForTest();
            var inc = new VersionIncrement
            {
                inc = incOpt,
                reset = reset
            };

            var req = new HttpRequestMessage(new HttpMethod("PATCH"), C.U.VeVersion + $"/{ver.username}/{ver.name}")
            {
                Content = H.JsonContent(inc)
            };
            req.AddAuthorization((await H.T.Create()).token);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

            ver.version = SemVer.Parse(exp);
            var con = await res.Content.ReadAsStringAsync();
            var act = JsonConvert.DeserializeObject<Version>(con);
            TAssert.Equal(ver, act);
        }

        [Fact]
        public async Task Delete()
        {
            var ver = await CreateVersionForTest();

            var req = new HttpRequestMessage(HttpMethod.Delete, C.U.VeVersion + $"/{ver.username}/{ver.name}");
            req.AddAuthorization((await H.T.Create()).token);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

            var con = await res.Content.ReadAsStringAsync();
            var act = JsonConvert.DeserializeObject<Version>(con);
            TAssert.Equal(ver, act);
        }

        [Fact]
        public async Task DeleteBadUsername()
        {
            var ver = await CreateVersionForTest();

            var req = new HttpRequestMessage(HttpMethod.Delete, C.U.VeVersion + $"/{ver.username}1/{ver.name}");
            req.AddAuthorization((await H.T.Create()).token);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
        }

        [Fact]
        public async Task DeleteBadVerName()
        {
            var ver = await CreateVersionForTest();

            var req = new HttpRequestMessage(HttpMethod.Delete, C.U.VeVersion + $"/{ver.username}/{ver.name}1");
            req.AddAuthorization((await H.T.Create()).token);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }
    }
}
