using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Unlimitedinf.Apis.Contracts.Auth;
using Xunit;

namespace Unlimitedinf.Apis.Server.IntTests
{
    public static class H
    {
        private static readonly HttpClient client = new HttpClient();

        public static string CreateUniqueAccountName([CallerMemberName] string callerName = "")
        {
            return $"{callerName.Substring(0, Math.Min(callerName.Length, 16))}_{DateTime.Now.ToString("MMdd_HHmmss_fff")}";
        }

        public static string CreateUniqueTokenName([CallerMemberName] string callerName = "")
        {
            return $"{callerName.Substring(0, Math.Min(callerName.Length, 48))}_{DateTime.Now.ToString("MMdd_HHmmss_fff")}";
        }

        public static StringContent JsonContent<T>(T content)
        {
            return JsonContent(JsonConvert.SerializeObject(content));
        }

        public static StringContent JsonContent(string content)
        {
            return new StringContent(content, Encoding.UTF8, "application/json");
        }

        public static class A
        {
            // Create one account for all the tests other than the account ones
            private static Account account;

            private static bool AlreadyCreated = false;
            public static async Task<Account> Create()
            {
                if (!AlreadyCreated)
                {
                    account = new Account
                    {
                        username = H.CreateUniqueAccountName(),
                        email = "test@postler.me",
                        secret = "test"
                    };

                    var req = new HttpRequestMessage(HttpMethod.Post, C.U.AuAccount);
                    req.Content = H.JsonContent(account);
                    var res = await client.SendAsync(req);
                    Assert.Equal(HttpStatusCode.Created, res.StatusCode);

                    AlreadyCreated = true;
                }
                return account;
            }
        }

        public static class T
        {
            // Create one token for all the tests other than the token ones
            private static Token token;

            private static bool AlreadyCreated = false;
            public static async Task<Token> Create(Account acc = null, bool useHA = true, bool useStored = true)
            {
                if (!AlreadyCreated || !useStored)
                {
                    if (useHA)
                        acc = await H.A.Create();
                    var tokCreate = new TokenCreate
                    {
                        username = acc.username,
                        secret = acc.secret,
                        name = H.CreateUniqueTokenName(),
                        expire = TokenExpiration.hour
                    };

                    var req = new HttpRequestMessage(HttpMethod.Post, C.U.AuToken);
                    req.Content = H.JsonContent(tokCreate);
                    var res = await client.SendAsync(req);
                    Assert.Equal(HttpStatusCode.Created, res.StatusCode);

                    var loctok = JsonConvert.DeserializeObject<Token>(await res.Content.ReadAsStringAsync());
                    if (!useStored)
                        return loctok;
                    token = loctok;
                    AlreadyCreated = true;
                }
                return token;
            }
        }
    }
}
