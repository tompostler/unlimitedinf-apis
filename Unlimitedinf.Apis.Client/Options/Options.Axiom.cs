using Mono.Options;
using Newtonsoft.Json;
using System;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Client.Options
{
    internal static partial class Options
    {
        public static AxiomOptions ParseAxiom(string[] args)
        {
            var config = new AxiomOptions();
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
                    "http=",
                    "Get a specific HTTP code axiom.",
                    v => config.Http = int.Parse(v)
                },
                {
                    "t|type=",
                    "The generic type of axiom to retrieve.",
                    v => config.Type = v
                },
                {
                    "i|id=",
                    "The generic id for a type of axiom to retrieve.",
                    v => config.Id = v
                }
            };

            optset.Parse(args);
            Log.Ver("OPTIONS:");
            Log.Ver(config.ToString());
            Log.Line();

            var emptyType = string.IsNullOrWhiteSpace(config.Type);
            var emptyId = string.IsNullOrWhiteSpace(config.Id);
            if (config.Http == null && emptyType && emptyId)
                config.Help = true;

            if ((emptyType && !emptyId) || (!emptyType && emptyId))
            {
                Log.Err("Must specify both type and id.");
                config.Help = true;
            }

            if (config.Help)
            {
                Log.Inf(string.Format(Options.OptionsBaseHelpText, "axiom", AxiomHelpText));
                optset.WriteOptionDescriptions(Console.Out);
                U.Exit();
            }

            return config;
        }

        private const string AxiomHelpText = @"
Because truths are self-evident, these axioms can change as specificactions
evolve. Always hit the APIs to make sure the latest version is acquired.
";
        //1234567890123456789012345678901234567890123456789012345678901234567890

        internal class AxiomOptions : ModuleOptions
        {
            public int? Http { get; set; } = null;
            public string Type { get; set; }
            public string Id { get; set; }
        }
    }
}
