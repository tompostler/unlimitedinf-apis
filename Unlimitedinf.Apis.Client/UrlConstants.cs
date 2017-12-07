namespace Unlimitedinf.Apis.Client
{
    internal static class Curl
    {
        private const string BaseUrl = "https://unlimitedinf-apis.azurewebsites.net";

        private const string AuBase = BaseUrl + "/auth";
        internal const string AuAccount = AuBase + "/accounts";
        internal const string AuToken = AuBase + "/tokens";

        internal const string AxBase = BaseUrl + "/axioms";

        private const string VBase = BaseUrl + "/versioning";
        internal const string VVersion = VBase + "/versions";
        internal const string VCount = VBase + "/counts";

        private const string NBase = BaseUrl + "/notes";
        internal const string NRepo = NBase + "/repos";
        internal const string NRepoPsScript = NRepo + "/ps-script";
    }
}
