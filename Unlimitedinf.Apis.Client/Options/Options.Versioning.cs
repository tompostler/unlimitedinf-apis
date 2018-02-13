using Mono.Options;
using Newtonsoft.Json;
using System;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Client.Options
{
    internal static partial class Options
    {
        public static VersioningOptions ParseVersioning(string[] args)
        {
            var config = new VersioningOptions();
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
                    "v|create-version",
                    "Begins the interactive process of creating a version.",
                    v => config.Version = CRUD.Create
                },
                {
                    "r|read-version=",
                    "Fetch information about an exisitng version, by a forward-slash-separated username/versionName pair. Alternatively, just specify the username to list all the versions for that user.",
                    v => config.ReadVersion = v
                },
                {
                    "u|update-version",
                    "Begins the interactive process of updating a version.",
                    v => config.Version = CRUD.Update
                },
                {
                    "d|delete-version",
                    "Begins the interactive process of deleting a version.",
                    v => config.Version = CRUD.Delete
                },
                {
                    "c|create-count",
                    "Begins the interactive process of creating a count.",
                    v => config.Count = CRUD.Create
                },
                {
                    "s|read-count=",
                    "Fetch information about an exisitng count, by a forward-slash-separated username/countName pair. Alternatively, just specify the username to list all the counts for that user.",
                    v => config.ReadCount = v
                },
                {
                    "t|update-count",
                    "Begins the interactive process of updating a count.",
                    v => config.Count = CRUD.Update
                },
                {
                    "e|delete-count",
                    "Begins the interactive process of deleting a count.",
                    v => config.Count = CRUD.Delete
                }
            };

            optset.Parse(args);
            Log.Ver("OPTIONS:");
            Log.Ver(config.ToString());
            Log.Line();

            if (config.Help)
            {
                Log.Inf(string.Format(Options.OptionsBaseHelpText, "ver", VersioningHelpText));
                optset.WriteOptionDescriptions(Console.Out);
                U.Exit();
            }

            return config;
        }

        private const string VersioningHelpText = @"
Ability to work with SemVer.2.0.0 compliant versions in addition to basic
counters. Mainly intended to help keep track of builds before I had discovered
the niceties of GitVersion, but now used in places where multiply-versioned
projects all exist in one repo that only has a master branch (so mainly my other
personal projects).
";
        //1234567890123456789012345678901234567890123456789012345678901234567890

        internal class VersioningOptions : ModuleOptions
        {
            public CRUD Count { get; set; }
            public CRUD Version { get; set; }
            public string ReadCount { get; set; }
            public string ReadVersion { get; set; }

            public override string ToString()
            {
                return JsonConvert.SerializeObject(this);
            }
        }
    }
}
