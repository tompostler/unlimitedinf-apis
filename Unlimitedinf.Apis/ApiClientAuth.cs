using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using CA = Unlimitedinf.Apis.Contracts.Auth;

namespace Unlimitedinf.Apis
{
    /// <summary>
    /// A helper class to handle the authentication CRUD necessary for the ApiClient.
    /// </summary>
    public static class ApiClientAuth
    {
        private const string BaseUrl = "https://unlimitedinf-apis.azurewebsites.net";

        private const string AuthBase = BaseUrl + "/auth";
        private const string AuthAccount = AuthBase + "/accounts";
        private const string AuthToken = AuthBase + "/tokens";

        /// <summary>
        /// Account-related operations.
        /// </summary>
        public static class Account
        {
            /// <summary>
            /// Method to create an account.
            /// </summary>
            public static async Task<CA.Account> Create(CA.Account account)
            {
                HttpResponseMessage response;
                using (var httpClient = new HttpClient())
                {
                    response = await httpClient.PostAsync(AuthAccount, new StringContent(JsonConvert.SerializeObject(account)));
                }
                string content = await response.Content.ReadAsStringAsync();
                ExceptionCreator.ThrowMaybe(response.StatusCode, content);

                return JsonConvert.DeserializeObject<CA.Account>(content);
            }

            /// <summary>
            /// Method to get an account's information.
            /// </summary>
            public static async Task<CA.Account> Read(string username)
            {
                HttpResponseMessage response;
                using (var httpClient = new HttpClient())
                {
                    response = await httpClient.GetAsync($"{AuthAccount}?username={username}");
                }
                string content = await response.Content.ReadAsStringAsync();
                ExceptionCreator.ThrowMaybe(response.StatusCode, content);

                return JsonConvert.DeserializeObject<CA.Account>(content);
            }
        }

        /// <summary>
        /// Generate a token to be used with the constructor of this class.
        /// </summary>
        /// <param name="creation">The parameters for creating a token.</param>
        public static async Task<CA.Token> CreateToken(CA.TokenCreate creation)
        {
            HttpResponseMessage response;
            using (var httpClient = new HttpClient())
            {
                response = await httpClient.PostAsync(AuthToken, new StringContent(JsonConvert.SerializeObject(creation)));
            }
            string content = await response.Content.ReadAsStringAsync();
            ExceptionCreator.ThrowMaybe(response.StatusCode, content);

            return JsonConvert.DeserializeObject<CA.Token>(content);
        }

        /// <summary>
        /// Resurface the ability to check if a token is expired based on the token value.
        /// </summary>
        public static bool IsTokenExpired(string token)
        {
            return CA.Token.IsTokenExpired(token);
        }
    }
}
