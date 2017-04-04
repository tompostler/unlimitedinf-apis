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
        public static CloudTable Version { get; }

        static TableStorage()
        {
            Auth = TableClient.GetTableReference("apisauth");
            Version = TableClient.GetTableReference("apisversion");

            Task.WaitAll(
                Auth.CreateIfNotExistsAsync(),
                Version.CreateIfNotExistsAsync()
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