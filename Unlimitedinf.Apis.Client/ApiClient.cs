using System;
using CA = Unlimitedinf.Apis.Contracts.Auth;

namespace Unlimitedinf.Apis.Client
{
    /// <summary>
    /// The main client for the APIs.
    /// </summary>
    public sealed partial class ApiClient
    {
        private string Token { get; set; }

        /// <summary>
        /// Ctor.
        /// </summary>
        public ApiClient(string token)
        {
            if (CA.Token.IsTokenExpired(token))
                throw new ArgumentException("Token is expired.", nameof(token));
            this.Token = token;

            this.Version = new Versioning(this.Token);
        }

        public Versioning Version { get; }
    }
}
