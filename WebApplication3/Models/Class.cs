using Azure.Storage.Blobs;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs.Models;
using Azure.Storage;
using System.Net.Mime;

namespace WebApplication3.Model
{
 

    // Class to interact with Azure Blob Storage
    public class BlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;

        // Constructor to initialize the BlobServiceClient
        public BlobStorageService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        // Upload a file to Azure Blob Storage
        public async Task UploadFileAsync(string containerName, string blobName, Stream fileStream)
        {
            try
            {
                // Get the container client
                BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

                // Create the container if it doesn't exist
                await containerClient.CreateIfNotExistsAsync();

                // Get the blob client
                BlobClient blobClient = containerClient.GetBlobClient(blobName);

                // Upload the file, overwrite if it already exists
                await blobClient.UploadAsync(fileStream, overwrite: true);
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it, rethrow it, etc.)
                throw new Exception("Error uploading file to Blob Storage: " + ex.Message);
            }
        }

        // Download a file from Azure Blob Storage
        public async Task<Stream> DownloadFileAsync(string containerName, string blobName)
        {
            try
            {
                // Get the container client
                BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

                // Get the blob client
                BlobClient blobClient = containerClient.GetBlobClient(blobName);

                // Download the file and return the stream
                BlobDownloadInfo download = await blobClient.DownloadAsync();
                return download.Content;
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it, rethrow it, etc.)
                throw new Exception("Error downloading file from Blob Storage: " + ex.Message);
            }
        }

        // List all blobs in a container
        public async Task<List<string>> ListBlobsAsync(string containerName)
        {
            List<string> blobNames = new List<string>();

            try
            {
                // Get the container client
                BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

                // List all blobs in the container
                await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
                {
                    blobNames.Add(blobItem.Name);
                }
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., log it, rethrow it, etc.)
                throw new Exception("Error listing blobs in Blob Storage: " + ex.Message);
            }

            return blobNames;
        }
    }

}
