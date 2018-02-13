using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Unlimitedinf.Apis.Contracts;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Client
{
    internal static class Input
    {
        internal static Dictionary<string, string> Get(params string[] args)
        {
            var results = new Dictionary<string, string>(args.Length);

            foreach (var arg in args)
            {
                var value = Get(arg);
                if (string.IsNullOrWhiteSpace(value))
                    return null;
                results.Add(arg, value);
            }

            return results;
        }

        internal static string Get(string thingToGet, bool useConfig = true)
        {
            if (useConfig)
            {
                if (thingToGet.Equals("username", StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(Settings.I.Username))
                {
                    Log.Ver("Retrieved username from settings.");
                    return Settings.I.Username;
                }
            }

            Console.Write($"{thingToGet}: ");
            return Console.ReadLine().Trim();
        }

        internal static T Deserialize<T>(this string obj, out string token)
            where T : new()
        {
            token = (string)JObject.Parse(obj)["token"];
            if (string.IsNullOrWhiteSpace(token))
                token = GetToken();
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
            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty).ToList();
            // Use the input order attribute to figure out the order that we should ask about the properties
            properties = properties.OrderBy(p => p.GetCustomAttributes<InputOrderAttribute>().SingleOrDefault()?.Order ?? 0).ToList();
            var result = new T();

            foreach (var property in properties)
            {
                var r = property.GetCustomAttribute<RequiredAttribute>() == null ? "?" : string.Empty;
                var v = string.Empty;
                if (Settings.I.VerboseInput)
                {
                    // Added as deemed necessary for assistance
                    if (property.PropertyType.IsEnum)
                        v = $" [{string.Join(",", Enum.GetNames(property.PropertyType))}]";
                    else if (property.PropertyType == typeof(Boolean))
                        v = " [true,false]";
                    else if (property.PropertyType == typeof(Guid))
                        v = " [<guid>]";
                }

                var value = Get($"{property.Name}{r}{v}");
                if (!string.IsNullOrWhiteSpace(value))
                {
                    // Unfortunately, I have yet to find a better way to do this

                    // Enums
                    if (property.PropertyType.IsEnum)
                        property.SetValue(result, Enum.Parse(property.PropertyType, value));

                    // Base types
                    else if (property.PropertyType == typeof(Boolean))
                        property.SetValue(result, Boolean.Parse(value));
                    else if (property.PropertyType == typeof(Guid))
                        property.SetValue(result, Guid.Parse(value));
                    else if (property.PropertyType == typeof(Int32))
                        property.SetValue(result, Int32.Parse(value));
                    else if (property.PropertyType == typeof(Int64))
                        property.SetValue(result, Int64.Parse(value));
                    else if (property.PropertyType == typeof(String))
                        property.SetValue(result, value);

                    // Objects
                    else if (property.PropertyType == typeof(SemVer))
                        property.SetValue(result, SemVer.Parse(value));
                    else if (property.PropertyType == typeof(Uri))
                        property.SetValue(result, new Uri(value));

                    else
                        throw new TomIsLazyException($"Property '{property.Name}' of type '{property.PropertyType}' does not have a valid conversion defined.");
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
            string token = string.Empty;
            if (!string.IsNullOrWhiteSpace(Settings.I.Token))
            {
                token = Settings.I.Token;
                Log.Ver("Retrieved token from settings.");
            }
            else
            {
                Console.Write("token: ");
                token = Console.ReadLine().Trim();
            }

            if (validate)
                ValidateToken(token);

            return token;
        }

        internal static void Validate<T>(T toValidate)
        {
            var errs = new List<ValidationResult>();
            var vc = new ValidationContext(toValidate);
            var vr = Validator.TryValidateObject(toValidate, vc, errs, true);

            foreach (var result in errs)
                Log.Err(result.ErrorMessage);

            if (!vr)
                U.Exit();
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
                Log.Err("Invalid token.");
                U.Exit();
            }
        }
    }
}