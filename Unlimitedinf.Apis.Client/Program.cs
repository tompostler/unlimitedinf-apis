namespace Unlimitedinf.Apis.Client
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            return ExitCode.Success;
        }

        private static class ExitCode
        {
            public const int Success = 0;
            public const int GenericError = 1;
        }
    }
}
