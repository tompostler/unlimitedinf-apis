using System;
using System.Linq;

namespace Unlimitedinf.Apis.Client
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length == 0 || args.Contains("-?") || args.Contains("--?") || args.Contains("-h") || args.Contains("--help") || args.Contains("help"))
                PrintHelpAndExit();
            PrintHelpAndExit();
            return ExitCode.Success;
        }

        private static void PrintHelpAndExit()
        {
            Console.WriteLine($@"
Unlimitedinf.Apis.Client.exe v{typeof(Program).Assembly.GetName().Version}

An executable tool to work with the endpoints at the API app hosted at:
    https://unlimitedinf-apis.azurewebsites.net

Usage is somewhat similar to that of git. I.e. <exec> <command> <parameters>
The following are valid commands:

    help        This help text.

    account     The module dealing with account interactions.
        create  Create a new account. Prompts for additional information.
        read u  Will read additional information for an account whose username is
                'u'
        update  Update an existing account. Prompts for additional information.
        delete  Deletes an existing account. Prompts for additional information.

    token       The module dealing with token interactions.
        create  Create a new token. Prompts for additional information.
        delete  Deletes an existing token. Prompts for additional information.

Note: In any of the commands that mention prompting for additional information,
      a serialized version of the information being prompted for will short
      circuit the prompts and cause immediate execution.
");
            Environment.Exit(ExitCode.HelpText);
        }

        private static class ExitCode
        {
            public const int Success = 0;
            public const int GenericError = 1;
            public const int HelpText = 2;
        }
    }
}
