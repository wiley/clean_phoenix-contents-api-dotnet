using Contents.API.Requests.TrainingProgram;
using Contents.Domain.LearningObject;
using Contents.Domain.TrainingProgram;
using Contents.Services.Interfaces;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Contents.API.Helpers.Converters
{
    public class TrainingProgramConverters
    {
        private readonly ITrainingProgramStepService _trainingProgramStepService;

        public TrainingProgramConverters(ITrainingProgramStepService trainingProgramStepService)
        {
            _trainingProgramStepService = trainingProgramStepService;
        }

        public TrainingProgram ConvertUpdate(Guid trainingProgramId, TrainingProgramUpdateRequest request)
        {
            TrainingProgram trainingProgram = new()
            {
       
                Title = request.Title,
                Description = request.Description,
                LanguageTag = request.LanguageTag,
                ReferenceCode = request.ReferenceCode,
                ThumbnailPath = request.ThumbnailPath,
                OrganizationIds = request.OrganizationIds,
                ProductIds = request.ProductIds,
                IsDiscoverable = request.IsDiscoverable,
                Tags = request.Tags,
                Authors = request.Authors,
                Steps = new List<TrainingProgramStep>()
            };
            request.StepIds?.ForEach(stepId =>
            {
                TrainingProgramStep step = _trainingProgramStepService.Get(trainingProgramId, stepId);
                if (step != null)
                {
                    trainingProgram.Steps.Add(step);
                }
            });

            return trainingProgram;
        }
    }
}
