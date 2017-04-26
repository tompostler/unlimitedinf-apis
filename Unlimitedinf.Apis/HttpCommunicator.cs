using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace Unlimitedinf.Apis
{
    internal static class HttpCommunicator
    {
        public static async Task<TResult> Post<TContent, TResult>(string url, TContent content)
        {
            HttpResponseMessage response;
            using (var httpClient = new HttpClient())
            {
                response = await httpClient.PostAsync(url, new StringContent(JsonConvert.SerializeObject(content)));
            }
            string rontent = await response.Content.ReadAsStringAsync();
            ExceptionCreator.ThrowMaybe(response.StatusCode, rontent);
            return JsonConvert.DeserializeObject<TResult>(rontent);
        }

        public static async Task Put<TContent>(string url, TContent content)
        {
            HttpResponseMessage response;
            using (var httpClient = new HttpClient())
            {
                response = await httpClient.PutAsync(url, new StringContent(JsonConvert.SerializeObject(content)));
            }
            string rontent = await response.Content.ReadAsStringAsync();
            ExceptionCreator.ThrowMaybe(response.StatusCode, rontent);
        }

        public static async Task Delete<TContent>(string url, TContent content)
        {
            HttpResponseMessage response;
            using (var httpClient = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, url);
                request.Content = new StringContent(JsonConvert.SerializeObject(content));
                response = await httpClient.SendAsync(request);

            }
            string rontent = await response.Content.ReadAsStringAsync();
            ExceptionCreator.ThrowMaybe(response.StatusCode, rontent);
        }

        public static async Task<TResult> Get<TResult>(string url)
        {
            HttpResponseMessage response;
            using (var httpClient = new HttpClient())
            {
                response = await httpClient.GetAsync(url);
            }
            string rontent = await response.Content.ReadAsStringAsync();
            ExceptionCreator.ThrowMaybe(response.StatusCode, rontent);
            return JsonConvert.DeserializeObject<TResult>(rontent);
        }
    }
}
