using Azure.Storage.Blobs;
using Course.Domain.Services.Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Infrastructure.Services.Azure
{
    public class StorageService : IStorageService
    {
        private readonly BlobServiceClient _blobClient;

        public StorageService(BlobServiceClient blobClient) => _blobClient = blobClient;

        public async Task UploadCourseImage(Stream image, string imageName, Guid courseIdentifier)
        {
            var container = _blobClient.GetBlobContainerClient(courseIdentifier.ToString());

            await container.CreateIfNotExistsAsync();

            var blobClient = container.GetBlobClient(imageName);
            await blobClient.UploadAsync(image, overwrite: true);
        }
    }
}
