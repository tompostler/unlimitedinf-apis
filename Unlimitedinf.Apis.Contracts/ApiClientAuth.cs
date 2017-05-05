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
                return await HttpCommunicator.Post<CA.Account, CA.Account>(AuthAccount, account);
            }

            /// <summary>
            /// Method to get an account's information.
            /// </summary>
            public static async Task<CA.Account> Read(string username)
            {
                return await HttpCommunicator.Get<CA.Account>($"{AuthAccount}?username={username}");
            }

            /// <summary>
            /// Method to update an account's information.
            /// Because Azure's PUT operations return a 204, this will throw on error else succeed.
            /// </summary>
            public static async Task Update(CA.AccountUpdate account)
            {
                await HttpCommunicator.Put(AuthAccount, account);
            }

            /// <summary>
            /// Method to delete an account.
            /// Because Azure's DELETE operations return a 204, this will throw on error else succeed.
            /// </summary>
            public static async Task Delete(CA.Account account)
            {
                await HttpCommunicator.Delete(AuthAccount, account);
            }
        }

        /// <summary>
        /// Token-related operations.
        /// </summary>
        public static class Token
        {
            /// <summary>
            /// Method to create a token.
            /// </summary>
            public static async Task<CA.Token> Create(CA.TokenCreate creation)
            {
                return await HttpCommunicator.Post<CA.TokenCreate, CA.Token>(AuthToken, creation);
            }

            /// <summary>
            /// Method to delete a token.
            /// Because Azure's DELETE operations return a 204, this will throw on error else succeed.
            /// </summary>
            public static async Task Delete(CA.TokenDelete deletion)
            {
                await HttpCommunicator.Delete(AuthToken, deletion);
            }
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
