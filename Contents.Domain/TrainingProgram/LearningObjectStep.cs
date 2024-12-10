using MongoDB.Bson.Serialization.Attributes;

namespace Contents.Domain.TrainingProgram
{
    public class LearningObjectStep
    {
        [BsonIgnoreIfNull]
        public LearningObject.LearningObject LearningObject { get; set; }

        [BsonIgnoreIfNull]
        public bool IsMandatory { get; set; }
    }
}
