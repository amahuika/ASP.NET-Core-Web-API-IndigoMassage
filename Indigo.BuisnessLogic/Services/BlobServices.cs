using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;

namespace Indigo.BuisnessLogic.Services
{
    public class BlobServices : IBlobServices
    {
        private readonly BlobServiceClient _blobServiceClient;

        public BlobServices(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task<bool> DeleteBlob(string blobName, string containerName)
        {


            BlobContainerClient blobContainer = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = blobContainer.GetBlobClient(blobName);

            return await blobClient.DeleteIfExistsAsync();


        }

        public async Task<string> GetBlob(string blobName, string containerName)
        {
            BlobContainerClient blobContainer = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = blobContainer.GetBlobClient(blobName);

            return blobClient.Uri.AbsoluteUri;
        }

        public async Task<string> UploadBlob(string blobName, string containerName, IFormFile file)
        {
            BlobContainerClient blobContainer = _blobServiceClient.GetBlobContainerClient(containerName);

            BlobClient blobClient = blobContainer.GetBlobClient(blobName);

            var httpHeaders = new BlobHttpHeaders
            {
                ContentType = file.ContentType
            };


            var result = await blobClient.UploadAsync(file.OpenReadStream(), httpHeaders);

            if (result != null)
            {
                return await GetBlob(blobName, containerName);
            }




            return "";
        }
    }
}
