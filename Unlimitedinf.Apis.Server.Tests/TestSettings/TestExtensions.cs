using System.Net.Http;

namespace Unlimitedinf.Apis.Server.IntTests
{
    public static class E
    {
        public static void AddAuthorization(this HttpRequestMessage req, string token)
        {
            req.Headers.Add("Authorization", "Token " + token);
        }
    }
}
