using System.ComponentModel.DataAnnotations;
using Unlimitedinf.Apis.Contracts.CustomValidators;

namespace Unlimitedinf.Apis.Contracts.Versioning
{
    /// <summary>
    /// Representing a count.
    /// </summary>
    public class Count
    {
        /// <summary>
        /// <see cref="Auth.Account.username"/>.
        /// </summary>
        [Required, StringLength(100), CustomValidation(typeof(AccountValidator), nameof(AccountValidator.UsernameValidation))]
        public string username { get; set; }

        /// <summary>
        /// The unique name for this count.
        /// </summary>
        [Required, StringLength(100)]
        public string name { get; set; }

        /// <summary>
        /// The actual count.
        /// </summary>
        [Required]
        public long count { get; set; } = 0;
    }

    /// <summary>
    /// Representing count incrementation or decrementation.
    /// </summary>
    public class CountChange
    {
        /// <summary>
        /// See <see cref="Auth.Account.username"/>.
        /// </summary>
        [Required, StringLength(100), CustomValidation(typeof(AccountValidator), nameof(AccountValidator.UsernameValidation))]
        public string username { get; set; }

        /// <summary>
        /// See <see cref="Count.name"/>.
        /// </summary>
        [Required, StringLength(100)]
        public string name { get; set; }

        /// <summary>
        /// Whether we should increment, decrement, or reset the count to 0. Default: increment.
        /// </summary>
        public CountChangeOption type { get; set; } = CountChangeOption.inc;
    }

    /// <summary>
    /// Representing whether we should increment or decrement the count.
    /// </summary>
    public enum CountChangeOption
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        inc,
        dec,
        res
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    }
}
