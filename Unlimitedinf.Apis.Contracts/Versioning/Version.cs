using System.ComponentModel.DataAnnotations;
using Unlimitedinf.Apis.Contracts.CustomValidators;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Contracts.Versioning
{
    /// <summary>
    /// Representing a version.
    /// </summary>
    public class Version
    {
        /// <summary>
        /// <see cref="Auth.Account.username"/>.
        /// </summary>
        [Required, StringLength(100), CustomValidation(typeof(AccountValidator), nameof(AccountValidator.UsernameValidation)), InputOrder(0)]
        public string username { get; set; }

        /// <summary>
        /// The unique name for this version.
        /// </summary>
        [Required, StringLength(100), CustomValidation(typeof(KeyFieldValidator), nameof(KeyFieldValidator.KeyFieldValidation)), InputOrder(1)]
        public string name { get; set; }

        /// <summary>
        /// The actual version.
        /// </summary>
        [Required, InputOrder(2)]
        public SemVer version { get; set; }
    }

    /// <summary>
    /// Representing version incrementation.
    /// </summary>
    public class VersionIncrement
    {
        /// <summary>
        /// Which part of the version to increment.
        /// </summary>
        [Required, InputOrder(0)]
        public VersionIncrementOption inc { get; set; }

        /// <summary>
        /// Whether or not to reset all the following parts of the version. Default is true.
        /// </summary>
        [InputOrder(1)]
        public bool reset { get; set; } = true;
    }

    /// <summary>
    /// Representing which part of the version to increment.
    /// </summary>
    public enum VersionIncrementOption
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        major,
        minor,
        patch
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
