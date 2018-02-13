using Mono.Options;
using Newtonsoft.Json;
using System;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Client.Options
{
    internal static partial class Options
    {
        public static CatanOptions ParseCatan(string[] args)
        {
            var config = new CatanOptions();
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
                    "create-plus",
                    "Begins the very interactive process of creating a Catan game.",
                    v => config.Catan = CRUD.Create
                },
                {
                    "r|read-catan=",
                    "Fetch information about an exisitng Catan game, by a forward-slash-separated username/catanName pair. Alternatively, just specify the username to list all the Catan games for that user.",
                    v => config.ReadCatan = v
                },
                {
                    "d|delete-catan",
                    "Begins the interactive process of deleting a Catan game.",
                    v => config.Catan = CRUD.Delete
                },
                {
                    "s|read-catan-stats=",
                    "Fetch the nicely-formatted stats about an existing Catan game, by a forward-slash-separated username/catanName pair.",
                    v => config.ReadCatanStats = v
                }
            };

            optset.Parse(args);
            Log.Ver("OPTIONS:");
            Log.Ver(config.ToString());
            Log.Line();

            if (config.Help)
            {
                Log.Inf(string.Format(Options.OptionsBaseHelpText, "catan", CatanHelpText));
                optset.WriteOptionDescriptions(Console.Out);
                U.Exit();
            }

            return config;
        }

        private const string CatanHelpText = @"
Play Catan? Now keep track of your games and see if they are intrinsically fair!
";
        //1234567890123456789012345678901234567890123456789012345678901234567890

        internal class CatanOptions : ModuleOptions
        {
            public CRUD Catan { get; set; }
            public string ReadCatanStats { get; set; }
            public string ReadCatan { get; set; }
        }
    }
}
