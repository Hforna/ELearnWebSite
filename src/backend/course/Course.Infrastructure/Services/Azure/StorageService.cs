using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Course.Domain.Services.Azure;
using Course.Exception;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Course.Infrastructure.Services.Azure
{
    public class StorageService : IStorageService
    {
        private readonly BlobServiceClient _blobClient;

        public StorageService(BlobServiceClient blobClient) => _blobClient = blobClient;

        public async Task DeleteCourseImage(Guid courseIdentifier, string image)
        {
            var container = _blobClient.GetBlobContainerClient(courseIdentifier.ToString());

            if (await container.ExistsAsync())
                throw new NotFoundException(ResourceExceptMessages.INVALID_COURSE_CONTAINER);

            var blob = container.GetBlobClient(image);
            await blob.DeleteIfExistsAsync();
        }

        public async Task DeleteContainer(Guid courseIdentifier)
        {
            var container = _blobClient.GetBlobContainerClient(courseIdentifier.ToString());

            await container.DeleteIfExistsAsync();
        }

        public async Task DeleteThumbnailVideo(string videoId)
        {
            await _blobClient.DeleteBlobContainerAsync(videoId);
        }

        public async Task DeleteVideo(Guid courseIdentifier, string videoId)
        {
            var container = _blobClient.GetBlobContainerClient(courseIdentifier.ToString());

            if (await container.ExistsAsync())
                throw new NotFoundException(ResourceExceptMessages.INVALID_COURSE_CONTAINER);

            await container.DeleteBlobIfExistsAsync(videoId);
        }

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

        public async Task<string> GetVideo(Guid courseIdentifier, string videoId)
        {
            var container = _blobClient.GetBlobContainerClient(courseIdentifier.ToString());

            if (await container.ExistsAsync() == false) return "";

            var blob = container.GetBlobClient(videoId);

            if (await container.ExistsAsync() == false) return "";

            var sasBuilder = new BlobSasBuilder()
            {
                BlobName = videoId,
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

        public async Task UploadThumbnailVideo(string videoId, string thumbnailName, Stream image)
        {
            var container = _blobClient.GetBlobContainerClient(videoId);
            await container.CreateIfNotExistsAsync();

            var blobClient = container.GetBlobClient(thumbnailName);
            await blobClient.UploadAsync(image, overwrite: true);
        }

        public async Task UploadVideo(Guid courseIdentifier, Stream video, string videoId)
        {
            var container = _blobClient.GetBlobContainerClient(courseIdentifier.ToString());
            await container.CreateIfNotExistsAsync();

            var blobClient = container.GetBlobClient(videoId);
            await blobClient.UploadAsync(video, overwrite: true);
        }
    }
}
