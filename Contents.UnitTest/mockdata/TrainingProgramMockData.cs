using Contents.Domain.TrainingProgram;
using System;
using System.Collections.Generic;

namespace Contents.UnitTest.MockData
{
    public static class TrainingProgramMockData
    {
        public static List<TrainingProgram> GetTrainingProgramListData()
        {
            List<TrainingProgram> trainingPrograms = new List<TrainingProgram>
            {
                GenerateTrainingProgram(Guid.Parse("3fa35f64-5717-4562-b3fc-2c963f66afa6")),
                GenerateTrainingProgram(Guid.Parse("c3008b5b-0a73-4def-8496-ea850f154d9a"))
            };

            return trainingPrograms;
        }

        public static TrainingProgram GetTrainingProgram(Guid id)
        {
            return GenerateTrainingProgram(id);
        }

        private static TrainingProgram GenerateTrainingProgram(Guid id)
        {
            return new TrainingProgram
            {

                Id = id,
                Title = "My Title - Training Program",
                Description = "My Description - Training Program",
                EstimatedDuration = 4600,
                LanguageTag = "pt-BR",
                ReferenceCode = "REF001",
                ThumbnailPath = "path/to/thumnail.png",
                OrganizationIds = new List<int> { 534, 123, 654, 1092381 },
                ProductIds = new List<Guid> { id },
                IsDiscoverable = true,
                Tags = new List<string> { "Java", "C#", "Communication" },
                Authors = new List<string> { "Mark Scullard", "Frank Sinatra" },
                Steps = new List<TrainingProgramStep>(),
                CreatedBy = 1231,
                UpdatedBy = 123019,
                CreatedAt = DateTime.Parse("2023-02-22T21:40:18.243Z"),
                UpdatedAt = DateTime.Parse("2023-02-22T21:40:18.243Z")
            };
        }

        public static List<TrainingProgramStep> GetStepListData()
        {
            List<TrainingProgramStep> steps = new List<TrainingProgramStep>
            {
                GenerateStep(Guid.Parse("3fa35f64-5717-4562-b3fc-2c963f66afa6")),
                GenerateStep(Guid.Parse("c3008b5b-0a73-4def-8496-ea850f154d9a"))
            };

            return steps;
        }

        public static TrainingProgramStep GetStep(Guid id)
        {
            return GenerateStep(id);
        }

        private static TrainingProgramStep GenerateStep(Guid id)
        {
            return new TrainingProgramStep
            {
                Id = id,
                Title = "My Title - Step",
                Description = "My Description - Step ",
                EstimatedDuration = 4600,
                LearningObjects = new List<LearningObjectStep>(),
                CreatedBy = 1231,
                UpdatedBy = 123019,
                CreatedAt = DateTime.Parse("2023-02-22T21:40:18.243Z"),
                UpdatedAt = DateTime.Parse("2023-02-22T21:40:18.243Z")
            };
        }

        public static List<LearningObjectStep> GetLearningObjectStepListData()
        {
            List<LearningObjectStep> learningObjects = new List<LearningObjectStep>
            {
                GenerateLearningObjectStep(Guid.NewGuid()),
                GenerateLearningObjectStep(Guid.NewGuid())
            };

            return learningObjects;
        }

        public static LearningObjectStep GetLearningObjectStep(Guid id)
        {
            return GenerateLearningObjectStep(id);
        }

        private static LearningObjectStep GenerateLearningObjectStep(Guid id)
        {
            return new LearningObjectStep
            {
                LearningObject = LearningObjectMockData.GetLearningObject(id),
                IsMandatory = true
            };
        }
    }
}
