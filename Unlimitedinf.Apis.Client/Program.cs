﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unlimitedinf.Apis.Client.Options;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Client
{
    static class Program
    {
        public static async Task Main(string[] args)
        {
#if DEBUG
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
#endif // DEBUG

            Log.PrintVerbosityLevel = false;
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings { Formatting = Formatting.Indented, NullValueHandling = NullValueHandling.Ignore };
            Log.Ver("CONFIG: " + JsonConvert.SerializeObject(Settings.I));

            try
            {
                (var module, var options) = Options.Options.Parse(args);
                switch (module)
                {
                    case Module.Help:
                        Log.Inf(Options.Options.BaseHelpText);
                        return;
                    case Module.Config:
                        Modules.Config.Run(options);
                        return;
                    case Module.Auth:
                        await Modules.Auth.Run(options);
                        return;
                    case Module.Repo:
                        await Modules.Repo.Run(options);
                        return;
                }
            }
            catch (Exception e)
            {
                Log.Err(e.ToString());
                U.Exit();
            }
        }
    }
}
