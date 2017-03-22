using Microsoft.Azure;
using Microsoft.Web.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Http;
using Unlimitedinf.Apis.Auth;

namespace Unlimitedinf.Apis.Controllers.v1
{
    [RequireHttps, ApiVersion("1.0", Deprecated = true)]
    [RoutePrefix("api/values")]
    public class ValuesController : ApiController
    {
        // GET api/values
        [Route, HttpGet]
        public IEnumerable<DemoButt> Get()
        {
            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("unlimitedinfapis_AzureStorageConnectionString"));

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("demobutt");

            // Check how many we've got
            if (table.Exists())
            {
                TableQuery<DemoButt> query = new TableQuery<DemoButt>();
                return table.ExecuteQuery(query);
            }

            return new List<DemoButt>();
        }

        private static List<DateTime> LetsSeeHowLongThisLasts = new List<DateTime>();

        // GET api/values/5
        [Route, HttpGet]
        public List<DateTime> Get(int id)
        {
            LetsSeeHowLongThisLasts.Add(DateTime.Now);
            return LetsSeeHowLongThisLasts;
        }

        // POST api/values
        [Route, HttpPost]
        public async Task Post(DemoButt value)
        {
            // Retrieve the storage account from the connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(
                CloudConfigurationManager.GetSetting("unlimitedinfapis_AzureStorageConnectionString"));

            // Create the table client.
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();

            // Retrieve a reference to the table.
            CloudTable table = tableClient.GetTableReference("demobutt");

            // 10% chance of dropping the table
            if (new Random().Next() % 10 == 0)
            {
                Trace.TraceInformation("Dropping table");
                await table.DeleteIfExistsAsync();
            }

            // Create the table if it doesn't exist.
            await table.CreateIfNotExistsAsync();

            TableOperation insert = TableOperation.Insert(new DemoButt());
            await table.ExecuteAsync(insert);
        }

        public class DemoButt : TableEntity
        {
            public DemoButt()
            {
                this.PartitionKey = Guid.NewGuid().ToString();
                this.RowKey = Guid.NewGuid().ToString();
                this.Insertion = DateTime.Now;
            }

            public DateTime Insertion { get; set; }
        }
    }
}
