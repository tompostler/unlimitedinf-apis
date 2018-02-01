using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Unlimitedinf.Apis.Contracts.CustomValidators;
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
    public class Token : TokenDelete
    {
        /// <summary>
        /// When this token expires.
        /// </summary>
        [Required]
        public DateTimeOffset expiration { get; set; }

        internal const string DateTimeFmt = "yyyyMMddHHmmss";

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

            DateTimeOffset tdt;
            if (!DateTimeOffset.TryParseExact(token.Chop(DateTimeFmt.Length), DateTimeFmt, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out tdt))
                return true;

            return tdt < DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// Without hitting the servie, get the username from a token. Will be the normalized username.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public static string GetUsernameFrom(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token));

            token = token.FromBase64String();
            return token.Split(' ')[1];
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
        [Required, StringLength(128)]
        public string token { get; set; }
    }

    /// <summary>
    /// Determine how long before the token expires.
    /// </summary>
    public enum TokenExpiration
    {
        /// <summary>
        /// The token expires after one minute (<c>DateTimeOffset.UtcNow.AddMinutes(1)</c>).
        /// </summary>
        minute,
        /// <summary>
        /// The token expires after one hour (<c>DateTimeOffset.UtcNow.AddHours(1)</c>).
        /// </summary>
        hour,
        /// <summary>
        /// The token expires after one day (<c>DateTimeOffset.UtcNow.AddDays(1)</c>).
        /// </summary>
        day,
        /// <summary>
        /// The token expires after one week (<c>DateTimeOffset.UtcNow.AddDays(7)</c>).
        /// </summary>
        week,
        /// <summary>
        /// The token expires after one month (<c>DateTimeOffset.UtcNow.AddMonths(1)</c>).
        /// </summary>
        month,
        /// <summary>
        /// The token expires after one quarter (approx 90 days, <c>DateTimeOffset.UtcNow.AddMonths(3)</c>).
        /// </summary>
        quarter,
        /// <summary>
        /// The token expires after one year (<c>DateTimeOffset.UtcNow.AddYears(1)</c>).
        /// </summary>
        year,
        /// <summary>
        /// The token expires never. This is not recommended, for obvious reasons. However, will set to <c>DateTimeOffset.MaxValue</c>.
        /// </summary>
        never
    }
}
