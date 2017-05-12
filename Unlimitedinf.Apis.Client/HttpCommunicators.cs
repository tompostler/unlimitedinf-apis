using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Unlimitedinf.Apis.Client
{
    internal static class StaticHttpCommunicator
    {
        private static HttpClient StaticClient = new HttpClient();

        static StaticHttpCommunicator()
        {
            StaticClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public static async Task<TResult> Post<TContent, TResult>(string url, TContent content)
        {
            HttpResponseMessage response = await StaticClient.PostAsync(url, new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json"));
            string rontent = await response.Content.ReadAsStringAsync();
            ExceptionCreator.ThrowMaybe(response.StatusCode, rontent);
            return JsonConvert.DeserializeObject<TResult>(rontent);
        }

        public static async Task Put<TContent>(string url, TContent content)
        {
            HttpResponseMessage response = await StaticClient.PutAsync(url, new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json"));
            string rontent = await response.Content.ReadAsStringAsync();
            ExceptionCreator.ThrowMaybe(response.StatusCode, rontent);
        }

        public static async Task Delete<TContent>(string url, TContent content)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json")
            };
            HttpResponseMessage response = await StaticClient.SendAsync(request);
            string rontent = await response.Content.ReadAsStringAsync();
            ExceptionCreator.ThrowMaybe(response.StatusCode, rontent);
        }

        public static async Task<TResult> Get<TResult>(string url)
        {
            HttpResponseMessage response = await StaticClient.GetAsync(url);
            string rontent = await response.Content.ReadAsStringAsync();
            ExceptionCreator.ThrowMaybe(response.StatusCode, rontent);
            return JsonConvert.DeserializeObject<TResult>(rontent);
        }
    }

    internal sealed class HttpCommunicator : IDisposable
    {
        private HttpClient Client = new HttpClient();

        public HttpCommunicator(string token)
        {
            this.Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", token);
        }

        public async Task<TResult> Post<TContent, TResult>(string url, TContent content)
        {
            HttpResponseMessage response = await this.Client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json"));
            string rontent = await response.Content.ReadAsStringAsync();
            ExceptionCreator.ThrowMaybe(response.StatusCode, rontent);
            return JsonConvert.DeserializeObject<TResult>(rontent);
        }

        public async Task Put<TContent>(string url, TContent content)
        {
            HttpResponseMessage response = await this.Client.PutAsync(url, new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json"));
            string rontent = await response.Content.ReadAsStringAsync();
            ExceptionCreator.ThrowMaybe(response.StatusCode, rontent);
        }

        public async Task<TResult> Put<TContent, TResult>(string url, TContent content)
        {
            HttpResponseMessage response = await this.Client.PutAsync(url, new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json"));
            string rontent = await response.Content.ReadAsStringAsync();
            ExceptionCreator.ThrowMaybe(response.StatusCode, rontent);
            return JsonConvert.DeserializeObject<TResult>(rontent);
        }

        public async Task Delete<TContent>(string url, TContent content)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json")
            };
            HttpResponseMessage response = await this.Client.SendAsync(request);
            string rontent = await response.Content.ReadAsStringAsync();
            ExceptionCreator.ThrowMaybe(response.StatusCode, rontent);
        }

        public async Task<TResult> Delete<TContent, TResult>(string url, TContent content)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(content), Encoding.UTF8, "application/json")
            };
            HttpResponseMessage response = await this.Client.SendAsync(request);
            string rontent = await response.Content.ReadAsStringAsync();
            ExceptionCreator.ThrowMaybe(response.StatusCode, rontent);
            return JsonConvert.DeserializeObject<TResult>(rontent);
        }

        public async Task<TResult> Delete<TResult>(string url)
        {
            HttpResponseMessage response = await this.Client.DeleteAsync(url);
            string rontent = await response.Content.ReadAsStringAsync();
            ExceptionCreator.ThrowMaybe(response.StatusCode, rontent);
            return JsonConvert.DeserializeObject<TResult>(rontent);
        }

        public async Task<TResult> Get<TResult>(string url)
        {
            HttpResponseMessage response = await this.Client.GetAsync(url);
            string rontent = await response.Content.ReadAsStringAsync();
            ExceptionCreator.ThrowMaybe(response.StatusCode, rontent);
            return JsonConvert.DeserializeObject<TResult>(rontent);
        }

        public void Dispose()
        {
            this.Client.Dispose();
        }
    }
}
