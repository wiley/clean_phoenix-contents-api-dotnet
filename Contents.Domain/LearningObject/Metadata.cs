using MongoDB.Bson.Serialization.Attributes;

namespace Contents.Domain.LearningObject
{
    public class Metadata
    {
        [BsonIgnoreIfNull]
        public Filepaths Filepaths { get; set; }

        [BsonIgnoreIfNull]
        public string Filepath { get; set; }

        [BsonIgnoreIfNull]
        public string Url { get; set; }
    }
}
