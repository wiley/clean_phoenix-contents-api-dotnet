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
    public class TrainingProgramStepLearningObjectService : ITrainingProgramStepLearningObjectService
    {
        private readonly IMongoRepository<TrainingProgram> _repository;
        private readonly IPaginationService<LearningObjectStep> _paginationService;
        private readonly ILogger<TrainingProgramStepLearningObjectService> _logger;
        private readonly ITrainingProgramService _trainingProgramService;
        private readonly ITrainingProgramStepService _trainingProgramStepService;
        private readonly DarwinAuthorizationContext _authorizationContext;

        public TrainingProgramStepLearningObjectService(
            IMongoRepository<TrainingProgram> repository,
            ILogger<TrainingProgramStepLearningObjectService> logger,
            IPaginationService<LearningObjectStep> paginationService,
            ITrainingProgramService trainingProgramService,
            ITrainingProgramStepService trainingProgramStepService,
            DarwinAuthorizationContext authorizationContext)
        {
            _repository = repository;
            _logger = logger;
            _paginationService = paginationService;
            _authorizationContext = authorizationContext;
            _trainingProgramService = trainingProgramService;
            _trainingProgramStepService = trainingProgramStepService;
        }

        public int TotalFound => _paginationService.TotalRecords;

        public async Task<List<LearningObjectStep>> GetAll(Guid trainingProgramId, Guid stepId, PageRequest request)
        {
            TrainingProgramStep step = _trainingProgramStepService.Get(trainingProgramId, stepId);
            return await _paginationService.ApplyPaginationAsync(step.LearningObjects.AsQueryable(), request);
        }

        public LearningObjectStep Get(Guid trainingProgramId, Guid stepId, Guid Id)
        {
            var filterLearningObject = Builders<LearningObjectStep>.Filter.Eq(learningObject => learningObject.LearningObject.Id, Id);

            var filterStep = Builders<TrainingProgramStep>.Filter.Eq(step => step.Id, stepId) 
                & Builders<TrainingProgramStep>.Filter.ElemMatch(step => step.LearningObjects, filterLearningObject);

            var filter = Builders<TrainingProgram>.Filter.Eq(doc => doc.Id, trainingProgramId)
                & Builders<TrainingProgram>.Filter.ElemMatch(e => e.Steps, filterStep);

            TrainingProgram trainingProgram = _repository.FindOne(filter);

            if (trainingProgram != null)
                return trainingProgram.Steps
                        .SelectMany(step => step.LearningObjects)
                        .FirstOrDefault(learningObject => learningObject.LearningObject.Id == Id);
            return null;
        }

        public async Task<LearningObjectStep> Create(Guid trainingProgramId, Guid stepId, LearningObjectStep learningObjectStep)
        {
            TrainingProgramStep trainingProgramStep = _trainingProgramStepService.Get(trainingProgramId, stepId);
            trainingProgramStep.LearningObjects.Add(learningObjectStep);

            trainingProgramStep.UpdatedAt = DateTime.Now;
            trainingProgramStep.UpdatedBy = _authorizationContext.UserId;

            trainingProgramStep = await _trainingProgramStepService.Update(trainingProgramId, trainingProgramStep);

            _logger.LogInformation($"Inserted TrainingProgramStepLearningObject - {trainingProgramStep}");

            return learningObjectStep;
        }

        public async Task<LearningObjectStep> Update(Guid trainingProgramId, Guid stepId, LearningObjectStep learningObjectStep)
        {
            TrainingProgramStep step = _trainingProgramStepService.Get(trainingProgramId, stepId);
            step.UpdatedBy = _authorizationContext.UserId;
            step.UpdatedAt = DateTime.Now;
            int index = step.LearningObjects.FindIndex(learningObject => learningObject.LearningObject.Id == learningObjectStep.LearningObject.Id);
            if (index != -1)
            {
                step.LearningObjects[index] = learningObjectStep;
                step = await _trainingProgramStepService.Update(trainingProgramId, step);
                _logger.LogInformation($"Updated TrainingProgramStepLearningObject - {step}");
            }
            return learningObjectStep;
        }
        
        public void Delete(Guid trainingProgramId, Guid stepId, Guid Id)
        {
            TrainingProgramStep step = _trainingProgramStepService.Get(trainingProgramId, stepId);
            step.UpdatedAt = DateTime.Now;
            step.UpdatedBy = _authorizationContext.UserId;

            step.LearningObjects.RemoveAll(learningObject => learningObject.LearningObject.Id == Id);

            step = _trainingProgramStepService.Update(trainingProgramId, step).Result;

            _logger.LogInformation($"Deleted TrainingProgramStepLearningObject - {step}");
        }
    }
}
