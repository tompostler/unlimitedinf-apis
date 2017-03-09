using Microsoft.Azure;
using Microsoft.Web.Http;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Unlimitedinf.Apis.Controllers.V1
{
    [ApiVersion("1.0")]
    public class ValuesController : ApiController
    {
        // GET api/values
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

        // GET api/values/5
        public string Get(int id)
        {
            return id.ToString();
        }

        // POST api/values
        public void Post([FromBody]string value)
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
                if (table.ExecuteQuery(query).Count() > 10)
                    table.DeleteIfExists();
            }

            // Create the table if it doesn't exist.
            table.CreateIfNotExists();

            TableOperation insert = TableOperation.Insert(new DemoButt());
            table.Execute(insert);
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

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
