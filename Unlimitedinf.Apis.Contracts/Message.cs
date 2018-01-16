using System;
using System.ComponentModel.DataAnnotations;
using Unlimitedinf.Apis.Contracts.CustomValidators;

namespace Unlimitedinf.Apis.Contracts
{
    /// <summary>
    /// Representing a message.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// See <see cref="Auth.Account.username"/>. The server will force this to lower case.
        /// </summary>
        [Required, StringLength(100), CustomValidation(typeof(AccountValidator), nameof(AccountValidator.UsernameValidation))]
        public string from { get; set; }

        /// <summary>
        /// The target recipient for this message. This is user defined, so it can represent ad-hoc groups, specific
        /// usernames, friendly names, etc. at some point in the future. Right now, it has to target another existing
        /// username. The server will force this to lower case, and yes you can send messages to yourself.
        /// </summary>
        [Required, StringLength(100), CustomValidation(typeof(KeyFieldValidator), nameof(KeyFieldValidator.KeyFieldValidation))]
        public string to { get; set; }

        /// <summary>
        /// This way you can have multiple conversations at a time with a specific user. The server will force this to
        /// lower case.
        /// </summary>
        [Required, StringLength(100), CustomValidation(typeof(KeyFieldValidator), nameof(KeyFieldValidator.KeyFieldValidation))]
        public string subject { get; set; }

        /// <summary>
        /// The id of the message this message is a response to. In multipart messages, use the first message as the
        /// reply message id.
        /// </summary>
        public Guid? rept { get; set; }

        /// <summary>
        /// The creation timestamp. Assigned by the server.
        /// </summary>
        public DateTimeOffset timestamp { get; set; }

        /// <summary>
        /// The message content.
        /// </summary>
        [Required, StringLength(1000, MinimumLength = 1)]
        public string message { get; set; }

        /// <summary>
        /// A message (or message part) can be marked as read.
        /// </summary>
        public bool read { get; set; }

        /// <summary>
        /// A message can be multipart. This is to aid in assembling in the right order. 100 message max.
        /// </summary>
        [Range(0, 100)]
        public byte part { get; set; } = 0;

        /// <summary>
        /// Set by the server.
        /// </summary>
        public Guid? id { get; set; }
    }
}
