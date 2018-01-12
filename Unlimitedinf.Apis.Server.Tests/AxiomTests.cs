using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Unlimitedinf.Apis.Server.IntTests
{
    public class AxiomTests
    {
        private static readonly HttpClient client = new HttpClient();

        [Theory]

        [InlineData(100), InlineData(101), InlineData(102)]

        [InlineData(200), InlineData(201), InlineData(202), InlineData(203), InlineData(204), InlineData(205), InlineData(206)]
        [InlineData(207), InlineData(208), InlineData(226)]

        [InlineData(300), InlineData(301), InlineData(302), InlineData(303), InlineData(304), InlineData(305), InlineData(306)]
        [InlineData(307), InlineData(308)]

        [InlineData(400), InlineData(401), InlineData(402), InlineData(403), InlineData(404), InlineData(405), InlineData(406)]
        [InlineData(407), InlineData(408), InlineData(409), InlineData(410), InlineData(411), InlineData(412), InlineData(413)]
        [InlineData(414), InlineData(415), InlineData(416), InlineData(417), InlineData(418), InlineData(421), InlineData(422)]
        [InlineData(423), InlineData(424), InlineData(426), InlineData(428), InlineData(429), InlineData(431)]

        [InlineData(500), InlineData(501), InlineData(502), InlineData(503), InlineData(504), InlineData(505), InlineData(506)]
        [InlineData(507), InlineData(508), InlineData(510), InlineData(511)]

        public async Task Http(int statusCode)
        {
            var res = await client.GetAsync(C.U.Axiom + $"/http/{statusCode}");
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        }
    }
}
