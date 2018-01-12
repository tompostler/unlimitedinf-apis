using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Unlimitedinf.Apis.Contracts;
using Xunit;

namespace Unlimitedinf.Apis.Server.IntTests
{
    public class MessageTests
    {
        private static readonly HttpClient client = new HttpClient();

        [Fact]
        public async Task Create()
        {
            var acc1 = await H.A.Create();
            var tok1 = await H.T.Create();
            var acc2 = await H.A.CreateNew();
            var tok2 = await H.T.CreateNew(acc2);

            var msg = new Message
            {
                from = acc1.username,
                to = acc2.username,
                subject = "test",
                message = "new message test"
            };

            var req = new HttpRequestMessage(HttpMethod.Post, C.U.Message)
            {
                Content = H.JsonContent(msg)
            };
            req.AddAuthorization(tok1.token);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.Created, res.StatusCode);

            var con = await res.Content.ReadAsStringAsync();
            var act = JsonConvert.DeserializeObject<Message>(con);
        }
    }
}
