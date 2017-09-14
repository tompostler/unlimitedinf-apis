using Newtonsoft.Json;
using System;
using Unlimitedinf.Apis.Contracts.Versioning;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Client.Program
{
    internal static class VCount
    {
        internal static int Run(string[] args)
        {
            if (args.Length == 0)
                return App.PrintHelp();

            var rargs = new string[args.Length - 1];
            Array.Copy(args, 1, rargs, 0, args.Length - 1);
            switch (args[0])
            {
                case "create":
                    return VCount.Create(rargs);
                case "read":
                    return VCount.Read(rargs);
                case "update":
                    return VCount.Update(rargs);
                case "delete":
                    return VCount.Delete(rargs);

                default:
                    return App.PrintHelp();
            }
        }

        private static int Create(string[] args)
        {
            Count count = null;
            string token = null;
            if (args.Length == 1)
                count = args[0].Deserialize<Count>(out token);
            else
                count = Input.Get<Count>(out token);
            Input.Validate(count, token);

            var client = new ApiClient(token);

            count = client.Versioning.CountCreate(count).GetAwaiter().GetResult();
            Log.Inf(JsonConvert.SerializeObject(count, Formatting.Indented));

            return ExitCode.Success;
        }

        private static int Read(string[] args)
        {
            if (args.Length == 1)
            {
                if (string.IsNullOrWhiteSpace(args[0]))
                {
                    Console.Error.WriteLine("Did not supply username argument.");
                    return ExitCode.ValidationFailed;
                }

                var result = ApiClient_Versioning.CountRead(args[0]).GetAwaiter().GetResult();
                Log.Inf(JsonConvert.SerializeObject(result, Formatting.Indented));
                return ExitCode.Success;
            }

            if (args.Length == 2)
            {
                if (string.IsNullOrWhiteSpace(args[0]) || string.IsNullOrWhiteSpace(args[1]))
                {
                    Console.Error.WriteLine("Did not supply username or countName argument.");
                    return ExitCode.ValidationFailed;
                }

                var result = ApiClient_Versioning.CountRead(args[0], args[1]).GetAwaiter().GetResult();
                Log.Inf(JsonConvert.SerializeObject(result, Formatting.Indented));
                return ExitCode.Success;
            }

            Log.Err("Unexpected arguments.");
            return ExitCode.ValidationFailed;
        }

        private static int Update(string[] args)
        {
            CountChange countChange = null;
            string token = null;
            if (args.Length == 1)
                countChange = args[0].Deserialize<CountChange>(out token);
            else
                countChange = Input.Get<CountChange>(out token);
            Input.Validate(countChange, token);

            var client = new ApiClient(token);
            var version = client.Versioning.CountUpdate(countChange).GetAwaiter().GetResult();
            Log.Inf(JsonConvert.SerializeObject(version, Formatting.Indented));

            return ExitCode.Success;
        }

        private static int Delete(string[] args)
        {
            Count count = null;
            string token = null;
            if (args.Length == 1)
                count = args[0].Deserialize<Count>(out token);
            else
                count = Input.Get<Count>(out token);
            Input.Validate(count, token);

            var client = new ApiClient(token);

            count = client.Versioning.CountDelete(count.username, count.name).GetAwaiter().GetResult();
            Log.Inf(JsonConvert.SerializeObject(count, Formatting.Indented));

            return ExitCode.Success;
        }
    }
}
