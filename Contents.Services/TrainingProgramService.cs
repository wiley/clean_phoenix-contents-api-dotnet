using Contents.Domain.Pagination;
using Contents.Domain.TrainingProgram;
using Contents.Infrastructure.Interface.Mongo;
using Contents.Services.Interfaces;
using DarwinAuthorization.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using WLS.KafkaMessenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace Contents.Services
{
    public class TrainingProgramService : ITrainingProgramService
    {
        private readonly IMongoRepository<TrainingProgram> _repository;
        private readonly IPaginationService<TrainingProgram> _paginationService;
        private readonly IKafkaService _kafkaService;
        private readonly ILogger<TrainingProgramService> _logger;
        private readonly DarwinAuthorizationContext _authorizationContext;

        public TrainingProgramService(
            IMongoRepository<TrainingProgram> repository,
            ILogger<TrainingProgramService> logger,
            IKafkaService kafkaService, 
            IPaginationService<TrainingProgram> paginationService,
        DarwinAuthorizationContext authorizationContext)
        {
            _repository = repository;
            _logger = logger;
            _kafkaService = kafkaService;
            _paginationService = paginationService;
            _authorizationContext = authorizationContext;
        }

        public int TotalFound => _paginationService.TotalRecords;

        public Task<List<TrainingProgram>> GetAll(PageRequest request)
        {
            IQueryable<TrainingProgram> trainingPrograms = _repository.AsQueryable();

            return _paginationService.ApplyPaginationAsync(trainingPrograms, request);
        }

        public TrainingProgram Get(Guid Id)
        {
            return _repository.FindById(Id);
        }

        public Task<List<TrainingProgram>> SearchByOrganizationId(PageRequest request, int? organizationId)
        {
            var filter = Builders<TrainingProgram>.Filter.Eq("OrganizationIds", organizationId);   
            var sort = Builders<TrainingProgram>.Sort.Descending("UpdatedAt");
            IQueryable<TrainingProgram> trainingPrograms = _repository.FilterBy(filter, sort).AsQueryable();

            return _paginationService.ApplyPaginationAsync(trainingPrograms, request);
        }

        public async Task Create(TrainingProgram trainingProgram)
        {
            trainingProgram.CreatedAt = DateTime.Now;
            trainingProgram.UpdatedAt = DateTime.Now;
            trainingProgram.CreatedBy = _authorizationContext.UserId;
            trainingProgram.UpdatedBy = _authorizationContext.UserId;

            await _repository.InsertOneAsync(trainingProgram);
            await _kafkaService.SendTrainingProgramKafkaMessage(trainingProgram, "TrainingProgramUpdated");

            _logger.LogInformation($"Inserted TrainingProgram - {trainingProgram}");
        }

        public async Task<TrainingProgram> Update(Guid Id, TrainingProgram trainingProgram)
        {
            TrainingProgram currentTrainingProgram = Get(Id);
            trainingProgram.Id = currentTrainingProgram.Id;
            trainingProgram.Steps ??= currentTrainingProgram.Steps;
            trainingProgram.CreatedAt = currentTrainingProgram.CreatedAt;
            trainingProgram.CreatedBy = currentTrainingProgram.CreatedBy;
            trainingProgram.UpdatedAt = DateTime.Now;
            trainingProgram.UpdatedBy = _authorizationContext.UserId;
            await _repository.ReplaceOneAsync(trainingProgram);
            _logger.LogInformation($"Updated TrainingProgram Data - {trainingProgram}");
            await _kafkaService.SendTrainingProgramKafkaMessage(trainingProgram, "TrainingProgramUpdated");
            return trainingProgram;
        }

        public async Task<TrainingProgram> Delete(Guid Id)
        {
            TrainingProgram trainingProgramDeleted = _repository.DeleteById(Id);
            
            if (trainingProgramDeleted != null) {
                await _kafkaService.SendTrainingProgramDeleteKafkaMessage(trainingProgramDeleted.Id, _authorizationContext.UserId, "TrainingProgramRemoved");
            }
            
            return trainingProgramDeleted;
        }

        public async Task GenerateKafkaEvents()
        {
            try {
                List<TrainingProgram> trainingPrograms = _repository.AsQueryable().ToList();
                foreach(TrainingProgram trainingProgram in trainingPrograms)
                {
                    await _kafkaService.SendTrainingProgramKafkaMessage(trainingProgram, "TrainingProgramUpdated");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "GenerateKafkaEvents - Error");
            }
        }
    }
}
