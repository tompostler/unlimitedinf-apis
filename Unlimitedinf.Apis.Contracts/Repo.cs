using System;
using System.ComponentModel.DataAnnotations;
using Unlimitedinf.Apis.Contracts.CustomValidators;

namespace Unlimitedinf.Apis.Contracts
{
    /// <summary>
    /// Representing a repo.
    /// </summary>
    public class Repo
    {
        /// <summary>
        /// <see cref="Auth.Account.username"/>.
        /// </summary>
        [Required, StringLength(100), CustomValidation(typeof(AccountValidator), nameof(AccountValidator.UsernameValidation)), InputOrder(0)]
        public string username { get; set; }

        /// <summary>
        /// The unique name for this repo. Also used when cloning the repo.
        /// </summary>
        [Required, StringLength(100), CustomValidation(typeof(KeyFieldValidator), nameof(KeyFieldValidator.KeyFieldValidation)), InputOrder(1)]
        public string name { get; set; }

        /// <summary>
        /// If specified, will be used for cloning the repo instead of the repo name. Note: this field is not checked
        /// for uniqueness, and should use forward slashes as directory separators. Spaces are also inadvisable.
        /// </summary>
        [StringLength(128), InputOrder(2)]
        public string path { get; set; }

        /// <summary>
        /// The repo URI.
        /// </summary>
        [Required, InputOrder(3)]
        public Uri repo { get; set; }

        /// <summary>
        /// The optional user.name configuration when cloning the repo.
        /// </summary>
        [InputOrder(4)]
        public string gitusername { get; set; }

        /// <summary>
        /// The optional user.email configuration when cloning the repo.
        /// </summary>
        [EmailAddress, InputOrder(5)]
        public string gituseremail { get; set; }
    }
}
