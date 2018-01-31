using System.Collections.Generic;
using System.Diagnostics;

namespace Unlimitedinf.Apis.Client.Options
{
    internal static class Options
    {
        public static bool Verbose { get; set; }
        public static Module Module { get; set; } = Module.Help;

        public static Module Parse(string[] args)
        {
            if (args == null || args.Length == 0 || new HashSet<string> { "--help", "-h", "help", "/?", "/help", "/h" }.Contains(args[0].ToLowerInvariant()))
                return Module;

            return Module;
        }

        public static readonly string BaseHelpText = $@"
Unlimitedinf.Apis.Client.exe v{FileVersionInfo.GetVersionInfo(typeof(Options).Assembly.Location).FileVersion}
";
//345678901234567890123456789012345678901234567890123456789012345678901234567890
    }
}
