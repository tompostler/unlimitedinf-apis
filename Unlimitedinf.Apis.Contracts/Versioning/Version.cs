using System.ComponentModel.DataAnnotations;
using Unlimitedinf.Apis.Contracts.Auth;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Contracts.Versioning
{
    /// <summary>
    /// Representing a version.
    /// </summary>
    public class Version
    {
        /// <summary>
        /// <see cref="Account.username"/>.
        /// </summary>
        [Required, StringLength(100), CustomValidation(typeof(AccountValidator), nameof(AccountValidator.UsernameValidation))]
        public string username { get; set; }

        /// <summary>
        /// The unique name for this version.
        /// </summary>
        [Required, StringLength(100)]
        public string name { get; set; }

        /// <summary>
        /// The actual version.
        /// </summary>
        [Required]
        public SemVer version { get; set; }
    }

    /// <summary>
    /// Representing version incrementation.
    /// </summary>
    public class VersionIncrement
    {
        /// <summary>
        /// See <see cref="Account.username"/>.
        /// </summary>
        [Required, StringLength(100), CustomValidation(typeof(AccountValidator), nameof(AccountValidator.UsernameValidation))]
        public string username { get; set; }

        /// <summary>
        /// See <see cref="Version.name"/>.
        /// </summary>
        [Required, StringLength(100)]
        public string name { get; set; }

        /// <summary>
        /// Which part of the version to increment.
        /// </summary>
        [Required]
        public VersionIncrementOption inc { get; set; }

        /// <summary>
        /// Whether or not to reset all the following parts of the version. Default is true.
        /// </summary>
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
