using Contents.Domain.Pagination;
using Contents.Domain.Skill;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contents.Services.Interfaces
{
    public interface ISkillCategoryService
    {
        int TotalFound { get; }

        Task<List<SkillCategory>> GetAll(SkillCategoryFilter request);

        SkillCategory Get(Guid id);

        Task Create(SkillCategory skillCategory);

        Task<SkillCategory> Update(Guid Id, SkillCategory skillCategory);

        SkillCategory Delete(Guid Id);

        Task<List<SkillCategory>> Search(string query, PageRequest request);
    }
}
