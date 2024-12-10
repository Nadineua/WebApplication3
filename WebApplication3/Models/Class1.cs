using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace WebApplication3.Model
{
    public class BlobDto
    {
        public string? Name { get; set; }
        public string? Uri { get; set; }
        public string? ContentType { get; set; }
        public Stream? Content { get; set; }
    }

    public class BlobResponseDto
    {
        public BlobResponseDto()
        {
            Blob = new BlobDto();
        }
        public string? Status { get; set; }
        public bool error { get; set; }
        public BlobDto Blob { get; set; }
    }

    public class FileService
    {
        private readonly string _storageAccount = "finalstorage12345";
        private readonly string _key = "vgxJqO/rziLICual4/dWEPryOMgE8FCifC0B0hFn28FDifSEx+YtwHvDyLgikRD5h6VoL0IxdL4N+AStw3G/5w==";
        private readonly BlobContainerClient _filesContainer;

        public FileService()
        {
            // Key Vault and BlobServiceClient setup
            var keyVaultUri = "https://finalkeyvault.vault.azure.net/";
            var secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());

            
            KeyVaultSecret storageSecret = secretClient.GetSecret("storagekey");
            _key = storageSecret.Value;
            var credential = new StorageSharedKeyCredential(_storageAccount, _key);
            var blobUri = $"https://{_storageAccount}.blob.core.windows.net"; // Changed to HTTPS for security
            var blobServiceClient = new BlobServiceClient(new Uri(blobUri), credential);
            _filesContainer = blobServiceClient.GetBlobContainerClient("test");

            // Ensure the container exists
            _filesContainer.CreateIfNotExists(PublicAccessType.Blob);
        }


        public async Task<List<BlobDto>> ListAsync()
        {
            List<BlobDto> files = new List<BlobDto>();

            // Enumerate blobs in the container
            await foreach (var file in _filesContainer.GetBlobsAsync())
            {
                string uri = _filesContainer.Uri.ToString();
                var name = file.Name;
                var fullUri = $"{uri}/{name}";

                files.Add(new BlobDto
                {
                    Uri = fullUri,
                    Name = name,
                    ContentType = file.Properties.ContentType
                });
            }

            return files; // Return after the loop finishes
        }

        public async Task<BlobResponseDto> UploadAsync(IFormFile blob)
        {
            BlobResponseDto response = new BlobResponseDto();

            BlobClient client = _filesContainer.GetBlobClient(blob.FileName);
            try
            {


                await using (Stream? data = blob.OpenReadStream())
                {
                    await client.UploadAsync(data, true); // Overwrite if the blob already exists
                }

                response.Status = $"File {blob.FileName} uploaded successfully.";
                response.error = false;
                response.Blob = new BlobDto
                {
                    Uri = client.Uri.AbsoluteUri,
                    Name = client.Name
                };
            }
            catch (Exception ex)
            {
                response.Status = $"Error uploading file: {ex.Message}";
                response.error = true;
            }

            return response;
        }
        public async Task<BlobDto> DownloadAsync(string blobFilename)
        {
            BlobClient file = _filesContainer.GetBlobClient(blobFilename);
            if (await file.ExistsAsync())
            {
                var data = await file.OpenReadAsync();
                Stream blobContent = data;
                var content = await file.DownloadContentAsync();
                string name = blobFilename;
                string contentType = content.Value.Details.ContentType;
                return new BlobDto { Content = blobContent, Name = name, ContentType = contentType };
            }
            return null;
        }
        public async Task<BlobResponseDto> DeleteAsync(string blobFilename)
        {
            BlobClient file = _filesContainer.GetBlobClient(blobFilename);
            await file.DeleteAsync();
            return new BlobResponseDto { error = false, Status = $"File:{blobFilename} has been deleted" };
        }
    }


}
