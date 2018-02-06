using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Unlimitedinf.Apis.Client.Options
{
    internal static partial class Options
    {
        public static bool Verbose { get; set; }
        public static Module Module { get; set; } = Module.Help;

        public static (Module, ModuleOptions) Parse(string[] args)
        {
            if (args == null || args.Length == 0 || new HashSet<string> { "--help", "-h", "help", "/?", "/help", "/h" }.Contains(args[0].ToLowerInvariant()))
                return (Module.Help, null);

            var rargs = new string[args.Length - 1];
            Array.Copy(args, 1, rargs, 0, args.Length - 1);

            switch (args[0])
            {
                case "config":
                    return (Module.Config, ParseConfig(rargs));
                case "auth":
                    return (Module.Auth, ParseAuth(rargs));
                case "repo":
                    return (Module.Repo, ParseRepo(rargs));
                default:
                    throw new TomIsLazyException();
            }
        }

        public static readonly string BaseHelpText = $@"Unlimitedinf.Apis.Client.exe v{FileVersionInfo.GetVersionInfo(typeof(Options).Assembly.Location).FileVersion}

Usage: Unlimitedinf.Apis.Client.exe MODULE [OPTIONS]*

A console application to communicate with the APIs. The executable is broken up
by various modules, of which each may be handled differently. Specifying
multiple logical operations per module is unsupported and unguarded. Whenever an
input is followed by a '?', then it is optional. Good luck and have fun!

MODULES:
    help        Prints this helptext. For detailed help on a specific module,
                select that module and pass the '--help' option.
    config      Enables saving of some settings between executions of this
                program. Currently can save/delete a username and token.
    auth        Create, read, update, or delete accounts and tokens.
    repo        Create, read, delete, and generate a script from a list of
                repositories. Secured per user.
";
//345678901234567890123456789012345678901234567890123456789012345678901234567890
        public static readonly string OptionsBaseHelpText = $@"Unlimitedinf.Apis.Client.exe v{FileVersionInfo.GetVersionInfo(typeof(Options).Assembly.Location).FileVersion}

Usage: Unlimitedinf.Apis.Client.exe {{0}} [OPTIONS]*

A console application to communicate with the APIs. The executable is broken up
by various modules, of which each may be handled differently. Below are the
options available for the {{0}} module.
{{1}}
OPTIONS:";

        public abstract class ModuleOptions
        {
            public bool Help { get; set; }
        }
    }
}
