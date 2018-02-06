using Mono.Options;
using Newtonsoft.Json;
using System;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Client.Options
{
    internal static partial class Options
    {
        public static RepoOptions ParseRepo(string[] args)
        {
            var config = new RepoOptions();
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
                    "create-repo",
                    "Begins the interactive process of creating a repo.",
                    v => config.Repo = CRUD.Create
                },
                {
                    "read-repos",
                    "Fetch list of all exisiting repos.",
                    v => config.Repo = CRUD.Read
                },
                {
                    "read-ps",
                    "Fetch the PowerShell script to create all the repos under the ~/Source/Repos directory. Built for Windows, and expects git to already be installed.",
                    v => config.Powershell = true
                },
                {
                    "delete-repo",
                    "Begins the interactive process of deleting a repo.",
                    v => config.Repo = CRUD.Delete
                }
            };

            optset.Parse(args);
            Log.Ver("OPTIONS:");
            Log.Ver(config.ToString());
            Log.Line();

            if (config.Help)
            {
                Log.Inf(string.Format(Options.OptionsBaseHelpText, "auth", RepoHelpText));
                optset.WriteOptionDescriptions(Console.Out);
                U.Exit();
            }

            return config;
        }

        private const string RepoHelpText = @"
Manage repositories on a per-user basis. Mainly used for cloning all your
repositories at once with the use of a PowerShell script.
";
        //1234567890123456789012345678901234567890123456789012345678901234567890

        internal class RepoOptions : ModuleOptions
        {
            public CRUD Repo { get; set; }
            public bool Powershell { get; set; }

            public override string ToString()
            {
                return JsonConvert.SerializeObject(this);
            }
        }
    }
}
