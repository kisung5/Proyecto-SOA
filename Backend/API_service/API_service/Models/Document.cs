using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace API_service.Models
{
    public class Document
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        public double OffensivePercentage { get; set; }
        
        public double SentimentPercentage { get; set; }
    }
}
