using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Unlimitedinf.Apis.Contracts.CustomValidators
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class AccountValidator
    {
        private static readonly Regex Whitespace = new Regex(@"\s", RegexOptions.Compiled);

        private static readonly HashSet<string> CantUse = new HashSet<string>
        {
            "accounts"
        };

        public static ValidationResult UsernameValidation(string username, ValidationContext context)
        {
            if (username == null)
                return new ValidationResult("Username cannot be null.");
            else if (CantUse.Contains(username.ToLowerInvariant()))
                return new ValidationResult($"Username cannot be '{username}'");
            else if (Whitespace.IsMatch(username))
                return new ValidationResult("Username cannot contain whitespace.");

            var keyfieldValidationResult = KeyFieldValidator.KeyFieldValidation(username, context);
            return keyfieldValidationResult;
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
