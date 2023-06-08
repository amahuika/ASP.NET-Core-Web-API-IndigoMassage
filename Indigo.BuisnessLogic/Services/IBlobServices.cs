using Microsoft.AspNetCore.Http;

namespace Indigo.BuisnessLogic.Services
{
    public interface IBlobServices
    {
        // interface for blob services
        // upload image to blob storage
        // delete image from blob storage
        // get image from blob storage

        Task<string> GetBlob(string blobName, string containerName);
        Task<bool> DeleteBlob(string blobName, string containerName);
        Task<string> UploadBlob(string blobName, string containerName, IFormFile file);




    }
}
