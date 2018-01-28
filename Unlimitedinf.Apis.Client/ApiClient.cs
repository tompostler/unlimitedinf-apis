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

        /// <summary>
        /// Message client.
        /// </summary>
        public ApiClient_Messages Messages { get; set; }
        /// <summary>
        /// Catan client.
        /// </summary>
        public ApiClient_Catans Catans { get; }
        /// <summary>
        /// Repo client.
        /// </summary>
        public ApiClient_Repos Repos { get; }
        /// <summary>
        /// Versioning client.
        /// </summary>
        public ApiClient_Versioning Versioning { get; }
    }
}
