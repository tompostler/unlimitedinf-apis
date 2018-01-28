using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Unlimitedinf.Apis.Client.Program.Auth;
using Unlimitedinf.Apis.Client.Program.Versioning;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Client.Program
{
    public static class App
    {
        public static int Main(string[] args)
        {
            Log.PrintVerbosityLevel = false;

#if DEBUG
            Log.PrintVerbosityLevel = true;
            Log.Verbosity = Log.VerbositySetting.Verbose;
            if (args.Length == 0)
            {
                var lst = new List<string>();
                Log.Inf("Args, one per line:");
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

            // If we're running a new version, we need to see if there are existing settings from an older version of the app
            if (Properties.Settings.Default.UpgradeRequired)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }

            try
            {
                var rargs = new string[args.Length - 1];
                Array.Copy(args, 1, rargs, 0, args.Length - 1);
                switch (args[0])
                {
                    case "account":
                        return AccountProgram.Run(rargs);
                    case "token":
                        return TokenProgram.Run(rargs);
                    case "axiom":
                        return AxBase.Run(rargs);
                    case "catan":
                        return CatanProgram.Run(rargs);
                    case "repo":
                        return RepoProgram.Run(rargs);
                    case "version":
                        return VersionProgram.Run(rargs);
                    case "count":
                        return CountProgram.Run(rargs);

                    default:
                        return PrintHelp();
                }
            }
            catch (ApiException e)
            {
                Log.Err($"{e.GetType().Name}! Code {(int)e.StatusCode}.");
                return (int)e.StatusCode;
            }
            catch (Exception e)
            {
                Log.Err("Other exception hit! Message:");
                Log.Inf(e.Message);
                Log.Ver(JsonConvert.SerializeObject(e, Formatting.Indented)
                    .Replace("\\r", "\r")
                    .Replace("\\n", "\n")
                    .Replace("\\\\", "\\"));
                return ExitCode.GenericError;
            }
        }

        internal static int PrintHelp()
        {
            Log.Inf($@"
Unlimitedinf.Apis.Client.exe v{FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion}

An executable tool to work with the endpoints at the API app hosted at:
    https://unlimitedinf-apis.azurewebsites.net

Usage is somewhat similar to that of git. I.e. <exec> <command> <parameters>
The following are valid commands:

    help        This help text.
    help error  This list of exit codes and what they mean.

    account     The module dealing with account interactions.
        create  Create a new account. Prompts for additional information.
        read u  Will read additional information for an account whose username
                is 'u'
        update  Update an existing account. Prompts for additional information.
        delete  Deletes an existing account. Prompts for additional information.

    token       The module dealing with token interactions.
        create  Create a new token. Prompts for additional information.
        delete  Deletes an existing token. Prompts for additional information.
        save t  Saves/Overwrites the token in user settings.

    axiom       The module dealing with axiom interactions.
                Axiom is special. Merely provide the type and id, and receive
                the axiom. Example:
                    Unlimitedinf.Apis.Client.exe axiom http 403

    message     The module dealing with message interactions.
        create  Create a new message. Prompts for additional information.
        read    Read all unread messages for the current user.
        reada   Read all messages for the current user.
        mread m Will mark the message 'm' as read.
        delete m* Will delete as many messages as you supply 'm' space-delimited
                ids for. If one message id is given, will show the deleted
                message. If multiple message ids are given, will only show the
                status for each deleted message.

    catan       The module dealing with recording games of catan.
        create+ Create a new catan game, printing stats as you go.
        read u g Will read additional information for the catan game 'g' that
                belongs to the user with username 'u'.
        reads u g Will read a statistical printout for the catan game 'g' that
                belongs to the user with username 'u'.
        delete g Deletes an existing catan game by name.

    repo        The module dealing with repo interactions.
        create  Create a new repo. Prompts for additional information.
        read    Read all the repos for the current user.
        readps  Get a PS script to clone all of your repos.
        delete r Deletes an existing repo by name.

    version     The module dealing with version interactions.
        create  Create a new version. Prompts for additional information.
        read u  Will read additional information for all versions that belong to
                the user with username 'u'
        read u v Will read additional information for the version with name 'v'
                belonging to the user with username 'u'
        update  Update an existing version. Prompts for additional information.
        delete  Deletes an existing version. Prompts for additional information.

    count       The module dealing with count interactions. Usage is exactly the
                same as the version module.

Note: In any of the commands that mention prompting for additional information,
      a serialized version of the information being prompted for will short
      circuit the prompts and cause immediate execution.
");
            return ExitCode.HelpText;
        }

        private static int PrintHelpError()
        {
            Log.Inf($@"
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
