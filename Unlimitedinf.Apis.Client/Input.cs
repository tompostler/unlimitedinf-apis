using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Unlimitedinf.Apis.Client
{
    internal static class Input
    {
        public static Dictionary<string, string> Get(params string[] args)
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

        public static T GetAndValidate<T>()
            where T : new()
        {
            var value = Input.Get<T>();
            Input.Validate(value);
            return value;
        }

        public static T Get<T>()
            where T : new()
        {
            var properties = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
            var result = new T();

            foreach (var property in properties)
            {
                Console.Write($"{property.Name}: ");
                var value = Console.ReadLine().Trim();
                if (!string.IsNullOrWhiteSpace(value))
                    property.SetValue(result, Convert.ChangeType(value, property.PropertyType));
            }

            return result;
        }

        public static void Validate<T>(T toValidate)
        {
            var errs = new List<ValidationResult>();
            var vc = new ValidationContext(toValidate);
            var vr = Validator.TryValidateObject(toValidate, vc, errs, true);

            foreach (var result in errs)
                Console.Error.WriteLine(result.ErrorMessage);

            if (!vr)
                Environment.Exit(ExitCode.ValidationFailed);
        }
    }
}
