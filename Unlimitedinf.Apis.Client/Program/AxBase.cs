using Newtonsoft.Json;
using System;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Client.Program
{
    internal static class AxBase
    {
        internal static int Run(string[] args)
        {
            if (args.Length == 0)
                return App.PrintHelp();

            var rargs = new string[args.Length - 1];
            Array.Copy(args, 1, rargs, 0, args.Length - 1);
            return AxBase.Read(rargs);
        }

        private static int Read(string[] args)
        {
            if (args.Length != 2)
            {
                Console.Error.WriteLine("Did not supply type or id arguments.");
                return ExitCode.ValidationFailed;
            }

            var result = ApiClient_Axioms.AxiomRead(args[0], args[1]).GetAwaiter().GetResult();
            Log.Inf(JsonConvert.SerializeObject(result, Formatting.Indented));
            return ExitCode.Success;
        }
    }
}
