using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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

        public static string CreateUniqueName([CallerMemberName] string callerName = "")
        {
            return $"{callerName.Substring(0, Math.Min(callerName.Length, 32))}_{DateTime.Now.ToString("MMdd_HHmmss_fff")}_{Tools.GenerateRandom.HexToken(10)}";
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

            private static SemaphoreSlim semaphore = new SemaphoreSlim(1);
            private static bool AlreadyCreated = false;
            public static async Task<Account> Create()
            {
                if (!AlreadyCreated)
                {
                    await semaphore.WaitAsync();
                    if (!AlreadyCreated)
                    {
                        account = await CreateNew();
                        AlreadyCreated = true;
                    }
                    semaphore.Release();
                }
                return account;
            }

            public static async Task<Account> CreateNew()
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

                return acc;
            }
        }

        public static class T
        {
            // Create one token for all the tests other than the token ones
            private static Token token;

            private static SemaphoreSlim semaphore = new SemaphoreSlim(1);
            private static bool AlreadyCreated = false;
            public static async Task<Token> Create()
            {
                if (!AlreadyCreated)
                {
                    await semaphore.WaitAsync();
                    if (!AlreadyCreated)
                    {
                        token = await CreateNew();
                        AlreadyCreated = true;
                    }
                    semaphore.Release();
                }
                return token;
            }

            public static async Task<Token> CreateNew()
            {
                var acc = await H.A.Create();
                return await CreateNew(acc);
            }

            public static async Task<Token> CreateNew(Account acc)
            {
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

                return JsonConvert.DeserializeObject<Token>(await res.Content.ReadAsStringAsync());
            }
        }
    }
}
