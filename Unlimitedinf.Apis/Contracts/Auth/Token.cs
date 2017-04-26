using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.Contracts.Auth
{
    /// <summary>
    /// Representing the values needed to create a token.
    /// </summary>
    public class TokenCreate
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        [Required, StringLength(32), CustomValidation(typeof(AccountValidator), nameof(AccountValidator.UsernameValidation))]
        public string username { get; set; }

        /// <summary>
        /// The secret used to protect the account.
        /// </summary>
        [Required, StringLength(100)]
        public string secret { get; set; }

        /// <summary>
        /// Give a friendly name to the token. Required if you want multiple tokens.
        /// </summary>
        [StringLength(64)]
        public string name { get; set; }

        /// <summary>
        /// How long-lived you want the token to be.
        /// </summary>
        [Required]
        public TokenExpiration expire { get; set; }
    }

    /// <summary>
    /// Representing a token. A single account can have unlimited tokens.
    /// </summary>
    public class Token
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        [Required, StringLength(32), CustomValidation(typeof(AccountValidator), nameof(AccountValidator.UsernameValidation))]
        public string username { get; set; }

        /// <summary>
        /// Give a friendly name to the token. Required if you want multiple tokens.
        /// </summary>
        [StringLength(64)]
        public string name { get; set; }

        /// <summary>
        /// The Base64 token.
        /// </summary>
        [Required, StringLength(64)]
        public string token { get; set; }

        /// <summary>
        /// When this token expires.
        /// </summary>
        [Required]
        public DateTime expiration { get; set; }

        internal const string DateTimeFmt = "yyMMddHHmmss";

        /// <summary>
        /// Without hitting the service, easily determine if a token is expired.
        /// </summary>
        public static bool IsTokenExpired(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token));

            token = token.FromBase64String();
            if (token.Length < DateTimeFmt.Length)
                return true;

            DateTime tdt;
            if (!DateTime.TryParseExact(token.Chop(DateTimeFmt.Length), DateTimeFmt, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out tdt))
                return true;

            return tdt < DateTime.UtcNow;
        }
    }

    /// <summary>
    /// Representing what is needed to delete a token.
    /// </summary>
    public class TokenDelete
    {
        /// <summary>
        /// Unique identifier.
        /// </summary>
        [Required, StringLength(32), CustomValidation(typeof(AccountValidator), nameof(AccountValidator.UsernameValidation))]
        public string username { get; set; }

        /// <summary>
        /// Give a friendly name to the token. Required if you want multiple tokens.
        /// </summary>
        [StringLength(64)]
        public string name { get; set; }

        /// <summary>
        /// The Base64 token.
        /// </summary>
        [Required, StringLength(64)]
        public string token { get; set; }
    }

    /// <summary>
    /// Determine how long before the token expires.
    /// </summary>
    public enum TokenExpiration
    {
        /// <summary>
        /// The token expires after one minute (<c>DateTime.UtcNow.AddMinutes(1)</c>).
        /// </summary>
        Minute,
        /// <summary>
        /// The token expires after one hour (<c>DateTime.UtcNow.AddHours(1)</c>).
        /// </summary>
        Hour,
        /// <summary>
        /// The token expires after one day (<c>DateTime.UtcNow.AddDays(1)</c>).
        /// </summary>
        Day,
        /// <summary>
        /// The token expires after one week (<c>DateTime.UtcNow.AddDays(7)</c>).
        /// </summary>
        Week,
        /// <summary>
        /// The token expires after one month (<c>DateTime.UtcNow.AddMonths(1)</c>).
        /// </summary>
        Month,
        /// <summary>
        /// The token expires after one quarter (approx 90 days, <c>DateTime.UtcNow.AddMonths(3)</c>).
        /// </summary>
        Quarter,
        /// <summary>
        /// The token expires after one year (<c>DateTime.UtcNow.AddYears(1)</c>).
        /// </summary>
        Year,
        /// <summary>
        /// The token expires never. This is not recommended, for obvious reasons. However, will set to <c>DateTime.MaxValue</c>.
        /// </summary>
        Never
    }
}
