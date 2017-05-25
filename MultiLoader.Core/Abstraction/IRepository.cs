using System;
using System.Collections.Generic;

namespace MultiLoader.Core.Abstraction
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> GetAll();
        void AddRange(IEnumerable<TEntity> entity);
        void Add(TEntity entity);
    }
}