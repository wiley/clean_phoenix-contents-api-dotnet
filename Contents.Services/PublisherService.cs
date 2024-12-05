using Contents.Domain.Pagination;
using Contents.Domain.Publisher;
using Contents.Domain.Skill;
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
    public class PublisherService : IPublisherService
    {
        private readonly IMongoRepository<Publisher> _repository;
        private readonly IMongoRepository<SkillCategory> _skillCategoryRepository;
        private readonly IMongoRepository<Skill> _skillRepository;
        private readonly IPaginationService<Publisher> _paginationService;
        private readonly ILogger<PublisherService> _logger;
        private readonly DarwinAuthorizationContext _authorizationContext;

        public PublisherService(
            IMongoRepository<Publisher> repository,
            IMongoRepository<SkillCategory> skillCategoryRepository,
            IMongoRepository<Skill> skillRepository,
            ILogger<PublisherService> logger,
            IPaginationService<Publisher> paginationService,
            DarwinAuthorizationContext authorizationContext)
        {
            _repository = repository;
            _skillCategoryRepository = skillCategoryRepository;
            _skillRepository = skillRepository;
            _logger = logger;
            _paginationService = paginationService;
            _authorizationContext = authorizationContext;
        }

        public int TotalFound => _paginationService.TotalRecords;

        public Task<List<Publisher>> GetAll(PageRequest request)
        {
            IQueryable<Publisher> publishers = _repository.AsQueryable();

            return _paginationService.ApplyPaginationAsync(publishers, request);
        }

        public Publisher Get(Guid Id)
        {
            return _repository.FindById(Id);
        }

        public async Task Create(Publisher publisher)
        {
            publisher.CreatedAt = DateTime.Now;
            publisher.UpdatedAt = DateTime.Now;
            publisher.CreatedBy = _authorizationContext.UserId;
            publisher.UpdatedBy = _authorizationContext.UserId;

            await _repository.InsertOneAsync(publisher);

            _logger.LogInformation($"Inserted Publisher - {publisher}");
        }

        public async Task<Publisher> Update(Guid Id, Publisher publisher)
        {
            Publisher currentPublisher = Get(Id);
            publisher.Id = currentPublisher.Id;
            publisher.CreatedBy = currentPublisher.CreatedBy;
            publisher.CreatedAt = currentPublisher.CreatedAt;
            publisher.UpdatedAt = DateTime.Now;
            publisher.UpdatedBy = _authorizationContext.UserId;
            await _repository.ReplaceOneAsync(publisher);
            _logger.LogInformation($"Updated Publisher Data - {publisher}");

            return publisher;
        }

        public Publisher Delete(Guid Id)
        {
            Publisher publisher = _repository.DeleteById(Id);
            FilterDefinition<SkillCategory> filter = Builders<SkillCategory>.Filter.Eq(sc => sc.PublisherId, Id);
            List<SkillCategory> skillCategories = _skillCategoryRepository.Find(filter);
            skillCategories.ForEach(skillCategory =>
            {
                FilterDefinition<Skill> filterSkill = Builders<Skill>.Filter.Eq(s => s.CategoryId, skillCategory.Id);
                _skillRepository.DeleteMany(filterSkill);
            });

            _skillCategoryRepository.DeleteMany(filter);
            return publisher;
        }
    }
}
