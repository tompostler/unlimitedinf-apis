using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Unlimitedinf.Apis.Server.IntTests
{
    public class BasicTests
    {
        private static readonly HttpClient client = new HttpClient();

        [Theory]
        [InlineData("GET")]
        [InlineData("HEAD")]
        [InlineData("OPTIONS")]
        public async Task Ping(string smethod)
        {
            var method = new HttpMethod(smethod);
            var req = new HttpRequestMessage(method, C.U.Ping);
            var res = await client.SendAsync(req);

            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        }

        [Theory]
        [InlineData("DELETE")]
        [InlineData("GET")]
        [InlineData("HEAD")]
        [InlineData("OPTIONS")]
        [InlineData("PATCH")]
        [InlineData("POST")]
        [InlineData("PUT")]
        public async Task Teapot(string smethod)
        {
            var method = new HttpMethod(smethod);
            var req = new HttpRequestMessage(method, C.U.Teapot);
            var res = await client.SendAsync(req);

            Assert.Equal(418, (int)res.StatusCode);
        }
    }
}
