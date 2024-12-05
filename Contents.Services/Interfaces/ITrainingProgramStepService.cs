using Contents.Domain.Pagination;
using Contents.Domain.TrainingProgram;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contents.Services.Interfaces
{
    public interface ITrainingProgramStepService
    {
        int TotalFound { get; }

        Task<List<TrainingProgramStep>> GetAll(Guid trainingProgramId, PageRequest request);

        TrainingProgramStep Get(Guid trainingProgramId, Guid trainingProgramStepId);

        Task<TrainingProgramStep> Create(Guid trainingProgramId, TrainingProgramStep trainingProgramStep);

        Task<TrainingProgramStep> Update(Guid trainingProgramId, TrainingProgramStep trainingProgramStep);

        void Delete(Guid trainingProgramId, Guid stepId);
    }
}
