using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Course.Domain.Services.Azure;
using Course.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Course.Infrastructure.Services.Azure
{
    public class StorageService : IStorageService
    {
        private readonly BlobServiceClient _blobClient;

        public StorageService(BlobServiceClient blobClient) => _blobClient = blobClient;

        public async Task<string> GetCourseImage(Guid courseIdentifier, string image)
        {
            var container = _blobClient.GetBlobContainerClient(courseIdentifier.ToString());

            if(await container.ExistsAsync() == false) return "";

            var blob = container.GetBlobClient(image);

            if (await container.ExistsAsync() == false) return "";

            var sasBuilder = new BlobSasBuilder()
            {
                BlobName = image,
                BlobContainerName = courseIdentifier.ToString(),
                ExpiresOn = DateTime.UtcNow.AddMinutes(40),
                Resource = "b"
            };
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            return blob.GenerateSasUri(sasBuilder).ToString();
        }

        public async Task UploadCourseImage(Stream image, string imageName, Guid courseIdentifier)
        {
            var container = _blobClient.GetBlobContainerClient(courseIdentifier.ToString());

            await container.CreateIfNotExistsAsync();

            var blobClient = container.GetBlobClient(imageName);
            await blobClient.UploadAsync(image, overwrite: true);
        }
    }
}
