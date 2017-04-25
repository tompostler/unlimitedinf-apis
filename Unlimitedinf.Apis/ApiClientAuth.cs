using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Unlimitedinf.Apis.Contracts.Auth;

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
        /// Method to create an account.
        /// </summary>
        /// <param name="account">The account information.</param>
        public static async Task<Account> CreateAccount(Account account)
        {
            HttpResponseMessage response;
            using (var httpClient = new HttpClient())
            {
                response = await httpClient.PostAsync(AuthAccount, new StringContent(JsonConvert.SerializeObject(account)));
            }
            ExceptionCreator.ThrowMaybe(response.StatusCode, await response.Content.ReadAsStringAsync());

            return JsonConvert.DeserializeObject<Account>(await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Generate a token to be used with the constructor of this class.
        /// </summary>
        /// <param name="creation">The parameters for creating a token.</param>
        public static async Task<Token> CreateToken(TokenCreate creation)
        {
            HttpResponseMessage response;
            using (var httpClient = new HttpClient())
            {
                response = await httpClient.PostAsync(AuthToken, new StringContent(JsonConvert.SerializeObject(creation)));
            }
            ExceptionCreator.ThrowMaybe(response.StatusCode, await response.Content.ReadAsStringAsync());

            return JsonConvert.DeserializeObject<Token>(await response.Content.ReadAsStringAsync());
        }
    }
}
