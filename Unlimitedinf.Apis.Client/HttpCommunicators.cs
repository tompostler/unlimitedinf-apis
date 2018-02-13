using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Client
{
    internal static class StaticHttpCommunicator
    {
        private static HttpClient StaticClient = new HttpClient();

        static StaticHttpCommunicator()
        {
            StaticClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        internal static async Task<TResult> Post<TResult>(string url, object content)
        {
            var sontent = JsonConvert.SerializeObject(content);
            Log.Ver($"{nameof(url)}: POST {url}");
            Log.Ver($"{nameof(content)}: {sontent}");
            HttpResponseMessage response = await StaticClient.PostAsync(url, new StringContent(sontent, Encoding.UTF8, "application/json"));
            string rontent = await response.Content.ReadAsStringAsync();
            ExceptionCreator.ThrowMaybe(response.StatusCode, rontent);
            return JsonConvert.DeserializeObject<TResult>(rontent);
        }

        internal static async Task Put(string url, object content)
        {
            var sontent = JsonConvert.SerializeObject(content);
            Log.Ver($"{nameof(url)}: PUT {url}");
            Log.Ver($"{nameof(content)}: {sontent}");
            HttpResponseMessage response = await StaticClient.PutAsync(url, new StringContent(sontent, Encoding.UTF8, "application/json"));
            string rontent = await response.Content.ReadAsStringAsync();
            ExceptionCreator.ThrowMaybe(response.StatusCode, rontent);
        }

        internal static async Task<TResult> Put<TResult>(string url, object content)
        {
            var sontent = JsonConvert.SerializeObject(content);
            Log.Ver($"{nameof(url)}: PUT {url}");
            Log.Ver($"{nameof(content)}: {sontent}");
            HttpResponseMessage response = await StaticClient.PutAsync(url, new StringContent(sontent, Encoding.UTF8, "application/json"));
            string rontent = await response.Content.ReadAsStringAsync();
            ExceptionCreator.ThrowMaybe(response.StatusCode, rontent);
            return JsonConvert.DeserializeObject<TResult>(rontent);
        }

        internal static async Task Delete(string url, object content)
        {
            var sontent = JsonConvert.SerializeObject(content);
            Log.Ver($"{nameof(url)}: DELETE {url}");
            Log.Ver($"{nameof(content)}: {sontent}");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, url)
            {
                Content = new StringContent(sontent, Encoding.UTF8, "application/json")
            };
            HttpResponseMessage response = await StaticClient.SendAsync(request);
            string rontent = await response.Content.ReadAsStringAsync();
            ExceptionCreator.ThrowMaybe(response.StatusCode, rontent);
        }

        internal static async Task<TResult> Delete<TResult>(string url)
        {
            Log.Ver($"{nameof(url)}: DELETE {url}");
            HttpResponseMessage response = await StaticClient.DeleteAsync(url);
            string rontent = await response.Content.ReadAsStringAsync();
            ExceptionCreator.ThrowMaybe(response.StatusCode, rontent);
            return JsonConvert.DeserializeObject<TResult>(rontent);
        }

        internal static async Task<TResult> Delete<TResult>(string url, object content)
        {
            var sontent = JsonConvert.SerializeObject(content);
            Log.Ver($"{nameof(url)}: DELETE {url}");
            Log.Ver($"{nameof(content)}: {sontent}");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, url)
            {
                Content = new StringContent(sontent, Encoding.UTF8, "application/json")
            };
            HttpResponseMessage response = await StaticClient.SendAsync(request);
            string rontent = await response.Content.ReadAsStringAsync();
            ExceptionCreator.ThrowMaybe(response.StatusCode, rontent);
            return JsonConvert.DeserializeObject<TResult>(rontent);
        }

        internal static async Task<TResult> Get<TResult>(string url)
        {
            Log.Ver($"{nameof(url)}: GET {url}");
            HttpResponseMessage response = await StaticClient.GetAsync(url);
            string rontent = await response.Content.ReadAsStringAsync();
            ExceptionCreator.ThrowMaybe(response.StatusCode, rontent);
            return JsonConvert.DeserializeObject<TResult>(rontent);
        }
    }

    internal sealed class HttpCommunicator : IDisposable
    {
        private HttpClient Client = new HttpClient();

        internal HttpCommunicator(string token)
        {
            this.Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Token", token);
        }

        internal async Task<TResult> Post<TResult>(string url, object content)
        {
            var sontent = JsonConvert.SerializeObject(content);
            Log.Ver($"{nameof(url)}: POST {url}");
            Log.Ver($"{nameof(content)}: {sontent}");
            HttpResponseMessage response = await this.Client.PostAsync(url, new StringContent(sontent, Encoding.UTF8, "application/json"));
            string rontent = await response.Content.ReadAsStringAsync();
            ExceptionCreator.ThrowMaybe(response.StatusCode, rontent);
            return JsonConvert.DeserializeObject<TResult>(rontent);
        }

        internal async Task<TResult> Patch<TResult>(string url)
        {
            Log.Ver($"{nameof(url)}: PATCH {url}");
            HttpMethod method = new HttpMethod("PATCH");
            HttpRequestMessage request = new HttpRequestMessage(method, url);
            HttpResponseMessage response = await this.Client.SendAsync(request);
            string rontent = await response.Content.ReadAsStringAsync();
            ExceptionCreator.ThrowMaybe(response.StatusCode, rontent);
            return JsonConvert.DeserializeObject<TResult>(rontent);
        }

        internal async Task<TResult> Patch<TResult>(string url, object content)
        {
            var sontent = JsonConvert.SerializeObject(content);
            Log.Ver($"{nameof(url)}: PATCH {url}");
            Log.Ver($"{nameof(content)}: {sontent}");
            HttpMethod method = new HttpMethod("PATCH");
            HttpRequestMessage request = new HttpRequestMessage(method, url)
            {
                Content = new StringContent(sontent, Encoding.UTF8, "application/json")
            };
            HttpResponseMessage response = await this.Client.SendAsync(request);
            string rontent = await response.Content.ReadAsStringAsync();
            ExceptionCreator.ThrowMaybe(response.StatusCode, rontent);
            return JsonConvert.DeserializeObject<TResult>(rontent);
        }

        internal async Task<TResult> Put<TResult>(string url, object content)
        {
            var sontent = JsonConvert.SerializeObject(content);
            Log.Ver($"{nameof(url)}: PUT {url}");
            Log.Ver($"{nameof(content)}: {sontent}");
            HttpResponseMessage response = await this.Client.PutAsync(url, new StringContent(sontent, Encoding.UTF8, "application/json"));
            string rontent = await response.Content.ReadAsStringAsync();
            ExceptionCreator.ThrowMaybe(response.StatusCode, rontent);
            return JsonConvert.DeserializeObject<TResult>(rontent);
        }

        internal async Task Delete(string url, object content)
        {
            var sontent = JsonConvert.SerializeObject(content);
            Log.Ver($"{nameof(url)}: DELETE {url}");
            Log.Ver($"{nameof(content)}: {sontent}");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, url)
            {
                Content = new StringContent(sontent, Encoding.UTF8, "application/json")
            };
            HttpResponseMessage response = await this.Client.SendAsync(request);
            string rontent = await response.Content.ReadAsStringAsync();
            ExceptionCreator.ThrowMaybe(response.StatusCode, rontent);
        }

        internal async Task<TResult> Delete<TResult>(string url, object content)
        {
            var sontent = JsonConvert.SerializeObject(content);
            Log.Ver($"{nameof(url)}: DELETE {url}");
            Log.Ver($"{nameof(content)}: {sontent}");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, url)
            {
                Content = new StringContent(sontent, Encoding.UTF8, "application/json")
            };
            HttpResponseMessage response = await this.Client.SendAsync(request);
            string rontent = await response.Content.ReadAsStringAsync();
            ExceptionCreator.ThrowMaybe(response.StatusCode, rontent);
            return JsonConvert.DeserializeObject<TResult>(rontent);
        }

        internal async Task<TResult> Delete<TResult>(string url)
        {
            Log.Ver($"{nameof(url)}: DELETE {url}");
            HttpResponseMessage response = await this.Client.DeleteAsync(url);
            string rontent = await response.Content.ReadAsStringAsync();
            ExceptionCreator.ThrowMaybe(response.StatusCode, rontent);
            return JsonConvert.DeserializeObject<TResult>(rontent);
        }

        internal async Task<TResult> Get<TResult>(string url)
        {
            Log.Ver($"{nameof(url)}: GET {url}");
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
