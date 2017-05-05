using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace Unlimitedinf.Apis.Client.Program
{
    public static class App
    {
        public static int Main(string[] args)
        {
#if DEBUG
            if (args.Length == 0)
            {
                var lst = new List<string>();
                Console.WriteLine("Args, one per line:");
                var arg = Console.ReadLine();
                while (!string.IsNullOrWhiteSpace(arg))
                {
                    lst.Add(arg);
                    arg = Console.ReadLine();
                }
                args = lst.ToArray();
            }
#endif

            if (args.Length == 0 || args.Contains("-?") || args.Contains("--?") || args.Contains("-h") || args.Contains("--help") || args.Contains("help"))
                if (args.Length == 2 && args[0] == "help" && args[1] == "error")
                    return PrintHelpError();
                else
                    return PrintHelp();

            try
            {
                var rargs = new string[args.Length - 1];
                Array.Copy(args, 1, rargs, 0, args.Length - 1);
                switch (args[0])
                {
                    case "account":
                        return AuthAccount.Run(rargs);

                    case "token":
                        return AuthToken.Run(rargs);

                    default:
                        return PrintHelp();
                }
            }
            catch (ApiException e)
            {
                Console.Error.WriteLine($"{e.GetType().Name}! Code {(int)e.StatusCode}.");
                return (int)e.StatusCode;
            }
        }

        internal static int PrintHelp()
        {
            Console.WriteLine($@"
Unlimitedinf.Apis.Client.exe v{FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion}

An executable tool to work with the endpoints at the API app hosted at:
    https://unlimitedinf-apis.azurewebsites.net

Usage is somewhat similar to that of git. I.e. <exec> <command> <parameters>
The following are valid commands:

    help        This help text.
    help error  This list of exit codes and what they mean.

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
            return ExitCode.HelpText;
        }

        private static int PrintHelpError()
        {
            Console.WriteLine($@"
Unlimitedinf.Apis.Client.exe v{FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion}

Error/exit codes and what they mean:

    0           Success.
    1           A general error happened.
    2           Help text is displayed.
    3           Functionality not yet implemented.
    4           Provided values failed validation.
    400-500     An ApiException was thrown and this is the status code.
");

            return ExitCode.HelpText;
        }
    }

    public static class ExitCode
    {
        public const int Success = 0;
        public const int GenericError = 1;
        public const int HelpText = 2;
        public const int NotImplemented = 3;
        public const int ValidationFailed = 4;
    }
}
