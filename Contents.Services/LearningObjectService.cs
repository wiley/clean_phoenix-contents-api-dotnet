using Contents.Domain.LearningObject;
using Contents.Domain.Pagination;
using Contents.Infrastructure.Interface.Mongo;
using Contents.Services.Interfaces;
using DarwinAuthorization.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Contents.Services
{
    public class LearningObjectService : ILearningObjectService
    {
        private readonly IMongoRepository<LearningObject> _repository;
        private readonly IPaginationService<LearningObject> _paginationService;
        private readonly ILogger<LearningObjectService> _logger;
        private readonly DarwinAuthorizationContext _authorizationContext;

        public LearningObjectService(IMongoRepository<LearningObject> repository, ILogger<LearningObjectService> logger,
           IPaginationService<LearningObject> paginationService, DarwinAuthorizationContext authorizationContext)
        {
            _repository = repository;
            _logger = logger;
            _paginationService = paginationService;
            _authorizationContext = authorizationContext;
        }

        public int TotalFound => _paginationService.TotalRecords;

        public Task<List<LearningObject>> GetAll(LearningObjectFilter filter)
        {
            PageRequest pageRequest = new()
            {
                PageOffset = filter.Offset,
                PageSize = filter.Size
            };

            IQueryable<LearningObject> learningObjects = _repository.AsQueryable();

            if(!string.IsNullOrEmpty(filter.LanguageTag))
                learningObjects = learningObjects.Where(l => l.LanguageTag == filter.LanguageTag);

            if(!string.IsNullOrEmpty(filter.Type))
                learningObjects = learningObjects.Where(l => l.Type == filter.Type);

            if(filter.EstimatedDurationEqual != null)
                learningObjects = learningObjects.Where(l => l.EstimatedDuration == filter.EstimatedDurationEqual);

            if (filter.EstimatedDurationGreaterThan != null)
                learningObjects = learningObjects.Where(l => l.EstimatedDuration > filter.EstimatedDurationGreaterThan);

            if (filter.EstimatedDurationLowerThan != null)
                learningObjects = learningObjects.Where(l => l.EstimatedDuration < filter.EstimatedDurationLowerThan);

            if (filter.EstimatedDurationGreaterThanEqual != null)
                learningObjects = learningObjects.Where(l => l.EstimatedDuration >= filter.EstimatedDurationGreaterThanEqual);

            if (filter.EstimatedDurationLowerThanEqual != null)
                learningObjects = learningObjects.Where(l => l.EstimatedDuration <= filter.EstimatedDurationLowerThanEqual);

            return _paginationService.ApplyPaginationAsync(learningObjects, pageRequest);
        }

        public LearningObject Get(Guid Id)
        {
            return _repository.FindById(Id);
        }

        public async Task Create(LearningObject learningObject)
        {
            learningObject.CreatedAt = DateTime.Now;
            learningObject.UpdatedAt = DateTime.Now;
            learningObject.CreatedBy = _authorizationContext.UserId;
            learningObject.UpdatedBy = _authorizationContext.UserId;

            await _repository.InsertOneAsync(learningObject);

            _logger.LogInformation($"Inserted LearningObject - {learningObject}");
        }

        public async Task<LearningObject> Update(Guid Id, LearningObject learningObject)
        {
            LearningObject currentTrainingProgram = Get(Id);
            learningObject.Id = currentTrainingProgram.Id;
            learningObject.CreatedAt = currentTrainingProgram.CreatedAt;
            learningObject.CreatedBy = currentTrainingProgram.CreatedBy;
            learningObject.UpdatedAt = DateTime.Now;
            learningObject.UpdatedBy = _authorizationContext.UserId;
            await _repository.ReplaceOneAsync(learningObject);
            _logger.LogInformation($"Updated LearningObject Data - {learningObject}");

            return learningObject;
        }

        public LearningObject Delete(Guid Id)
        {
            return _repository.DeleteById(Id);
        }

        public Task<List<LearningObject>> Search(string query, List<string> types, int organizationId, JObject metadata, PageRequest request)
        {
            FilterDefinition<LearningObject> filterQuery = Builders<LearningObject>.Filter.Empty;
            FilterDefinition<LearningObject> filterTypes = Builders<LearningObject>.Filter.Empty;
            FilterDefinition<LearningObject> filterOrganizationId = Builders<LearningObject>.Filter.Empty;
            FilterDefinition<LearningObject> filterMetadata = Builders<LearningObject>.Filter.Empty;

            if (!string.IsNullOrEmpty(query))
            {
                query = Regex.Escape(query);

                filterQuery = Builders<LearningObject>.Filter.Or(
                    Builders<LearningObject>.Filter.Regex("Title", new BsonRegularExpression(query, "i")),
                    Builders<LearningObject>.Filter.Regex("Description", new BsonRegularExpression(query, "i"))
                );
            }
            if (types?.Count > 0)
                filterTypes = Builders<LearningObject>.Filter.In(learningObject => learningObject.Type, types);

            filterOrganizationId = Builders<LearningObject>.Filter.AnyEq(learningObject => learningObject.OrganizationIds, organizationId);

            List<FilterDefinition<LearningObject>> listOfFilters = new() { };
            
            if (metadata != null)
            {
                List<JProperty> fields = metadata.Properties().ToList();
                foreach (var field in fields)
                {
                    var fieldName = field.Name;
                    var fieldValue = field.Value;

                    var metadataFilter = fieldValue.Type == JTokenType.Array
                        ? Builders<LearningObject>.Filter.In(learningObject => (string)learningObject.Metadata[fieldName], fieldValue.ToObject<List<string>>())
                        : Builders<LearningObject>.Filter.Eq(learningObject => (string)learningObject.Metadata[fieldName], fieldValue.ToString());
                   
                    filterMetadata = filterMetadata & metadataFilter;
                }
            }          

            FilterDefinition<LearningObject> filter = filterQuery & filterTypes & filterOrganizationId & filterMetadata;

            List<LearningObject> learningObjects = _repository.Find(filter);
            return _paginationService.ApplyPaginationAsync(learningObjects.AsQueryable(), request);
        }
    }
}
