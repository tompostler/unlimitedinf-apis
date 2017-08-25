﻿using Newtonsoft.Json;
using System;
using Unlimitedinf.Apis.Contracts.Auth;

namespace Unlimitedinf.Apis.Client.Program
{
    internal static class AuAccount
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
                    return AuAccount.Create(rargs);
                case "read":
                    return AuAccount.Read(rargs);
                case "update":
                    return AuAccount.Update(rargs);
                case "delete":
                    return AuAccount.Delete(rargs);

                default:
                    return App.PrintHelp();
            }
        }

        private static int Create(string[] args)
        {
            Account account = null;
            if (args.Length == 1)
                account = JsonConvert.DeserializeObject<Account>(args[0]);
            else
                account = Input.Get<Account>();
            Input.Validate(account);

            account = ApiClientAuth.Account.Create(account).GetAwaiter().GetResult();
            Console.WriteLine(JsonConvert.SerializeObject(account, Formatting.Indented));

            return ExitCode.Success;
        }

        private static int Read(string[] args)
        {
            if (args.Length != 1 || string.IsNullOrWhiteSpace(args[0]))
            {
                Console.Error.WriteLine("Did not supply username argument.");
                return ExitCode.ValidationFailed;
            }

            var account = ApiClientAuth.Account.Read(args[0]).GetAwaiter().GetResult();
            Console.WriteLine(JsonConvert.SerializeObject(account, Formatting.Indented));

            return ExitCode.Success;
        }

        private static int Update(string[] args)
        {
            AccountUpdate account = null;
            if (args.Length == 1)
                account = JsonConvert.DeserializeObject<AccountUpdate>(args[0]);
            else
                account = Input.Get<AccountUpdate>();
            Input.Validate(account);

            ApiClientAuth.Account.Update(account).GetAwaiter().GetResult();
            Console.WriteLine(JsonConvert.SerializeObject(new Account { email = account.email, username = account.username }, Formatting.Indented));

            return ExitCode.Success;
        }

        private static int Delete(string[] args)
        {
            Account account = null;
            if (args.Length == 1)
                account = JsonConvert.DeserializeObject<Account>(args[0]);
            else
                account = Input.Get<Account>();
            Input.Validate(account);

            ApiClientAuth.Account.Delete(account).GetAwaiter().GetResult();
            Console.WriteLine(JsonConvert.SerializeObject(new Account { email = account.email, username = account.username }, Formatting.Indented));

            return ExitCode.Success;
        }
    }
}