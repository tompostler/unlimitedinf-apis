using Newtonsoft.Json;
using System;
using Unlimitedinf.Apis.Contracts.Auth;

namespace Unlimitedinf.Apis.Client.Program
{
    internal static class AToken
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
                    return AToken.Create(rargs);
                case "delete":
                    return AToken.Delete(rargs);

                default:
                    return App.PrintHelp();
            }
        }

        private static int Create(string[] args)
        {
            TokenCreate tokenc = null;
            if (args.Length == 1)
                tokenc = JsonConvert.DeserializeObject<TokenCreate>(args[0]);
            else
                tokenc = Input.Get<TokenCreate>();
            Input.Validate(tokenc);

            var token = ApiClientAuth.Token.Create(tokenc).GetAwaiter().GetResult();
            Console.WriteLine(JsonConvert.SerializeObject(token, Formatting.Indented));

            return ExitCode.Success;
        }

        private static int Delete(string[] args)
        {
            TokenDelete token = null;
            if (args.Length == 1)
                token = JsonConvert.DeserializeObject<TokenDelete>(args[0]);
            else
                token = Input.Get<TokenDelete>();
            Input.Validate(token);

            ApiClientAuth.Token.Delete(token).GetAwaiter().GetResult();
            Console.WriteLine(JsonConvert.SerializeObject(new Token { name = token.name, token = token.token, username = token.username }, Formatting.Indented));

            return ExitCode.Success;
        }
    }
}
