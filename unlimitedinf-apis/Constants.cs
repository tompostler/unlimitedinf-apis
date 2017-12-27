namespace Unlimitedinf.Apis
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
            /// A list containing just PartitionKey and RowKey to help with filtering.
            /// </summary>
            public static readonly string[] PRKF = new string[] { PK, RK };
        }
    }
}