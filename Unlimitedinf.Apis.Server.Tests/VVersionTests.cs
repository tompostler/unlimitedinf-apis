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

        [Fact]
        public async Task Create()
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
        }
    }
}
