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
        [Required, StringLength(100), CustomValidation(typeof(AccountValidator), nameof(AccountValidator.UsernameValidation))]
        public string username { get; set; }

        /// <summary>
        /// The unique name for this repo. Also used when cloning the repo.
        /// </summary>
        [Required, StringLength(100), CustomValidation(typeof(KeyFieldValidator), nameof(KeyFieldValidator.KeyFieldValidation))]
        public string name { get; set; }

        /// <summary>
        /// If specified, will be used for cloning the repo instead of the repo name. Note: this field is not checked
        /// for uniqueness, and should use forward slashes as directory separators. Spaces are also inadvisable.
        /// </summary>
        [StringLength(128)]
        public string path { get; set; }

        /// <summary>
        /// The repo URI.
        /// </summary>
        [Required]
        public Uri repo { get; set; }

        /// <summary>
        /// The optional user.name configuration when cloning the repo.
        /// </summary>
        public string gitusername { get; set; }

        /// <summary>
        /// The optional user.email configuration when cloning the repo.
        /// </summary>
        [EmailAddress]
        public string gituseremail { get; set; }
    }
}
