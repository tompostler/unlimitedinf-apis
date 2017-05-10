using CA = Unlimitedinf.Apis.Contracts.Auth;

namespace Unlimitedinf.Apis.Client
{
    public sealed partial class ApiClient
    {
        public sealed class Versioning
        {
            private string Token { get; set; }

            internal Versioning(string token)
            {
                this.Token = token;
            }
        }
    }
}
