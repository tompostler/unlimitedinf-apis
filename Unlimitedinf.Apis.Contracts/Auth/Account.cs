﻿using System.ComponentModel.DataAnnotations;
using Unlimitedinf.Apis.Contracts.CustomValidators;

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
        [Required, StringLength(32), CustomValidation(typeof(AccountValidator), nameof(AccountValidator.UsernameValidation))]
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
}
