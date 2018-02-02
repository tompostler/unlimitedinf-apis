using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Unlimitedinf.Apis.Contracts.Auth;
using Unlimitedinf.Tools;
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

            var req = new HttpRequestMessage(HttpMethod.Post, C.U.AuAccount)
            {
                Content = H.JsonContent(acc)
            };
            var res = await client.SendAsync(req);

            Assert.Equal(HttpStatusCode.Created, res.StatusCode);
            var act = JsonConvert.DeserializeObject<Account>(await res.Content.ReadAsStringAsync());
            TAssert.Equal(acc, act);
        }

        [Fact]
        public async Task AccountCreateDuplicate()
        {
            var acc = await H.A.Create();

            var req = new HttpRequestMessage(HttpMethod.Post, C.U.AuAccount)
            {
                Content = H.JsonContent(acc)
            };
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
            var req = new HttpRequestMessage(HttpMethod.Post, C.U.AuAccount)
            {
                Content = H.JsonContent(acc)
            };
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.Created, res.StatusCode);

            var accupd = new AccountUpdate
            {
                secret = "test2",
                oldsecret = "test"
            };
            req = new HttpRequestMessage(HttpMethod.Put, C.U.AuAccount + "/" + acc.username)
            {
                Content = H.JsonContent(accupd)
            };
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

            var req = new HttpRequestMessage(HttpMethod.Put, C.U.AuAccount + "/" + acc.username)
            {
                Content = H.JsonContent(accupd)
            };
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

            var req = new HttpRequestMessage(HttpMethod.Put, C.U.AuAccount + "/" + H.CreateUniqueAccountName())
            {
                Content = H.JsonContent(accupd)
            };
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

            var req = new HttpRequestMessage(HttpMethod.Put, C.U.AuAccount + "/" + acc.username)
            {
                Content = H.JsonContent(accupd)
            };
            var res = await client.SendAsync(req);

            Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
        }

        [Fact]
        public async Task TokenCreate()
        {
            var acc = await H.A.Create();
            var tok = new TokenCreate
            {
                username = acc.username,
                secret = acc.secret,
                name = H.CreateUniqueTokenName(),
                expire = TokenExpiration.hour
            };

            var req = new HttpRequestMessage(HttpMethod.Post, C.U.AuToken)
            {
                Content = H.JsonContent(tok)
            };
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.Created, res.StatusCode);

            var con = await res.Content.ReadAsStringAsync();
            var act = JsonConvert.DeserializeObject<Token>(con);
            TAssert.Equal(tok, act);
        }

        [Fact]
        public async Task TokenCreateInvalidAccount()
        {
            var acc = await H.A.Create();
            var tok = new TokenCreate
            {
                username = H.CreateUniqueAccountName(),
                secret = acc.secret,
                name = H.CreateUniqueTokenName(),
                expire = TokenExpiration.hour
            };

            var req = new HttpRequestMessage(HttpMethod.Post, C.U.AuToken)
            {
                Content = H.JsonContent(tok)
            };
            var res = await client.SendAsync(req);

            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task TokenCreateInvalidSecret()
        {
            var acc = await H.A.Create();
            var tok = new TokenCreate
            {
                username = acc.username,
                secret = acc.secret + "1",
                name = H.CreateUniqueTokenName(),
                expire = TokenExpiration.hour
            };

            var req = new HttpRequestMessage(HttpMethod.Post, C.U.AuToken)
            {
                Content = H.JsonContent(tok)
            };
            var res = await client.SendAsync(req);

            Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
        }

        [Fact]
        public async Task TokenCreateDuplicate()
        {
            var acc = await H.A.Create();
            var tok = await H.T.Create();

            var tokCre = new TokenCreate
            {
                username = tok.username,
                name = tok.name,
                secret = acc.secret,
                expire = TokenExpiration.minute
            };

            var req = new HttpRequestMessage(HttpMethod.Post, C.U.AuToken)
            {
                Content = H.JsonContent(tokCre)
            };
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.Conflict, res.StatusCode);
        }

        [Fact]
        public async Task TokenDelete()
        {
            // Need to create a token that can be deleted separate from the rest of the tests
            var tok = await H.T.CreateNew();

            var res = await client.DeleteAsync($"{C.U.AuToken}/{tok.token}");
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

            var con = await res.Content.ReadAsStringAsync();
            var act = JsonConvert.DeserializeObject<Token>(con);
            TAssert.Equal(tok, act);
        }

        [Fact]
        public async Task TokenDeleteUsernameNotFound()
        {
            // <datetime string> <username> <hex fill to 96 chars>
            // Since usernames cannot contain whitespace, this will work
            var token = string.Format(
                CultureInfo.InvariantCulture,
                "{0} {1} {2}",
                DateTime.Now.AddDays(1).ToString(Token.DateTimeFmt),
                H.CreateUniqueAccountName(),
                GenerateRandom.HexToken(96)
                ).Chop(96).ToBase64String();

            var res = await client.DeleteAsync($"{C.U.AuToken}/{token}");
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task TokenDeleteTokenNotFound()
        {
            var tok = await H.T.CreateNew();
            // <datetime string> <username> <hex fill to 96 chars>
            // Since usernames cannot contain whitespace, this will work
            var token = string.Format(
                CultureInfo.InvariantCulture,
                "{0} {1} {2}",
                DateTime.Now.AddDays(1).ToString(Token.DateTimeFmt),
                tok.username,
                GenerateRandom.HexToken(96)
                ).Chop(96).ToBase64String();

            var res = await client.DeleteAsync($"{C.U.AuToken}/{token}");
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task AccountDelete()
        {
            var acc = await H.A.CreateNew();
            var tok = await H.T.CreateNew(acc);
            var accDel = new AccountDelete
            {
                secret = acc.secret
            };

            var req = new HttpRequestMessage(HttpMethod.Delete, C.U.AuAccount + "/" + acc.username)
            {
                Content = H.JsonContent(accDel)
            };
            req.AddAuthorization(tok.token);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

            var con = await res.Content.ReadAsStringAsync();
            var act = JsonConvert.DeserializeObject<Account>(con);
        }

        [Fact]
        public async Task AccountDeleteTokenDoesntMatch()
        {
            var acc = await H.A.Create();
            var tok = await H.T.Create();
            var accDel = new AccountDelete
            {
                secret = acc.secret
            };

            var req = new HttpRequestMessage(HttpMethod.Delete, C.U.AuAccount + "/" + acc.username + "1")
            {
                Content = H.JsonContent(accDel)
            };
            req.AddAuthorization(tok.token);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        }

        [Fact]
        public async Task AccountDeleteBadAccountSecret()
        {
            var acc = await H.A.Create();
            var tok = await H.T.Create();
            var accDel = new AccountDelete
            {
                secret = acc.secret + "1"
            };

            var req = new HttpRequestMessage(HttpMethod.Delete, C.U.AuAccount + "/" + acc.username)
            {
                Content = H.JsonContent(accDel)
            };
            req.AddAuthorization(tok.token);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
        }
    }
}
