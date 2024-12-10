using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Contents.Domain.Skill
{
    [BsonCollection("skill")]
    [BsonIgnoreExtraElements]
    public class Skill : GenericEntity
    {
        [BsonIgnoreIfNull]
        public string Name { get; set; }

        [BsonIgnoreIfNull]
        public string Description { get; set; }

        [BsonIgnoreIfNull]
        public Guid CategoryId { get; set; }

        [BsonIgnoreIfNull]
        public Guid PublisherId { get; set; }
    }
}
