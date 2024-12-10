using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Contents.API.Requests.TrainingProgram
{
    public class TrainingProgramStepCreateOrUpdateRequest
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public int EstimatedDuration { get; set; }

        public List<Guid> LearningObjects { get; set; }
    }
}
