namespace Unlimitedinf.Apis.Server.IntTests
{
    public static class C
    {
        /// <summary>
        /// URLs
        /// </summary>
        public static class U
        {
            private static string BaseUrl => S.I.BaseUri;

            private static readonly string AuBase = BaseUrl + "/auth";
            public static readonly string AuAccount = AuBase + "/accounts";
            public static readonly string AuToken = AuBase + "/tokens";

            public static readonly string Axiom = BaseUrl + "/axioms";

            public static readonly string DateTime = BaseUrl + "/datetime";

            public static readonly string Ping = BaseUrl + "/ping";

            public static readonly string Random = BaseUrl + "/random";

            public static readonly string Teapot = BaseUrl + "/teapot";

            private static readonly string VeBase = BaseUrl + "/versioning";
            public static readonly string VeCount = VeBase + "/counts";
            public static readonly string VeVersion = VeBase + "/versions";
        }
    }
}
