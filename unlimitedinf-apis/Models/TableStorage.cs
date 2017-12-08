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
        public static CloudTable NotesCatans { get; }
        public static CloudTable NotesRepos { get; }
        public static CloudTable Versioning { get; }

        public static List<CloudTable> AllTables => new List<CloudTable>
        {
            Axioms, Auth, NotesCatans, NotesRepos, Versioning
        };

        static TableStorage()
        {
            Axioms = TableClient.GetTableReference("apisaxiom");
            Auth = TableClient.GetTableReference("apisauth");
            NotesCatans = TableClient.GetTableReference("apisnotescatans");
            NotesRepos = TableClient.GetTableReference("apisnotesrepos");
            Versioning = TableClient.GetTableReference("apisversion");

            Task.WaitAll(
                Axioms.CreateIfNotExistsAsync(),
                Auth.CreateIfNotExistsAsync(),
                NotesCatans.CreateIfNotExistsAsync(),
                NotesRepos.CreateIfNotExistsAsync(),
                Versioning.CreateIfNotExistsAsync()
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