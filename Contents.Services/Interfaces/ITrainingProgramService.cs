using Contents.Domain.Pagination;
using Contents.Domain.TrainingProgram;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contents.Services.Interfaces
{
    public interface ITrainingProgramService
    {
        int TotalFound { get; }

        Task<List<TrainingProgram>> GetAll(PageRequest request);

        TrainingProgram Get(Guid id);

        Task<List<TrainingProgram>> SearchByOrganizationId(PageRequest request, int? organizationId);

        Task Create(TrainingProgram trainingProgram);

        Task<TrainingProgram> Update(Guid Id, TrainingProgram trainingProgram);

        Task<TrainingProgram> Delete(Guid Id);

        Task GenerateKafkaEvents();

        
    }
}
