using Microsoft.Azure.KeyVault;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;
using Unlimitedinf.Apis.Models.Axioms;
using Unlimitedinf.Tools;

namespace Unlimitedinf.Apis.AxiomsUpsert
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.PrintVerbosityLevel = false;
            Log.Verbosity = Log.VerbositySetting.Verbose;

            MainAsync().Wait();
        }

        private static async Task MainAsync()
        {
            var kvClient = GetKvClient();
            var axiomTable = await GetAxiomTableAsync(kvClient);

            foreach (var httpAxiom in Axioms.Http)
            {
                var retrieveOp = TableOperation.Retrieve(httpAxiom.PartitionKey, httpAxiom.RowKey);
                var retrieveRes = await axiomTable.ExecuteAsync(retrieveOp);
                Log.Ver($"{(retrieveRes.Result == null ? "Not Found:" : "Existing: ")} {httpAxiom.PartitionKey} {httpAxiom.RowKey}");

                if (retrieveRes.Result == null)
                {
                    var insertOp = TableOperation.Insert(httpAxiom);
                    var result = await axiomTable.ExecuteAsync(insertOp);
                    Log.Inf($"Inserted: {httpAxiom.PartitionKey} {httpAxiom.RowKey}");
                }
            }
        }

        private static KeyVaultClient GetKvClient()
        {
            const string applicationId = "754dd1b1-b115-4482-a3ec-3bec2ed8b152";
            string applicationSecret = Environment.GetEnvironmentVariable("AadTompostlerKvSecret");

            if (string.IsNullOrEmpty(applicationSecret))
            {
                Log.Err("AadTompostlerKvSecret environment variable not found!");
                Environment.Exit(1);
            }
            Log.Ver("Created keyvault client.");

            return new KeyVaultClient(async (authority, resource, scope) =>
            {
                var aadCreds = new ClientCredential(applicationId, applicationSecret);
                var authContext = new AuthenticationContext(authority);
                return (await authContext.AcquireTokenAsync(resource, aadCreds)).AccessToken;
            });
        }

        private static async Task<CloudTable> GetAxiomTableAsync(KeyVaultClient kvClient)
        {
            var connString = await kvClient.GetSecretAsync("https://tompostler.vault.azure.net/", "storage-unlimitedinfapis-key1-connectionstring");
            var tableClient = CloudStorageAccount.Parse(connString.Value).CreateCloudTableClient();
            var axiomTable = tableClient.GetTableReference("apisaxiom");
            await axiomTable.CreateIfNotExistsAsync();
            Log.Ver("Created axiom table client.");

            return axiomTable;
        }
    }
}
