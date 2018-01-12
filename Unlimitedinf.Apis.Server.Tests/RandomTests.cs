using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Unlimitedinf.Apis.Server.IntTests
{
    public class RandomTests
    {
        private static readonly HttpClient client = new HttpClient();

        [Theory]
        [InlineData("", 0, 101)]
        [InlineData("/bit", 0, 1)]
        [InlineData("/crumb", 0, 1<<2)]
        [InlineData("/nibble", 0, 1<<4)]
        [InlineData("/byte", 0, 1<<8)]
        [InlineData("/short", 0, 1<<16)]
        [InlineData("/int", 0, uint.MaxValue)]
        [InlineData("/long", 0, ulong.MaxValue)]
        [InlineData("/bits/1", 0, 1)]
        [InlineData("/bits/8", 0, 1<<8)]
        public async Task Num(string uriSuffix, ulong min, ulong max /*inclusive*/)
        {
            var req = new HttpRequestMessage(HttpMethod.Get, C.U.Random + uriSuffix);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

            var act = UInt64.Parse(await res.Content.ReadAsStringAsync());
            Assert.InRange(act, min, max);
        }

        [Fact]
        public async Task Guid()
        {
            var req = new HttpRequestMessage(HttpMethod.Get, C.U.Random + "/guid");
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

            // Exception being thrown here will fail test
            System.Guid.Parse(await res.Content.ReadAsStringAsync());
        }
    }
}
