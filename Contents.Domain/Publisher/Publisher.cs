using MongoDB.Bson.Serialization.Attributes;

namespace Contents.Domain.Publisher
{
    [BsonCollection("publisher")]
    [BsonIgnoreExtraElements]
    public class Publisher : GenericEntity
    {
        [BsonIgnoreIfNull]
        public string Name { get; set; }
    }
}
