﻿using System.Threading.Tasks;

namespace Contents.Infrastructure.Interface.Mongo
{
    public interface IConnectionThrottlingPipeline
    {
        Task<T> AddRequest<T>(Task<T> task);

        Task AddRequest(Task task);
    }
}