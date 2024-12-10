using Contents.Domain.Pagination;
using Contents.Domain.Skill;
using Contents.Infrastructure.Interface.Mongo;
using Contents.Services.Interfaces;
using DarwinAuthorization.Models;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Contents.Services
{
    public class SkillCategoryService : ISkillCategoryService
    {
        private readonly IMongoRepository<SkillCategory> _repository;
        private readonly IMongoRepository<Skill> _skillRepository;
        private readonly IPaginationService<SkillCategory> _paginationService;
        private readonly ILogger<SkillCategoryService> _logger;
        private readonly DarwinAuthorizationContext _authorizationContext;

        public SkillCategoryService(
            IMongoRepository<SkillCategory> repository,
            IMongoRepository<Skill> skillRepository,
            ILogger<SkillCategoryService> logger,
            IPaginationService<SkillCategory> paginationService,
            DarwinAuthorizationContext authorizationContext)
        {
            _repository = repository;
            _skillRepository = skillRepository;
            _logger = logger;
            _paginationService = paginationService;
            _authorizationContext = authorizationContext;
        }

        public int TotalFound => _paginationService.TotalRecords;

        public Task<List<SkillCategory>> GetAll(SkillCategoryFilter filter)
        {

            IQueryable<SkillCategory> skillCategories = _repository.AsQueryable();

            if(filter.RootOnly)
            {
                skillCategories = skillCategories.Where(sc => sc.ParentCategoryId == Guid.Empty);
            }

            PageRequest request = new()
            {
                PageOffset = filter.Offset,
                PageSize = filter.Size
            };

            return _paginationService.ApplyPaginationAsync(skillCategories, request);
        }

        public SkillCategory Get(Guid Id)
        {
            return _repository.FindById(Id);
        }

        public async Task Create(SkillCategory skillCategorie)
        {
            skillCategorie.CreatedAt = DateTime.Now;
            skillCategorie.UpdatedAt = DateTime.Now;
            skillCategorie.CreatedBy = _authorizationContext.UserId;
            skillCategorie.UpdatedBy = _authorizationContext.UserId;

            await _repository.InsertOneAsync(skillCategorie);

            _logger.LogInformation($"Inserted SkillCategory - {skillCategorie}");
        }

        public async Task<SkillCategory> Update(Guid Id, SkillCategory skillCategorie)
        {
            SkillCategory currentSkillCategorie = Get(Id);
            skillCategorie.Id = currentSkillCategorie.Id;
            skillCategorie.CreatedBy = currentSkillCategorie.CreatedBy;
            skillCategorie.CreatedAt = currentSkillCategorie.CreatedAt;
            skillCategorie.UpdatedAt = DateTime.Now;
            skillCategorie.UpdatedBy = _authorizationContext.UserId;
            await _repository.ReplaceOneAsync(skillCategorie);
            _logger.LogInformation($"Updated SkillCategory Data - {skillCategorie}");

            return skillCategorie;
        }

        public SkillCategory Delete(Guid Id)
        {
            SkillCategory skillCategory = _repository.DeleteById(Id);
            FilterDefinition<Skill> filter = Builders<Skill>.Filter.Eq(s => s.CategoryId, Id);
            _skillRepository.DeleteMany(filter);

            FilterDefinition<SkillCategory> filterSkillsCategoryChildren = Builders<SkillCategory>.Filter.Eq(s => s.ParentCategoryId, Id);
            List<SkillCategory> skillsCategoryChildren = _repository.Find(filterSkillsCategoryChildren);
            skillsCategoryChildren.ForEach(s => {
                this.Delete(s.Id);
            });

            return skillCategory;
        }

        public Task<List<SkillCategory>> Search(string query, PageRequest request)
        {
            FilterDefinition<SkillCategory> filterQuery = Builders<SkillCategory>.Filter.Empty;

            if (!string.IsNullOrEmpty(query))
            {
                filterQuery = Builders<SkillCategory>.Filter.Regex("Name", new BsonRegularExpression(query, "i"));
            }

            List<SkillCategory> skillCategories = _repository.Find(filterQuery);
            return _paginationService.ApplyPaginationAsync(skillCategories.AsQueryable(), request);
        }
    }
}
