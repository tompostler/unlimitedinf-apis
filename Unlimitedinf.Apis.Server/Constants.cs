using System.Collections.Generic;

namespace Unlimitedinf.Apis.Server
{
    /// <summary>
    /// Constants.
    /// </summary>
    internal static class C
    {
        /// <summary>
        /// Table storage constants.
        /// </summary>
        internal static class TS
        {
            public const string PK = "PartitionKey";
            public const string RK = "RowKey";

            /// <summary>
            /// An array containing just PartitionKey and RowKey to help with filtering.
            /// </summary>
            public static readonly string[] PRKF = new string[] { PK, RK };
            public static readonly List<string> PRKFL = new List<string>(PRKF);
        }
    }
}