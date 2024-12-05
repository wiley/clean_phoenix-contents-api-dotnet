using Contents.Domain.Pagination;
using Contents.Domain.Skill;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contents.Services.Interfaces
{
    public interface ISkillService
    {
        int TotalFound { get; }

        Task<List<Skill>> GetAll(PageRequest request);

        Skill Get(Guid id);

        Task Create(Skill skill);

        Task<Skill> Update(Guid Id, Skill skill);

        Skill Delete(Guid Id);

        void DeleteMany(FilterDefinition<Skill> filterDefinition);

        Task<List<Skill>> Search(string query, PageRequest request);
    }
}
