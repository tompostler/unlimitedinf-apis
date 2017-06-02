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

        public static CloudTable Auth { get; }
        public static CloudTable Versioning { get; }
        public static CloudTable Random { get; }

        public static List<CloudTable> AllTables => new List<CloudTable>
        {
            Auth, Versioning, Random
        };

        static TableStorage()
        {
            Auth = TableClient.GetTableReference("apisauth");
            Versioning = TableClient.GetTableReference("apisversion");
            Versioning = TableClient.GetTableReference("apisrandom");

            Task.WaitAll(
                Auth.CreateIfNotExistsAsync(),
                Versioning.CreateIfNotExistsAsync(),
                Random.CreateIfNotExistsAsync()
                );
        }
    }

    public static class TableStorageExtensions
    {
        public static async Task<IEnumerable<TEntity>> ExecuteQueryAsync<TEntity>(this CloudTable @this, TableQuery<TEntity> query) where TEntity : ITableEntity, new()
        {
            var results = new List<TEntity>();
            TableContinuationToken continuationToken = null;
            do
            {
                var result = await @this.ExecuteQuerySegmentedAsync<TEntity>(query, continuationToken);
                results.AddRange(result);
                continuationToken = result.ContinuationToken;
            } while (continuationToken != null);
            return results;
        }
    }
}