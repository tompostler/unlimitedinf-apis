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
    public class MessageTests
    {
        private static readonly HttpClient client = new HttpClient();

        /// <summary>
        /// The 'to' of the message is the default account and token (since more operations are done on retrieval).
        /// </summary>
        private async Task<Message> CreateMessageForTests()
        {
            var acc1 = await H.A.CreateNew();
            var tok1 = await H.T.CreateNew(acc1);
            var acc2 = await H.A.Create();
            var tok2 = await H.T.Create();

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
            TAssert.Equal(msg, act);

            return act;
        }

        [Fact]
        public async Task Create()
        {
            await CreateMessageForTests();
        }

        [Fact]
        public async Task CreateBadUsername()
        {
            var acc1 = await H.A.Create();
            var tok1 = await H.T.Create();

            var msg = new Message
            {
                from = acc1.username + "1",
                to = H.CreateUniqueAccountName(),
                subject = "test",
                message = "new message test"
            };

            var req = new HttpRequestMessage(HttpMethod.Post, C.U.Message)
            {
                Content = H.JsonContent(msg)
            };
            req.AddAuthorization(tok1.token);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
        }

        [Fact]
        public async Task CreateNotFound()
        {
            var acc1 = await H.A.Create();
            var tok1 = await H.T.Create();

            var msg = new Message
            {
                from = acc1.username,
                to = H.CreateUniqueAccountName(),
                subject = "test",
                message = "new message test"
            };

            var req = new HttpRequestMessage(HttpMethod.Post, C.U.Message)
            {
                Content = H.JsonContent(msg)
            };
            req.AddAuthorization(tok1.token);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        }

        [Fact]
        public async Task MarkAsRead()
        {
            var msg = await CreateMessageForTests();
            msg.read = true;

            var req = new HttpRequestMessage(new HttpMethod("PATCH"), C.U.Message + $"/{msg.id}/markread");
            req.AddAuthorization((await H.T.Create()).token);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

            var con = await res.Content.ReadAsStringAsync();
            var act = JsonConvert.DeserializeObject<Message>(con);
            TAssert.Equal(msg, act);
        }

        [Fact]
        public async Task MarkAsReadNotFound()
        {
            var req = new HttpRequestMessage(new HttpMethod("PATCH"), C.U.Message + $"/{Guid.NewGuid()}/markread");
            req.AddAuthorization((await H.T.Create()).token);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task Get()
        {
            var msg = await CreateMessageForTests();

            var req = new HttpRequestMessage(HttpMethod.Get, C.U.Message);
            req.AddAuthorization((await H.T.Create()).token);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

            var con = await res.Content.ReadAsStringAsync();
            var act = JsonConvert.DeserializeObject<List<Message>>(con);
            TAssert.Equal(msg, act.Single(_ => _.id == msg.id));
        }

        [Fact]
        public async Task GetMarkedAsRead()
        {
            var msg = await CreateMessageForTests();
            msg.read = true;

            var req = new HttpRequestMessage(new HttpMethod("PATCH"), C.U.Message + $"/{msg.id}/markread");
            req.AddAuthorization((await H.T.Create()).token);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

            var con = await res.Content.ReadAsStringAsync();
            var act = JsonConvert.DeserializeObject<Message>(con);
            TAssert.Equal(msg, act);

            // Get for unread only to make sure it's not there
            req = new HttpRequestMessage(HttpMethod.Get, C.U.Message);
            req.AddAuthorization((await H.T.Create()).token);
            res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

            con = await res.Content.ReadAsStringAsync();
            var act2 = JsonConvert.DeserializeObject<List<Message>>(con);
            Assert.Null(act2.SingleOrDefault(_ => _.id == msg.id));

            // Get including read to make sure it's there
            req = new HttpRequestMessage(HttpMethod.Get, C.U.Message + "?unreadOnly=false");
            req.AddAuthorization((await H.T.Create()).token);
            res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

            con = await res.Content.ReadAsStringAsync();
            act2 = JsonConvert.DeserializeObject<List<Message>>(con);
            TAssert.Equal(msg, act2.Single(_ => _.id == msg.id));
        }

        [Fact]
        public async Task Delete()
        {
            var msg = await CreateMessageForTests();

            var req = new HttpRequestMessage(HttpMethod.Delete, C.U.Message + $"/{msg.id}");
            req.AddAuthorization((await H.T.Create()).token);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.NoContent, res.StatusCode);
        }

        [Fact]
        public async Task DeleteNotFound()
        {
            var req = new HttpRequestMessage(HttpMethod.Delete, C.U.Message + $"/{Guid.NewGuid()}");
            req.AddAuthorization((await H.T.Create()).token);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task Deletes()
        {
            var msg1 = await CreateMessageForTests();
            var msg2 = await CreateMessageForTests();
            var guid = Guid.NewGuid();

            var req = new HttpRequestMessage(HttpMethod.Delete, C.U.Message + $"?ids={msg1.id}&ids={msg2.id}&ids={guid}");
            req.AddAuthorization((await H.T.Create()).token);
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

            var con = await res.Content.ReadAsStringAsync();
            var act = JsonConvert.DeserializeObject<Dictionary<Guid, HttpStatusCode>>(con);
            Assert.Equal(HttpStatusCode.NoContent, act[msg1.id.Value]);
            Assert.Equal(HttpStatusCode.NoContent, act[msg2.id.Value]);
            Assert.Equal(HttpStatusCode.NotFound, act[guid]);
        }
    }
}
