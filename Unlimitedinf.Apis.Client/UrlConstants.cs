namespace Unlimitedinf.Apis.Client
{
    internal static class Curl
    {
        private const string BaseUrl = "https://unlimitedinf-apis.azurewebsites.net";

        private const string AuBase = BaseUrl + "/auth";
        internal const string AuAccount = AuBase + "/accounts";
        internal const string AuToken = AuBase + "/tokens";

        private const string VBase = BaseUrl + "/versioning";
        internal const string VVersion = VBase + "/versions";
        internal const string VCount = VBase + "/counts";

        internal const string AxBase = BaseUrl + "/axioms";

        internal const string Catan = BaseUrl + "/catans";

        internal const string Message = BaseUrl + "/messages";

        internal const string Repo = BaseUrl + "/repos";
        internal const string RepoPsScript = Repo + "/ps-script";
    }
}
