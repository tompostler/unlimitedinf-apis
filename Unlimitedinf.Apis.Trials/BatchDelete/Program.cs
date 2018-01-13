using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Unlimitedinf.Tools;

namespace BatchDelete
{
    internal static class Program
    {
        static Random random = new Random();
        private const string connectionString = "UseDevelopmentStorage=true";
        //private const string connectionString = "UseDevelopmentStorage=true";
        static CloudTable TS = CloudStorageAccount.Parse(connectionString).CreateCloudTableClient().GetTableReference("trialsbatchdelete");

        static async Task Main(string[] args)
        {
            Entity.DefaultPartitionKey = Guid.NewGuid();
            Log.ConfigureDefaultConsoleApp();

            Log.Inf("Creating table...");
            await TS.CreateIfNotExistsAsync();
            Log.Inf("Table is created.");
            Log.Line();

            var entityCount = random.Next(3_000, 5_000);
            entityCount = 150;
            Log.Inf($"Creating {entityCount} entities...");

            await Insert(entityCount);
            entityCount = 200;
            await Delete(entityCount);
        }

        private static async Task Insert(int entityCount)
        {
            var entities = new List<Entity>(entityCount);
            for (int i = 0; i < entityCount; i++)
                entities.Add(new Entity(i));
            Log.Inf($"Created {entities.Count} entities.");
            Log.Line();
            var completed = 0;

            var sw = new Stopwatch();
            while (completed < entities.Count)
            {
                var batchEnts = entities.Skip(completed).Take(100).ToList();
                completed += batchEnts.Count;
                var batch = new TableBatchOperation();
                foreach (var batchEnt in batchEnts)
                    batch.Insert(batchEnt);
                Log.Inf("Created batch.");

                Log.Inf($"Sending batch of {completed - batchEnts.Count} to {completed}...");
                var bsw = Stopwatch.StartNew();
                sw.Start();
                var results = await TS.ExecuteBatchAsync(batch);
                sw.Stop();
                Log.Inf($"Sent after {bsw.ElapsedMilliseconds}ms. Results: {JsonConvert.SerializeObject(results.GroupBy(_ => _.HttpStatusCode).Select(_ => new { code = _.Key, count = _.Count() }).ToDictionary(_ => _.code, _ => _.count))}");
                Log.Line();
            }
            Log.Inf($"Created {completed} entities in TS over {sw.Elapsed.TotalSeconds:0.0}s.");
            Log.Inf($"Avg time per entity: {sw.Elapsed.TotalMilliseconds / completed:0.0}ms");
            Log.Inf($"Avg time per 100 batch: {sw.Elapsed.TotalSeconds / completed * 100:0.00}ms");
            Log.Line();
        }

        private static async Task Delete(int entityCount)
        {
            var sw = new Stopwatch();
            for (int entBatchStart = 0; entBatchStart < entityCount; entBatchStart += 100)
            {
                var batch = new TableBatchOperation();
                for (int i = entBatchStart; i < Math.Min(entBatchStart + 100, entityCount); i++)
                    //batch.Retrieve<Entity>(Entity.DefaultPartitionKey.ToString(), i.ToString(), new List<string> { "RowKey", "PartitionKey" });
                    batch.Delete(new Entity(i) { ETag = "*" });

                Log.Inf($"Sending batch of {entBatchStart} to {entBatchStart + batch.Count}...");
                try
                {
                    var bsw = Stopwatch.StartNew();
                    sw.Start();
                    var results = await TS.ExecuteBatchAsync(batch);
                    sw.Stop();
                    Log.Inf($"Sent after {bsw.ElapsedMilliseconds}ms. Results: {JsonConvert.SerializeObject(results.GroupBy(_ => _.HttpStatusCode).Select(_ => new { code = _.Key, count = _.Count() }).ToDictionary(_ => _.code, _ => _.count))}");
                    Log.Line();
                }
                catch (Exception ex)
                {
                    if (Debugger.IsAttached)
                        Debugger.Break();
                    throw;
                }
            }
            Log.Inf($"Deleted {entityCount} entities in TS over {sw.Elapsed.TotalSeconds:0.0}s.");
            Log.Inf($"Avg time per entity: {sw.Elapsed.TotalMilliseconds / entityCount:0.0}ms");
            Log.Inf($"Avg time per 100 batch: {sw.Elapsed.TotalSeconds / entityCount * 100:0.00}ms");
            Log.Line();
        }
    }
}
