using Contents.Domain.Pagination;
using Contents.Domain.TrainingProgram;
using Contents.Infrastructure.Interface.Mongo;
using Contents.Services.Interfaces;
using DarwinAuthorization.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contents.Services
{
    public class TrainingProgramStepService: ITrainingProgramStepService
    {
        private readonly IMongoRepository<TrainingProgram> _repository;
        private readonly IPaginationService<TrainingProgramStep> _paginationService;
        private readonly ILogger<TrainingProgramStepService> _logger;
        private readonly ITrainingProgramService _trainingProgramService;
        private readonly DarwinAuthorizationContext _authorizationContext;

        public TrainingProgramStepService(
            IMongoRepository<TrainingProgram> repository,
            ILogger<TrainingProgramStepService> logger,
            IPaginationService<TrainingProgramStep> paginationService,
            ITrainingProgramService trainingProgramService,
            DarwinAuthorizationContext authorizationContext)
        {
            _repository = repository;
            _logger = logger;
            _paginationService = paginationService;
            _authorizationContext = authorizationContext;
            _trainingProgramService = trainingProgramService;
        }

        public int TotalFound => _paginationService.TotalRecords;

        public async Task<List<TrainingProgramStep>> GetAll(Guid trainingProgramId, PageRequest request)
        {
            TrainingProgram trainingProgram = _trainingProgramService.Get(trainingProgramId);
            return await _paginationService.ApplyPaginationAsync(trainingProgram.Steps.AsQueryable(), request);
        }

        public TrainingProgramStep Get(Guid trainingProgramId, Guid trainingProgramStepId)
        {
            var filterStep = Builders<TrainingProgramStep>.Filter.Eq(step => step.Id, trainingProgramStepId);
            var filter = Builders<TrainingProgram>.Filter.Eq(doc => doc.Id, trainingProgramId) & Builders<TrainingProgram>.Filter.ElemMatch(e => e.Steps, filterStep);

            TrainingProgram trainingProgram = _repository.FindOne(filter);

            if (trainingProgram != null)
                return trainingProgram.Steps.Find(step => step.Id == trainingProgramStepId);

            return null;
        }

        public async Task<TrainingProgramStep> Create(Guid trainingProgramId, TrainingProgramStep trainingProgramStep)
        {
            TrainingProgram trainingProgram = _trainingProgramService.Get(trainingProgramId);
            trainingProgramStep.Id = Guid.NewGuid();
            trainingProgramStep.CreatedBy = _authorizationContext.UserId;
            trainingProgramStep.CreatedAt = DateTime.Now;
            trainingProgramStep.UpdatedBy = _authorizationContext.UserId;
            trainingProgramStep.UpdatedAt = DateTime.Now;
            trainingProgram.Steps.Add(trainingProgramStep);

            trainingProgram.UpdatedAt = DateTime.Now;
            trainingProgram.UpdatedBy = _authorizationContext.UserId;

            trainingProgram = await _trainingProgramService.Update(trainingProgramId, trainingProgram);

            _logger.LogInformation($"Inserted TrainingProgramStep - {trainingProgram}");

            return trainingProgramStep;
        }

        public async Task<TrainingProgramStep> Update(Guid trainingProgramId, TrainingProgramStep trainingProgramStep)
        {
            TrainingProgram trainingProgram = _trainingProgramService.Get(trainingProgramId);
            TrainingProgramStep currentStep = Get(trainingProgramId, trainingProgramStep.Id);

            trainingProgramStep.CreatedBy = currentStep.CreatedBy;
            trainingProgramStep.CreatedAt = currentStep.CreatedAt;
            trainingProgramStep.UpdatedBy = _authorizationContext.UserId;
            trainingProgramStep.UpdatedAt = DateTime.Now;
            int index = trainingProgram.Steps.FindIndex(step => step.Id == trainingProgramStep.Id);
            if (index != -1)
            {
                trainingProgram.Steps[index] = trainingProgramStep;
                trainingProgram = await _trainingProgramService.Update(trainingProgramId, trainingProgram);
                _logger.LogInformation($"Updated TrainingProgramStep - {trainingProgram}");
            }
            return trainingProgramStep;
        }

        public void Delete(Guid trainingProgramId, Guid stepId)
        {
            TrainingProgram trainingProgram = _trainingProgramService.Get(trainingProgramId);
            trainingProgram.UpdatedAt = DateTime.Now;
            trainingProgram.UpdatedBy = _authorizationContext.UserId;

            trainingProgram.Steps.RemoveAll(step => step.Id == stepId);

            trainingProgram = _trainingProgramService.Update(trainingProgramId, trainingProgram).Result;

            _logger.LogInformation($"Deleted TrainingProgramStep - {stepId}");
        }
    }
}
