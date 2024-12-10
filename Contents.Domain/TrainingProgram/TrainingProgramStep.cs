using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Contents.Domain.TrainingProgram
{
    public class TrainingProgramStep : GenericEntity
    {
        [BsonIgnoreIfNull]
        public string Title { get; set; }

        [BsonIgnoreIfNull]
        public string Description { get; set; }

        public int EstimatedDuration { get; set; }

        [BsonIgnoreIfNull]
        public List<LearningObjectStep> LearningObjects { get; set; }
    }
}
