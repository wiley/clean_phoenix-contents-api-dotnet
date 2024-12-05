using Contents.Domain.Pagination;
using Contents.Domain.LearningObject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Contents.Services.Interfaces
{
    public interface ILearningObjectService
    {
        int TotalFound { get; }

        Task<List<LearningObject>> GetAll(LearningObjectFilter filter);

        LearningObject Get(Guid id);

        Task Create(LearningObject learningObject);

        Task<LearningObject> Update(Guid Id, LearningObject learningObject);

        LearningObject Delete(Guid Id);

        Task<List<LearningObject>> Search(string query, List<string> types, int organizationId, JObject metadata, PageRequest request);
    }
}
