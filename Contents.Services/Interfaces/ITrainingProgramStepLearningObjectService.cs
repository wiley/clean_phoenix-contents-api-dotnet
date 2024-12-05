using Contents.Domain.Pagination;
using Contents.Domain.TrainingProgram;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contents.Services.Interfaces
{
    public interface ITrainingProgramStepLearningObjectService
    {
        int TotalFound { get; }

        Task<List<LearningObjectStep>> GetAll(Guid trainingProgramId, Guid stepId, PageRequest request);

        LearningObjectStep Get(Guid trainingProgramId, Guid stepId, Guid Id);

        Task<LearningObjectStep> Create(Guid trainingProgramId, Guid stepId, LearningObjectStep learningObjectStep);

        Task<LearningObjectStep> Update(Guid trainingProgramId, Guid stepId, LearningObjectStep learningObjectStep);

        void Delete(Guid trainingProgramId, Guid stepId, Guid Id);
    }
}
