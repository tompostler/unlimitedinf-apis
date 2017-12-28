using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using Unlimitedinf.Apis.Contracts;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Client.Program
{
    internal static class MessageProgram
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
                    return MessageProgram.Create(rargs);
                case "read":
                    return MessageProgram.Read();
                case "reada":
                    return MessageProgram.ReadAll();
                case "mread":
                    return MessageProgram.MarkAsRead(rargs);
                case "delete":
                    return MessageProgram.Delete(rargs);

                default:
                    return App.PrintHelp();
            }
        }

        private static int Create(string[] args)
        {
            Message message = null;
            string token = null;
            if (args.Length == 1)
                message = args[0].Deserialize<Message>(out token);
            else
                message = Input.Get<Message>(out token);
            Input.Validate(message, token);

            var client = new ApiClient(token);

            message = client.Messages.Create(message).GetAwaiter().GetResult();
            Log.Inf(JsonConvert.SerializeObject(message, Formatting.Indented));

            return ExitCode.Success;
        }

        private static int Read()
        {
            var client = new ApiClient(Input.GetToken());

            var result = client.Messages.Read().GetAwaiter().GetResult();
            Log.Inf(JsonConvert.SerializeObject(result, Formatting.Indented));
            return ExitCode.Success;
        }

        private static int ReadAll()
        {
            var client = new ApiClient(Input.GetToken());

            var result = client.Messages.ReadAll().GetAwaiter().GetResult();
            Log.Inf(JsonConvert.SerializeObject(result, Formatting.Indented));
            return ExitCode.Success;
        }

        private static int MarkAsRead(string[] args)
        {
            if (args.Length == 1)
            {
                if (string.IsNullOrWhiteSpace(args[0]))
                {
                    Log.Err("Did not supply message id argument.");
                    return ExitCode.ValidationFailed;
                }

                var client = new ApiClient(Input.GetToken());

                var result = client.Messages.MarkAsRead(Guid.Parse(args[0])).GetAwaiter().GetResult();
                Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
                return ExitCode.Success;
            }

            Log.Err("Did not supply message id argument.");
            return ExitCode.ValidationFailed;
        }

        private static int Delete(string[] args)
        {
            if (args.Length == 1)
            {
                if (string.IsNullOrWhiteSpace(args[0]))
                {
                    Log.Err("Did not supply message id argument.");
                    return ExitCode.ValidationFailed;
                }

                var client = new ApiClient(Input.GetToken());

                var result = client.Messages.Delete(Guid.Parse(args[0])).GetAwaiter().GetResult();
                Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
                return ExitCode.Success;
            }
            else if (args.Length > 1)
            {
                List<Guid> ids = new List<Guid>();
                if (!args.All(arg =>
                {
                    Guid id;
                    if (!Guid.TryParse(arg, out id))
                    {
                        Log.Err($"Invalid guid: {arg}");
                        return false;
                    }
                    ids.Add(id);
                    return true;
                }))
                {
                    Log.Err("Did not supply all valid message ids.");
                    return ExitCode.ValidationFailed;
                }

                var client = new ApiClient(Input.GetToken());

                var result = client.Messages.Delete(ids).GetAwaiter().GetResult();
                Console.WriteLine(JsonConvert.SerializeObject(result, Formatting.Indented));
                return ExitCode.Success;
            }

            Log.Err("Did not supply message id argument(s).");
            return ExitCode.ValidationFailed;
        }
    }
}
