using MongoDB.Bson.Serialization.Attributes;

namespace Contents.Domain.LearningObject
{
    public class Filepaths
    {
        [BsonIgnoreIfNull]
        public string VideoHighQuality { get; set; }

        [BsonIgnoreIfNull]
        public string VideoLowQuality { get; set; }

        [BsonIgnoreIfNull]
        public string Subtitles { get; set; }
    }
}
