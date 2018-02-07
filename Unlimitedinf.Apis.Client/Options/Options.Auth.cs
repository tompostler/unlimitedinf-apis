using Mono.Options;
using Newtonsoft.Json;
using System;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Client.Options
{
    internal static partial class Options
    {
        public static AuthOptions ParseAuth(string[] args)
        {
            var config = new AuthOptions();
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
                    "c|create-account",
                    "Begins the interactive process of creating an account.",
                    v => config.Account = CRUD.Create
                },
                {
                    "r|read-account=",
                    "Fetch information about an exisitng account, by username.",
                    v => config.ReadAccount = v
                },
                {
                    "u|update-account",
                    "Begins the interactive process of updating an account.",
                    v => config.Account = CRUD.Update
                },
                {
                    "d|delete-account",
                    "Begins the interactive process of deleting an account.",
                    v => config.Account = CRUD.Delete
                },
                {
                    "t|create-token",
                    "Begins the interactive process of creating a token.",
                    v => config.CreateToken = true
                },
                {
                    "x|delete-token=",
                    "Delete the token.",
                    v => config.DeleteToken = v
                }
            };

            optset.Parse(args);
            Log.Ver("OPTIONS:");
            Log.Ver(config.ToString());
            Log.Line();

            if (config.Help)
            {
                Log.Inf(string.Format(Options.OptionsBaseHelpText, "auth", AuthHelpText));
                optset.WriteOptionDescriptions(Console.Out);
                U.Exit();
            }

            return config;
        }

        private const string AuthHelpText = @"
If a username or token is found in the settings during the execution of this
program, then it will preempt any time that the username or token is required
for input (with the exception being the execution of the auth module).
";
        //1234567890123456789012345678901234567890123456789012345678901234567890

        internal class AuthOptions : ModuleOptions
        {
            public CRUD Account { get; set; }
            public string ReadAccount { get; set; }
            public bool CreateToken { get; set; }
            public string DeleteToken { get; set; }

            public override string ToString()
            {
                return JsonConvert.SerializeObject(this);
            }
        }
    }
}
