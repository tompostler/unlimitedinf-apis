using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Unlimitedinf.Apis.Contracts.Axioms;

namespace Unlimitedinf.Apis.Models.Axioms
{
    public static class AxiomCache
    {
        private static MemoryCache axiomFoundCache = new MemoryCache("AxiomFoundCache");
        private static MemoryCache axiomNotFoundCache = new MemoryCache("AxiomNotFoundCache");
        // Default found cache for a week if not accessed
        private static CacheItemPolicy defaultFoundCachePolicy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromDays(7) };
        // Default not found cache for 12ish hours if not accessed
        private static CacheItemPolicy defaultNotFoundCachePolicy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromHours(12) };

        /// <summary>
        /// Gets an axiom, given its type and id.
        /// </summary>
        /// <remarks>
        /// Will first check the memory cache to see if it was requested before, otherwise reaches out to tablestorage
        /// to get the value.
        /// Null return indicates axiom does not exist.
        /// </remarks>
        public static async Task<AxiomBase> Get(string type, string id)
        {
            // Cache key is type_id, e.g. http_403
            type = type.ToLowerInvariant();
            id = id.ToLowerInvariant();
            var key = type + "_" + id;
            Trace.TraceInformation("Axiom " + key);

            // First, check not found cache
            if (axiomNotFoundCache.Contains(key))
            {
                Trace.TraceInformation("Axiom not found: based on cache");
                return null;
            }

            // Second, check found cache
            if (axiomFoundCache.Contains(key))
            {
                Trace.TraceInformation("Axiom found: based on cache");
                return axiomFoundCache.Get(key) as AxiomBase;
            }

            // Lastly, check tablestorage and add to the appropriate cache
            var retrieve = TableOperation.Retrieve<AxiomBaseEntity>(type.ToLowerInvariant(), id.ToLowerInvariant());
            var result = await TableStorage.Axioms.ExecuteAsync(retrieve);

            // Not found
            if (result.Result == null)
            {
                Trace.TraceInformation("Axiom not found: based on tablestorage");
                axiomNotFoundCache.Add(key, false, defaultNotFoundCachePolicy);
                return null;
            }

            // Found
            Trace.TraceInformation("Axiom found: based on tablestorage");
            var axiom = (AxiomBase)(AxiomBaseEntity)result.Result;
            axiomFoundCache.Add(key, axiom, defaultFoundCachePolicy);
            return axiom;
        }
    }
}