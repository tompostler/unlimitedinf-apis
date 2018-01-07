using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Unlimitedinf.Apis.Contracts.Auth;
using Xunit;

namespace Unlimitedinf.Apis.Server.IntTests
{
    public class AuthTests
    {
        private static readonly HttpClient client = new HttpClient();

        [Fact]
        public async Task AccountCreate()
        {
            var acc = new Account
            {
                username = H.CreateUniqueAccountName(),
                email = "test@postler.me",
                secret = "test"
            };

            var req = new HttpRequestMessage(HttpMethod.Post, C.U.AuAccount);
            req.Content = H.JsonContent(acc);
            var res = await client.SendAsync(req);

            Assert.Equal(HttpStatusCode.Created, res.StatusCode);
        }

        [Fact]
        public async Task AccountCreateDuplicate()
        {
            var acc = await H.A.Create();

            var req = new HttpRequestMessage(HttpMethod.Post, C.U.AuAccount);
            req.Content = H.JsonContent(acc);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.Conflict, res.StatusCode);
        }

        [Fact]
        public async Task AccountRead()
        {
            var acc = await H.A.Create();

            var req = new HttpRequestMessage(HttpMethod.Get, C.U.AuAccount + "/" + acc.username);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            var act = JsonConvert.DeserializeObject<Account>(await res.Content.ReadAsStringAsync());
            TAssert.Equal(acc, act);

            req = new HttpRequestMessage(HttpMethod.Get, C.U.AuAccount + "?username=" + acc.username);
            res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            act = JsonConvert.DeserializeObject<Account>(await res.Content.ReadAsStringAsync());
            TAssert.Equal(acc, act);
        }

        [Fact]
        public async Task AccountUpdate()
        {
            // Have to create one to modify the secret so it doesn't mess up the rest of the tests
            var acc = new Account
            {
                username = H.CreateUniqueAccountName(),
                email = "test@postler.me",
                secret = "test"
            };
            var req = new HttpRequestMessage(HttpMethod.Post, C.U.AuAccount);
            req.Content = H.JsonContent(acc);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.Created, res.StatusCode);

            var accupd = new AccountUpdate
            {
                secret = "test2",
                oldsecret = "test"
            };
            req = new HttpRequestMessage(HttpMethod.Put, C.U.AuAccount + "/" + acc.username);
            req.Content = H.JsonContent(accupd);
            res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
            var act = JsonConvert.DeserializeObject<Account>(await res.Content.ReadAsStringAsync());
            TAssert.Equal(acc, act);
        }

        [Fact]
        public async Task AccountUpdateSameSecret()
        {
            var acc = await H.A.Create();

            var accupd = new AccountUpdate
            {
                secret = "test",
                oldsecret = "test"
            };

            var req = new HttpRequestMessage(HttpMethod.Put, C.U.AuAccount + "/" + acc.username);
            req.Content = H.JsonContent(accupd);
            var res = await client.SendAsync(req);

            Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        }

        [Fact]
        public async Task AccountUpdateNotFound()
        {
            var accupd = new AccountUpdate
            {
                secret = "test2",
                oldsecret = "test"
            };

            var req = new HttpRequestMessage(HttpMethod.Put, C.U.AuAccount + "/" + H.CreateUniqueAccountName());
            req.Content = H.JsonContent(accupd);
            var res = await client.SendAsync(req);

            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task AccountUpdateBadSecret()
        {
            var acc = await H.A.Create();

            var accupd = new AccountUpdate
            {
                secret = "test",
                oldsecret = "test2"
            };

            var req = new HttpRequestMessage(HttpMethod.Put, C.U.AuAccount + "/" + acc.username);
            req.Content = H.JsonContent(accupd);
            var res = await client.SendAsync(req);

            Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
        }
    }
}
