using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Services.Azure
{
    public interface IStorageService
    {
        public Task UploadCourseImage(Stream image, string imageName, Guid courseIdentifier);
        public Task<string> GetCourseImage(Guid courseIdentifier, string image);
        public Task DeleteVideo(Guid courseIdentifier, string videoId);
        public Task DeleteThumbnailVideo(string videoId);
        public Task DeleteCourseImage(Guid courseIdentifier, string image);
    }
}
