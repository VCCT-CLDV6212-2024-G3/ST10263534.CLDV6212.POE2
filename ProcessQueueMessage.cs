using Azure.Storage.Queues;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ST10114423.Functions
{
    public static class ProcessQueueMessage
    {
        [Function("ProcessQueueMessage")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // Get the queue name and message from the request query parameters
            string queueName = req.Query["queueName"];
            string message = req.Query["message"];

            // Check if the queue name or message is empty
            if (string.IsNullOrEmpty(queueName) || string.IsNullOrEmpty(message))
            {
                return new BadRequestObjectResult("Queue name and message must be provided.");
            }

            // Get the connection string from the environment variables
            var connectionString = Environment.GetEnvironmentVariable("AzureStorage:ConnectionString");

            // Create a new instance of QueueServiceClient using the connection string
            var queueServiceClient = new QueueServiceClient(connectionString);

            // Get the queue client for the specified queue name
            var queueClient = queueServiceClient.GetQueueClient(queueName);

            // Create the queue if it doesn't exist
            await queueClient.CreateIfNotExistsAsync();

            // Send the message to the queue
            await queueClient.SendMessageAsync(message);

            // Return a success response
            return new OkObjectResult("Message added to queue");
        }
    }
}
