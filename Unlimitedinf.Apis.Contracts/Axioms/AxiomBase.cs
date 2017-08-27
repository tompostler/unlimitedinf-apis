using System;
using System.ComponentModel.DataAnnotations;

namespace Unlimitedinf.Apis.Contracts.Axioms
{
    /// <summary>
    /// Representing an axiom.
    /// </summary>
    /// 
    /// <example>
    /// {
    ///   "type":   "Http",
    ///   "id":     "403",
    ///   "mod":    "2017-08-28 14:15:92.65",
    ///   "sum":    "Forbidden",
    ///   "desc":   "The request was valid, but the server is refusing action. The user might not have the necessary
    ///              permissions for a resource, or may need an account of some sort.",
    ///   "src":    "https://en.wikipedia.org/wiki/List_of_HTTP_status_codes#4xx_Client_errors"
    /// }
    /// </example>
    public class AxiomBase
    {
        /// <summary>
        /// The type of axiom.
        /// </summary>
        /// <remarks>
        /// This will be used in the url of the API. E.g. ~/axioms/type/id
        /// </remarks>
        [Required, StringLength(32)]
        public string type { get; set; }

        /// <summary>
        /// The id of an axiom.
        /// </summary>
        /// <remarks>
        /// This will be used in the url of the API. E.g. ~/axioms/type/id
        /// </remarks>
        [Required, StringLength(32)]
        public string id { get; set; }

        /// <summary>
        /// The last modified date of an axiom (in the system).
        /// </summary>
        [Required]
        public DateTimeOffset mod { get; set; }

        /// <summary>
        /// The axiom summary. Should be a short descriptor relating to the id.
        /// </summary>
        [Required, StringLength(128)]
        public string sum { get; set; }

        /// <summary>
        /// The axiom description. Should be a handful of sentences describing the axiom.
        /// </summary>
        [StringLength(1024)]
        public string desc { get; set; }

        /// <summary>
        /// The axiom source. Every axiom should be proven.
        /// </summary>
        [Required, StringLength(256)]
        public string src { get; set; }

        //TODO: Add a property containing markdown-formatted text for pretty html display.
    }
}
