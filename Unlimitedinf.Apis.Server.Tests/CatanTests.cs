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
    public class CatanTests
    {
        private static readonly HttpClient client = new HttpClient();
        private static Random random = new Random();

        private Catan.Roll CreateRollForTests(int minutesAgo)
        {
            return new Catan.Roll
            {
                d = DateTimeOffset.Now.AddMinutes(-minutesAgo),
                r = random.Next(1, 7),
                y = random.Next(1, 7)
            };
        }

        private async Task<Catan> CreateCatanForTests()
        {
            var tok = await H.T.Create();

            var cat = new Catan
            {
                username = tok.username,
                name = H.CreateUniqueName(),
                rolls = new List<Catan.Roll>
                {
                    this.CreateRollForTests(5),
                    this.CreateRollForTests(4),
                    this.CreateRollForTests(3),
                    this.CreateRollForTests(2),
                    this.CreateRollForTests(1),
                    this.CreateRollForTests(0)
                }
            };

            var req = new HttpRequestMessage(HttpMethod.Post, C.U.Catan)
            {
                Content = H.JsonContent(cat)
            };
            req.AddAuthorization(tok.token);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.Created, res.StatusCode);

            var con = await res.Content.ReadAsStringAsync();
            var act = JsonConvert.DeserializeObject<Catan>(con);
            TAssert.Equal(cat, act);

            return act;
        }

        [Fact]
        public async Task Create()
        {
            await this.CreateCatanForTests();
        }
        
        [Fact]
        public async Task CreateBadUsername()
        {
            var tok = await H.T.Create();

            var cat = new Catan
            {
                username = H.CreateUniqueAccountName(),
                name = H.CreateUniqueName(),
                rolls = new List<Catan.Roll>
                {
                    this.CreateRollForTests(5),
                    this.CreateRollForTests(4),
                    this.CreateRollForTests(3),
                    this.CreateRollForTests(2),
                    this.CreateRollForTests(1),
                    this.CreateRollForTests(0)
                }
            };

            var req = new HttpRequestMessage(HttpMethod.Post, C.U.Catan)
            {
                Content = H.JsonContent(cat)
            };
            req.AddAuthorization(tok.token);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
        }

        [Fact]
        public async Task Get()
        {
            var cat = await this.CreateCatanForTests();

            var res = await client.GetAsync($"{C.U.Catan}/{cat.username}/{cat.name}");
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

            var con = await res.Content.ReadAsStringAsync();
            var act = JsonConvert.DeserializeObject<Catan>(con);
            TAssert.Equal(cat, act);
        }

        [Fact]
        public async Task GetNotFound()
        {
            var cat = await this.CreateCatanForTests();

            var res = await client.GetAsync($"{C.U.Catan}/{cat.username}/{cat.name}1");
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task Gets()
        {
            var cat1 = await this.CreateCatanForTests();
            var cat2 = await this.CreateCatanForTests();

            var res = await client.GetAsync($"{C.U.Catan}/{cat1.username}");
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

            var con = await res.Content.ReadAsStringAsync();
            var act = JsonConvert.DeserializeObject<List<Catan>>(con);
            TAssert.Equal(cat1, act.Single(_ => _.name.Equals(cat1.name)));
            TAssert.Equal(cat2, act.Single(_ => _.name.Equals(cat2.name)));
        }

        [Fact]
        public async Task GetsNotFound()
        {
            var res = await client.GetAsync($"{C.U.Catan}/{H.CreateUniqueAccountName()}");
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

            var con = await res.Content.ReadAsStringAsync();
            var act = JsonConvert.DeserializeObject<List<Catan>>(con);
            Assert.Empty(act);
        }

        [Fact]
        public async Task Remove()
        {
            var cat = await this.CreateCatanForTests();

            var req = new HttpRequestMessage(HttpMethod.Delete, $"{C.U.Catan}/{cat.name}");
            req.AddAuthorization((await H.T.Create()).token);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

            var con = await res.Content.ReadAsStringAsync();
            var act = JsonConvert.DeserializeObject<Catan>(con);
            TAssert.Equal(cat, act);
        }

        [Fact]
        public async Task RemoveNotFound()
        {
            var req = new HttpRequestMessage(HttpMethod.Delete, $"{C.U.Catan}/{H.CreateUniqueName()}");
            req.AddAuthorization((await H.T.Create()).token);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }
    }
}
