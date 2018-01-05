namespace Unlimitedinf.Apis.Server.Tests
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

            public static readonly string Ping = BaseUrl + "/ping";

            public static readonly string Teapot = BaseUrl + "/teapot";
        }
    }
}
