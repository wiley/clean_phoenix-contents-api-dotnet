using Contents.Domain.Pagination;
using Contents.Domain.Publisher;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Contents.Services.Interfaces
{
    public interface IPublisherService
    {
        int TotalFound { get; }

        Task<List<Publisher>> GetAll(PageRequest request);

        Publisher Get(Guid id);

        Task Create(Publisher publisher);

        Task<Publisher> Update(Guid Id, Publisher publisher);

        Publisher Delete(Guid Id);

        
    }
}
