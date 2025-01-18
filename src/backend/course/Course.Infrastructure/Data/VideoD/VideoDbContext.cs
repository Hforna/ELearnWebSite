using Course.Domain.Entitites;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Infrastructure.Data.VideoD
{
    public class VideoDbContext
    {
        private IMongoDatabase _database;

        public VideoDbContext(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("mongo");

            var url = MongoUrl.Create(connectionString);
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(url.DatabaseName);
        }

        public IMongoCollection<Video> Videos => _database.GetCollection<Video>("videos");
    }
}
