using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Unlimitedinf.Apis.Client.Program
{
    internal static class Input
    {
        internal static Dictionary<string, string> Get(params string[] args)
        {
            var results = new Dictionary<string, string>(args.Length);

            foreach (var arg in args)
            {
                Console.Write($"{arg}: ");
                var value = Console.ReadLine().Trim();
                if (string.IsNullOrWhiteSpace(value))
                    return null;
                results.Add(arg, value);
            }

            return results;
        }

        internal static T Deserialize<T>(this string obj, out string token)
            where T : new()
        {
            token = (string)JObject.Parse(obj)["token"];
            return JsonConvert.DeserializeObject<T>(obj);
        }

        internal static T GetAndValidate<T>()
            where T : new()
        {
            var value = Input.Get<T>();
            Input.Validate(value);
            return value;
        }

        internal static T GetAndValidate<T>(out string token)
            where T : new()
        {
            token = GetToken(true);
            var value = Input.Get<T>();
            Input.Validate(value);
            return value;
        }

        internal static T Get<T>()
            where T : new()
        {
            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
            var result = new T();

            foreach (var property in properties)
            {
                Console.Write($"{property.Name}: ");
                var value = Console.ReadLine().Trim();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    // Unfortunately, I have yet to find a better way to do this

                    // Enums
                    if (property.PropertyType.IsEnum)
                        property.SetValue(result, Enum.Parse(property.PropertyType, value));

                    // Base types
                    else if (property.PropertyType == typeof(String))
                        property.SetValue(result, value);
                    else if (property.PropertyType == typeof(Boolean))
                        property.SetValue(result, Boolean.Parse(value));

                    // Objects
                    else if (property.PropertyType == typeof(Tools.SemVer))
                        property.SetValue(result, Tools.SemVer.Parse(value));

                    else
                        throw new NotImplementedException($"Property '{property.Name}' of type '{property.PropertyType}' does not have a valid conversion defined.");
                }
            }

            return result;
        }

        internal static T Get<T>(out string token)
            where T : new()
        {
            token = GetToken();
            return Get<T>();
        }

        internal static string GetToken(bool validate = false)
        {
            Console.Write("token: ");
            var value = Console.ReadLine().Trim();

            if (validate)
                ValidateToken(value);

            return value;
        }

        internal static void Validate<T>(T toValidate)
        {
            var errs = new List<ValidationResult>();
            var vc = new ValidationContext(toValidate);
            var vr = Validator.TryValidateObject(toValidate, vc, errs, true);

            foreach (var result in errs)
                Console.Error.WriteLine(result.ErrorMessage);

            if (!vr)
                Environment.Exit(ExitCode.ValidationFailed);
        }

        internal static void Validate<T>(T toValidate, string token)
        {
            ValidateToken(token);
            Validate(toValidate);
        }

        internal static void ValidateToken(string token)
        {
            if (Contracts.Auth.Token.IsTokenExpired(token))
            {
                Console.Error.WriteLine("Invalid token.");
                Environment.Exit(ExitCode.ValidationFailed);
            }
        }
    }
}
