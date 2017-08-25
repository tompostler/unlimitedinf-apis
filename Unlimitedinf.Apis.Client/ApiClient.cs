using System;
using CA = Unlimitedinf.Apis.Contracts.Auth;

namespace Unlimitedinf.Apis.Client
{
    /// <summary>
    /// The main client for the APIs.
    /// </summary>
    public sealed class ApiClient
    {
        private string Token { get; set; }
        private HttpCommunicator Communicator { get; set; }

        /// <summary>
        /// Ctor.
        /// </summary>
        public ApiClient(string token)
        {
            if (CA.Token.IsTokenExpired(token))
                throw new ArgumentException("Token is expired.", nameof(token));
            this.Token = token;
            this.Communicator = new HttpCommunicator(this.Token);

            this.Versioning = new ApiClient_Versioning(this.Token, this.Communicator);
        }

        public ApiClient_Versioning Versioning { get; }
    }
}
