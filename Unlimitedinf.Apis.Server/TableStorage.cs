using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Unlimitedinf.Apis.Server
{
    public class TableStorage
    {
        private readonly CloudTableClient TableClient;

        public static TableStorage Instance { get; private set; }

        public CloudTable Axioms { get; }
        public CloudTable Auth { get; }
        public CloudTable Catans { get; }
        public CloudTable Messages { get; }
        public CloudTable Frequencies { get; }
        public CloudTable Repos { get; }
        public CloudTable Versioning { get; }

        public List<CloudTable> AllTables => new List<CloudTable>
        {
            Axioms, Auth, Catans, Repos, Versioning
        };

        public TableStorage(IConfiguration config)
        {
            this.TableClient = CloudStorageAccount.Parse(config.GetConnectionString("unlimitedinfapis_AzureStorageConnectionString")).CreateCloudTableClient();

            this.Axioms = this.TableClient.GetTableReference("apisaxiom");
            this.Auth = this.TableClient.GetTableReference("apisauth");
            this.Catans = this.TableClient.GetTableReference("apiscatans");
            this.Frequencies = this.TableClient.GetTableReference("apisfrequencies");
            this.Messages = this.TableClient.GetTableReference("apismessages");
            this.Repos = this.TableClient.GetTableReference("apisrepos");
            this.Versioning = this.TableClient.GetTableReference("apisversion");

            Task.WaitAll(
                this.Axioms.CreateIfNotExistsAsync(),
                this.Auth.CreateIfNotExistsAsync(),
                this.Catans.CreateIfNotExistsAsync(),
                this.Frequencies.CreateIfNotExistsAsync(),
                this.Messages.CreateIfNotExistsAsync(),
                this.Repos.CreateIfNotExistsAsync(),
                this.Versioning.CreateIfNotExistsAsync()
                );

            TableStorage.Instance = this;
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