using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading.Tasks;

namespace ST10114423.Functions
{
    public static class UploadBlob
    {
        [Function("UploadBlob")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            // Get the container name and blob name from the query parameters
            string containerName = req.Query["containerName"];
            string blobName = req.Query["blobName"];

            // Check if the container name or blob name is empty or null
            if (string.IsNullOrEmpty(containerName) || string.IsNullOrEmpty(blobName))
            {
                // Return a bad request response with an error message
                return new BadRequestObjectResult("Container name and blob name must be provided.");
            }

            // Get the connection string from the environment variables
            var connectionString = Environment.GetEnvironmentVariable("AzureStorage:ConnectionString");

            // Create a new instance of BlobServiceClient using the connection string
            var blobServiceClient = new BlobServiceClient(connectionString);

            // Get the BlobContainerClient for the specified container name
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Create the container if it does not exist
            await containerClient.CreateIfNotExistsAsync();

            // Get the BlobClient for the specified blob name
            var blobClient = containerClient.GetBlobClient(blobName);

            // Upload the blob from the request body stream
            using var stream = req.Body;
            await blobClient.UploadAsync(stream, true);

            // Return an OK response with a success message
            return new OkObjectResult("Blob uploaded");
        }
    }
}
