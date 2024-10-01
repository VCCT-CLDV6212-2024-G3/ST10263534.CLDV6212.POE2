using Azure.Data.Tables;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ST10114423.Functions
{
    public static class StoreTableInfo
    {
        [Function("StoreTableInfo")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // Retrieve the values from the request query parameters
            string tableName = req.Query["tableName"];
            string partitionKey = req.Query["partitionKey"];
            string rowKey = req.Query["rowKey"];
            string data = req.Query["data"];

            // Check if any of the required parameters are missing or empty
            if (string.IsNullOrEmpty(tableName) || string.IsNullOrEmpty(partitionKey) || string.IsNullOrEmpty(rowKey) || string.IsNullOrEmpty(data))
            {
                return new BadRequestObjectResult("Table name, partition key, row key, and data must be provided.");
            }

            // Retrieve the connection string from the environment variables
            var connectionString = Environment.GetEnvironmentVariable("AzureStorage:ConnectionString");

            // Create a new instance of the TableServiceClient using the connection string
            var serviceClient = new TableServiceClient(connectionString);

            // Get the table client for the specified table name
            var tableClient = serviceClient.GetTableClient(tableName);

            // Create the table if it does not exist
            await tableClient.CreateIfNotExistsAsync();

            // Create a new TableEntity with the provided partition key, row key, and data
            var entity = new TableEntity(partitionKey, rowKey) { ["Data"] = data };

            // Add the entity to the table
            await tableClient.AddEntityAsync(entity);

            // Return a successful response
            return new OkObjectResult("Data added to table");
        }
    }
}
