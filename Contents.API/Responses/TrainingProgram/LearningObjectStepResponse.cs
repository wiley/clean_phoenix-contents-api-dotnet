using Contents.API.Responses.LearningObject;
using MongoDB.Bson.Serialization.Attributes;

namespace Contents.API.Responses.TrainingProgram
{
    public class LearningObjectStepResponse
    {
        public LearningObjectResponse LearningObject { get; set; }

        public bool IsMandatory { get; set; }
    }
}
