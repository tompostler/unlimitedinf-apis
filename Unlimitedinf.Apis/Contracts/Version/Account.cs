using System.ComponentModel.DataAnnotations;

namespace Unlimitedinf.Apis.Contracts.Version
{
    /// <summary>
    /// Representing a version account.
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

        /// <summary>
        /// When used, this is to confirm access to update an existing account with a new secret.
        /// </summary>
        [StringLength(100)]
        public string oldsecret { get; set; }
    }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class AccountValidator
    {
        public const string PartitionKey = "accounts";

        public static ValidationResult Username(string username, ValidationContext context)
        {
            if (username != null && username.Equals(PartitionKey))
                return new ValidationResult($"Username cannot be '{PartitionKey}'");
            else
                return null;
        }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
