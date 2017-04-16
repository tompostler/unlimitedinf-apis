using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Unlimitedinf.Apis.Contracts.Auth
{
    /// <summary>
    /// Representing an account.
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        [Required, StringLength(32), CustomValidation(typeof(AccountValidator), nameof(AccountValidator.Username))]
        public string username { get; set; }

        /// <summary>
        /// Point of contact.
        /// </summary>
        [Required, EmailAddress]
        public string email { get; set; }

        /// <summary>
        /// The secret used to protect the account.
        /// </summary>
        [Required, StringLength(100)]
        public string secret { get; set; }
    }

    /// <summary>
    /// Representing an account update.
    /// </summary>
    public class AccountUpdate : Account
    {
        /// <summary>
        /// The old secret used to protect the account.
        /// </summary>
        [Required, StringLength(100)]
        public string oldsecret { get; set; }
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class AccountValidator
    {
        private static Regex Whitespace = new Regex(@"\s", RegexOptions.Compiled);

        private static readonly HashSet<string> CantUse = new HashSet<string>
        {
            "accounts"
        };

        public static ValidationResult Username(string username, ValidationContext context)
        {
            if (username == null)
                return new ValidationResult("Username cannot be null.");
            else if (CantUse.Contains(username.ToLowerInvariant()))
                return new ValidationResult($"Username cannot be '{username}'");
            else if (Whitespace.IsMatch(username))
                return new ValidationResult("Username cannot contain whitespace.");
            else
                return null;
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
