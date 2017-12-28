using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Unlimitedinf.Apis
{
    public class TableStorage
    {
        public static readonly CloudTableClient TableClient = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("unlimitedinfapis_AzureStorageConnectionString")).CreateCloudTableClient();

        public static CloudTable Axioms { get; }
        public static CloudTable Auth { get; }
        public static CloudTable Catans { get; }
        public static CloudTable Messages { get; }
        public static CloudTable Repos { get; }
        public static CloudTable Versioning { get; }

        public static List<CloudTable> AllTables => new List<CloudTable>
        {
            Axioms, Auth, Catans, Repos, Versioning
        };

        static TableStorage()
        {
            Axioms = TableClient.GetTableReference("apisaxiom");
            Auth = TableClient.GetTableReference("apisauth");
            Catans = TableClient.GetTableReference("apiscatans");
            Messages = TableClient.GetTableReference("apismessages");
            Repos = TableClient.GetTableReference("apisrepos");
            Versioning = TableClient.GetTableReference("apisversion");

            Task.WaitAll(
                Axioms.CreateIfNotExistsAsync(),
                Auth.CreateIfNotExistsAsync(),
                Catans.CreateIfNotExistsAsync(),
                Messages.CreateIfNotExistsAsync(),
                Repos.CreateIfNotExistsAsync(),
                Versioning.CreateIfNotExistsAsync()
                );
        }
    }

    public static class TableStorageExtensions
    {
        /// <summary>
        /// A way to get a query's results easily without having to deal with the segmented querying every time.
        /// By default, maxes out at 100 results.
        /// </summary>
        /// <typeparam name="TEntity">Type to retrieve.</typeparam>
        /// <param name="query">The query.</param>
        /// <param name="maxResultCount">The number of results that will stop the segmented query. Since the number of results returned in a segmented query various, this is a soft maximum.</param>
        public static async Task<IEnumerable<TEntity>> ExecuteQueryAsync<TEntity>(this CloudTable @this, TableQuery<TEntity> query, int maxResultCount = 100)
            where TEntity : ITableEntity, new()
        {
            var results = new List<TEntity>();
            TableContinuationToken continuationToken = null;
            do
            {
                var result = await @this.ExecuteQuerySegmentedAsync<TEntity>(query, continuationToken);
                results.AddRange(result);
                continuationToken = result.ContinuationToken;
            } while (continuationToken != null && results.Count < maxResultCount);
            return results;
        }
    }
}