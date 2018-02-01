using Mono.Options;
using Newtonsoft.Json;
using System;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Client.Options
{
    internal static partial class Options
    {
        public static ConfigOptions ParseConfig(string[] args)
        {
            var config = new ConfigOptions();
            if (args.Length == 0)
                config.Help = true;

            var optset = new OptionSet
            {
                {
                    "h|help",
                    "Print this helptext and exits.",
                    v => config.Help = true
                },
                {
                    "s|show",
                    "Shows the existing saved configuration.",
                    v => config.Show = true
                },
                {
                    "c|clear",
                    "Clears the existing saved configuration.",
                    v => config.Clear = true
                },
                {
                    "u|username=",
                    "Sets a username in the configuration.",
                    v => config.Username = v
                },
                {
                    "t|token=",
                    "Sets a token in the configuration.",
                    v => config.Token = v
                }
            };

            optset.Parse(args);
            Log.Ver("OPTIONS:");
            Log.Ver(config.ToString());
            Log.Line();

            if (config.Help)
            {
                Log.Inf(string.Format(Options.OptionsBaseHelpText, "config"));
                optset.WriteOptionDescriptions(Console.Out);
                U.Exit();
            }

            return config;
        }

        internal class ConfigOptions : ModuleOptions
        {
            public bool Show { get; set; }
            public bool Clear { get; set; }
            public string Username { get; set; }
            public string Token { get; set; }

            public override string ToString()
            {
                return JsonConvert.SerializeObject(this, Formatting.Indented);
            }
        }
    }
}
