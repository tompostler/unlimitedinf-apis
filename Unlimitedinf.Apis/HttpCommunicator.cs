using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Unlimitedinf.Apis
{
    internal static class HttpCommunicator
    {
        private static HttpClient Client = new HttpClient();

        static HttpCommunicator()
        {
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static async Task<TResult> Post<TContent, TResult>(string url, TContent content)
        {
            HttpResponseMessage response = await Client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json"));
            string rontent = await response.Content.ReadAsStringAsync();
            ExceptionCreator.ThrowMaybe(response.StatusCode, rontent);
            return JsonConvert.DeserializeObject<TResult>(rontent);
        }

        public static async Task Put<TContent>(string url, TContent content)
        {
            HttpResponseMessage response = await Client.PutAsync(url, new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json"));
            string rontent = await response.Content.ReadAsStringAsync();
            ExceptionCreator.ThrowMaybe(response.StatusCode, rontent);
        }

        public static async Task Delete<TContent>(string url, TContent content)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(content))
            };
            HttpResponseMessage response = await Client.SendAsync(request);
            string rontent = await response.Content.ReadAsStringAsync();
            ExceptionCreator.ThrowMaybe(response.StatusCode, rontent);
        }

        public static async Task<TResult> Get<TResult>(string url)
        {
            HttpResponseMessage response = await Client.GetAsync(url);
            string rontent = await response.Content.ReadAsStringAsync();
            ExceptionCreator.ThrowMaybe(response.StatusCode, rontent);
            return JsonConvert.DeserializeObject<TResult>(rontent);
        }
    }
}
