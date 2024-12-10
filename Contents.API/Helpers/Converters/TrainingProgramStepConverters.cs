using Contents.API.Requests.TrainingProgram;
using Contents.Domain.LearningObject;
using Contents.Domain.TrainingProgram;
using Contents.Services.Interfaces;
using System.Collections.Generic;

namespace Contents.API.Helpers.Converters
{
    public class TrainingProgramStepConverters
    {
        private readonly ILearningObjectService _learningObjectService;

        public TrainingProgramStepConverters(ILearningObjectService learningObjectService)
        {
            _learningObjectService = learningObjectService;
        }

        public TrainingProgramStep Convert(TrainingProgramStepCreateOrUpdateRequest request)
        {
            TrainingProgramStep step = new()
            {
                Title = request.Title,
                Description = request.Description,
                EstimatedDuration = request.EstimatedDuration,
                LearningObjects = new List<LearningObjectStep>()
            };
            request.LearningObjects?.ForEach(learningObjectId =>
            {
                LearningObject learningObject = _learningObjectService.Get(learningObjectId);
                if (learningObject != null)
                {
                    step.LearningObjects.Add(new LearningObjectStep
                    {
                        LearningObject = learningObject,
                        IsMandatory = true
                    });
                }
            });

            return step;
        }
    }
}
