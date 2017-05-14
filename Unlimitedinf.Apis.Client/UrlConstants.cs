namespace Unlimitedinf.Apis.Client
{
    internal static class Curl
    {
        private const string BaseUrl = "https://unlimitedinf-apis.azurewebsites.net";

        private const string ABase = BaseUrl + "/auth";
        internal const string AAccount = ABase + "/accounts";
        internal const string AToken = ABase + "/tokens";

        private const string VBase = BaseUrl + "/versioning";
        internal const string VVersion = VBase + "/versions";
    }
}
