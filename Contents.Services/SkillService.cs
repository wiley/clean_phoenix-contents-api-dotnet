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
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Contents.Services
{
    public class SkillService : ISkillService
    {
        private readonly IMongoRepository<Skill> _repository;
        private readonly IPaginationService<Skill> _paginationService;
        private readonly ILogger<SkillService> _logger;
        private readonly ISkillCategoryService _skillCategoryService;
        private readonly DarwinAuthorizationContext _authorizationContext;

        public SkillService(
            IMongoRepository<Skill> repository,
            ILogger<SkillService> logger,
            IPaginationService<Skill> paginationService,
            ISkillCategoryService skillCategoryService,
            DarwinAuthorizationContext authorizationContext)
        {
            _repository = repository;
            _logger = logger;
            _paginationService = paginationService;
            _skillCategoryService = skillCategoryService;
            _authorizationContext = authorizationContext;
        }

        public int TotalFound => _paginationService.TotalRecords;

        public Task<List<Skill>> GetAll(PageRequest request)
        {
            IQueryable<Skill> skills = _repository.AsQueryable();
            return _paginationService.ApplyPaginationAsync(skills, request);
        }

        public Skill Get(Guid Id)
        {
            return _repository.FindById(Id);
        }

        public async Task Create(Skill skill)
        {
            if (skill.CategoryId != Guid.Empty)
            {
                SkillCategory skillCategory = _skillCategoryService.Get(skill.CategoryId);
                if (skillCategory != null) {
                    skill.PublisherId = skillCategory.PublisherId;
                }
            }
            skill.CreatedAt = DateTime.Now;
            skill.UpdatedAt = DateTime.Now;
            skill.CreatedBy = _authorizationContext.UserId;
            skill.UpdatedBy = _authorizationContext.UserId;

            await _repository.InsertOneAsync(skill);

            _logger.LogInformation($"Inserted Skill - {skill}");
        }

        public async Task<Skill> Update(Guid Id, Skill skill)
        {
            if (skill.CategoryId != Guid.Empty)
            {
                SkillCategory skillCategory = _skillCategoryService.Get(skill.CategoryId);
                if (skillCategory != null)
                {
                    skill.PublisherId = skillCategory.PublisherId;
                }
            }

            Skill currentSkill = Get(Id);
            skill.Id = currentSkill.Id;
            skill.CreatedBy = currentSkill.CreatedBy;
            skill.CreatedAt = currentSkill.CreatedAt;
            skill.UpdatedAt = DateTime.Now;
            skill.UpdatedBy = _authorizationContext.UserId;
            await _repository.ReplaceOneAsync(skill);
            _logger.LogInformation($"Updated Skill Data - {skill}");

            return skill;
        }

        public Skill Delete(Guid Id)
        {
            return _repository.DeleteById(Id);
        }

        public void DeleteMany(FilterDefinition<Skill> filterDefinition)
        {
            try
            {
                _repository.DeleteMany(filterDefinition);
            }
            catch (Exception ex)
            {
                _logger.LogError("DeleteManySkill - Unhandled Error", ex);
            }
        }

        public Task<List<Skill>> Search(string query, PageRequest request)
        {
            FilterDefinition<Skill> filterQuery = Builders<Skill>.Filter.Empty;

            if (!string.IsNullOrEmpty(query))
            {
                filterQuery = Builders<Skill>.Filter.Or(
                    Builders<Skill>.Filter.Regex("Name", new BsonRegularExpression(query, "i")),
                    Builders<Skill>.Filter.Regex("Description", new BsonRegularExpression(query, "i"))
                );
            }

            List<Skill> skills = _repository.Find(filterQuery);
            return _paginationService.ApplyPaginationAsync(skills.AsQueryable(), request);
        }
    }
}
