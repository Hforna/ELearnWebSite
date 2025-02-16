using Course.Domain.Entitites;
using Course.Domain.Repositories;
using Course.Exception;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Infrastructure.Data.VideoD
{
    public class VideoRepository : IVideoReadOnly, IVideoWriteOnly
    {
        private readonly VideoDbContext _dbContext;

        public VideoRepository(VideoDbContext dbContext) => _dbContext = dbContext;

        public async Task AddVideo(Video video)
        {
            await _dbContext.Videos.InsertOneAsync(video);
        }

        public async Task DeleteVideo(string id)
        {
            await GetVideo(id);

            await _dbContext.Videos.DeleteOneAsync(id);
        }

        public async Task<Video?> VideoById(string id)
        {
            var video = await GetVideo(id);

            return await video.FirstOrDefaultAsync();
        }

        private async Task<IAsyncCursor<Video?>> GetVideo(string id) => await _dbContext.Videos.FindAsync(id);
    }
}
