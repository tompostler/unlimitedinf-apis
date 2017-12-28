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

            this.Messages = new ApiClient_Messages(this.Communicator);
            this.Catans = new ApiClient_Catans(this.Communicator);
            this.Repos = new ApiClient_Repos(this.Communicator);
            this.Versioning = new ApiClient_Versioning(this.Communicator);
        }

        public ApiClient_Messages Messages { get; set; }
        public ApiClient_Catans Catans { get; }
        public ApiClient_Repos Repos { get; }
        public ApiClient_Versioning Versioning { get; }
    }
}
