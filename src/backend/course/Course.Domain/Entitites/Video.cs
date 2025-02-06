using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.Domain.Entitites
{
    public class Video
    {
        [BsonId]
        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("thumbnailName")]
        public string? thumbnailName { get; set; }
        [BsonElement("isTranscoded")]
        public bool IsTranscoded { get; set; } = false;
        [BsonElement("createdOn")]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
