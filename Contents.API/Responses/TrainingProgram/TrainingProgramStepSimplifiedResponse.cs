using System;
using System.Collections.Generic;

namespace Contents.API.Responses.TrainingProgram
{
    public class TrainingProgramStepSimplifiedResponse : GenericEntityResponse
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public int EstimatedDuration { get; set; }

        public List<Guid> LearningObjects { get; set; }
    }
}
