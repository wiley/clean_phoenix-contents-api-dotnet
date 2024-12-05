using Contents.Domain.TrainingProgram;
using System.Collections.Generic;

namespace Contents.API.Responses.TrainingProgram
{
    public class TrainingProgramStepResponse: GenericEntityResponse
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public int EstimatedDuration { get; set; }

        public List<LearningObjectStepResponse> LearningObjects { get; set; }
    }
}
