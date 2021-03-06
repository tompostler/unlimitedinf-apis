﻿using System.ComponentModel.DataAnnotations;
using Unlimitedinf.Apis.Contracts.CustomValidators;

namespace Unlimitedinf.Apis.Contracts.Auth
{
    /// <summary>
    /// Representing an account delete.
    /// </summary>
    public class AccountDelete
    {
        /// <summary>
        /// The secret used to protect the account.
        /// </summary>
        [Required, StringLength(100), InputOrder(1)]
        public string secret { get; set; }
    }

    /// <summary>
    /// Representing an account.
    /// </summary>
    public class Account : AccountDelete
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        [Required, StringLength(32), CustomValidation(typeof(AccountValidator), nameof(AccountValidator.UsernameValidation)), InputOrder(0)]
        public string username { get; set; }

        /// <summary>
        /// Point of contact.
        /// </summary>
        [Required, EmailAddress, InputOrder(2)]
        public string email { get; set; }
    }

    /// <summary>
    /// Representing an account update.
    /// </summary>
    public class AccountUpdate : AccountDelete
    {
        /// <summary>
        /// The old secret used to protect the account.
        /// </summary>
        [Required, StringLength(100), InputOrder(0)]
        public string oldsecret { get; set; }
    }
}
