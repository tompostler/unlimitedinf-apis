using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        [Required, StringLength(100), CustomValidation(typeof(AccountValidator), nameof(AccountValidator.Username))]
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
        private static readonly HashSet<string> CantUse = new HashSet<string>
        {
            "accounts"
        };

        public static ValidationResult Username(string username, ValidationContext context)
        {
            if (username != null && CantUse.Contains(username.ToLowerInvariant()))
                return new ValidationResult($"Username cannot be '{username}'");
            else
                return null;
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
