using System.Collections.Generic;
using System.Threading.Tasks;
using CV = Unlimitedinf.Apis.Contracts.Versioning;

namespace Unlimitedinf.Apis.Client
{
    /// <summary>
    /// Versioning client.
    /// </summary>
    public sealed class ApiClient_Versioning
    {
        private HttpCommunicator Communicator { get; set; }

        private ApiClient_Versioning() { }

        internal ApiClient_Versioning(HttpCommunicator communicator)
        {
            this.Communicator = communicator;
        }



        /// <summary>
        /// Create a version.
        /// </summary>
        public async Task<CV.Version> VersionCreate(CV.Version version)
        {
            return await this.Communicator.Post<CV.Version, CV.Version>(Curl.VVersion, version);
        }

        /// <summary>
        /// Read versions for a user.
        /// </summary>
        public static async Task<List<CV.Version>> VersionRead(string username)
        {
            return await StaticHttpCommunicator.Get<List<CV.Version>>($"{Curl.VVersion}/{username}");
        }
        /// <summary>
        /// Read a version.
        /// </summary>
        public static async Task<CV.Version> VersionRead(string username, string versionName)
        {
            return await StaticHttpCommunicator.Get<CV.Version>($"{Curl.VVersion}/{username}/{versionName}");
        }

        /// <summary>
        /// Update a version.
        /// </summary>
        public async Task<CV.Version> VersionUpdate(CV.VersionIncrement version)
        {
            return await this.Communicator.Put<CV.VersionIncrement, CV.Version>(Curl.VVersion, version);
        }

        /// <summary>
        /// Delete a version.
        /// </summary>
        public async Task<CV.Version> VersionDelete(string username, string versionName)
        {
            return await this.Communicator.Delete<CV.Version>($"{Curl.VVersion}/{username}/{versionName}");
        }



        /// <summary>
        /// Create a count.
        /// </summary>
        public async Task<CV.Count> CountCreate(CV.Count count)
        {
            return await this.Communicator.Post<CV.Count, CV.Count>(Curl.VCount, count);
        }

        /// <summary>
        /// Read counts for a user.
        /// </summary>
        public static async Task<List<CV.Count>> CountRead(string username)
        {
            return await StaticHttpCommunicator.Get<List<CV.Count>>($"{Curl.VCount}/{username}");
        }
        /// <summary>
        /// Read a count.
        /// </summary>
        public static async Task<CV.Count> CountRead(string username, string countName)
        {
            return await StaticHttpCommunicator.Get<CV.Count>($"{Curl.VCount}/{username}/{countName}");
        }

        /// <summary>
        /// Update a count.
        /// </summary>
        public async Task<CV.Count> CountUpdate(CV.CountChange countChange)
        {
            return await this.Communicator.Put<CV.CountChange, CV.Count>(Curl.VCount, countChange);
        }

        /// <summary>
        /// Delete a count.
        /// </summary>
        public async Task<CV.Count> CountDelete(string username, string countName)
        {
            return await this.Communicator.Delete<CV.Count>($"{Curl.VCount}/{username}/{countName}");
        }
    }
}
