using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Unlimitedinf.Apis.Server.IntTests
{
    public class DateTimesTests
    {
        private static readonly HttpClient client = new HttpClient();

        [Fact]
        public async Task AllIndividual()
        {
            // TODO: CodeGen this thing into individual tests?
            var testCases = new Dictionary<string, int>
            {
                { "yea", DateTimeOffset.UtcNow.Year },
                { "mon", DateTimeOffset.UtcNow.Month },
                { "dom", DateTimeOffset.UtcNow.Day },
                { "doy", DateTimeOffset.UtcNow.DayOfYear },
                { "dow", (int)DateTimeOffset.UtcNow.DayOfWeek },
                { "hou", DateTimeOffset.UtcNow.Hour },
                { "min", DateTimeOffset.UtcNow.Minute }
            };

            foreach (var testCase in testCases)
            {
                var req = new HttpRequestMessage(HttpMethod.Get, C.U.DateTime + "/" + testCase.Key);
                var res = await client.SendAsync(req);
                Assert.Equal(HttpStatusCode.OK, res.StatusCode);

                Assert.InRange(Int32.Parse(await res.Content.ReadAsStringAsync()), testCase.Value, testCase.Value + 1);
            }
        }

        [Fact]
        public async Task IsoTest()
        {
            var exp = DateTimeOffset.UtcNow.ToString("yyyy-MM-ddTHH:");
            var req = new HttpRequestMessage(HttpMethod.Get, C.U.DateTime + "/iso");
            var res = await client.SendAsync(req);
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

            var act = await res.Content.ReadAsStringAsync();
            Assert.StartsWith(exp, act);
        }
    }
}
