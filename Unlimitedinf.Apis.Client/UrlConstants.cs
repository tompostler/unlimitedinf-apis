namespace Unlimitedinf.Apis.Client
{
    internal static class Curl
    {
        private const string BaseUrl = "https://unlimitedinf-apis.azurewebsites.net";

        private const string AuBase = BaseUrl + "/auth";
        public const string AuAccount = AuBase + "/accounts";
        public const string AuToken = AuBase + "/tokens";

        public const string Axiom = BaseUrl + "/axioms";

        public const string Catan = BaseUrl + "/catans";

        public const string DateTime = BaseUrl + "/datetime";

        public const string Message = BaseUrl + "/messages";

        public const string Ping = BaseUrl + "/ping";

        public const string Random = BaseUrl + "/random";

        public const string Repo = BaseUrl + "/repos";
        public const string RepoPsScript = Repo + "/ps-script";

        public const string Teapot = BaseUrl + "/teapot";

        private const string VeBase = BaseUrl + "/versioning";
        public const string VeCount = VeBase + "/counts";
        public const string VeVersion = VeBase + "/versions";
    }
}
