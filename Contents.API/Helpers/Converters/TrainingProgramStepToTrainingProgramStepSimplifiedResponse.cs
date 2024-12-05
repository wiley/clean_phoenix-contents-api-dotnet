using AutoMapper;
using Contents.API.Responses.TrainingProgram;
using Contents.Domain.TrainingProgram;
using System;
using System.Collections.Generic;

namespace Contents.API.Helpers.Converters
{
    public class TrainingProgramStepToTrainingProgramStepSimplifiedResponse : ITypeConverter<TrainingProgramStep, TrainingProgramStepSimplifiedResponse>
    {
        public TrainingProgramStepSimplifiedResponse Convert(TrainingProgramStep source, TrainingProgramStepSimplifiedResponse destination, ResolutionContext context)
        {
            if (source != null)
            {
                List<Guid> learningObjectIds = new List<Guid>();
                source.LearningObjects.ForEach(learningObject =>
                {
                    learningObjectIds.Add(learningObject.LearningObject.Id);
                });

                destination = new()
                {
                    Id = source.Id,
                    Title = source.Title,
                    Description = source.Description,
                    LearningObjects = learningObjectIds,
                    CreatedAt = source.CreatedAt,
                    UpdatedAt = source.UpdatedAt,
                    CreatedBy = source.CreatedBy,
                    UpdatedBy = source.UpdatedBy
                };
                return destination;
            }
            return null;
        }
    }
}
