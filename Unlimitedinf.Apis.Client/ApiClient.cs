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

            this.Messaging = new ApiClient_Messaging(this.Token, this.Communicator);
            this.Notes = new ApiClient_Notes(this.Token, this.Communicator);
            this.Versioning = new ApiClient_Versioning(this.Token, this.Communicator);
        }

        public ApiClient_Messaging Messaging { get; set; }
        public ApiClient_Notes Notes { get; }
        public ApiClient_Versioning Versioning { get; }
    }
}
