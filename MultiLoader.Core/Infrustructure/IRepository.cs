using System.Collections.Generic;

namespace MultiLoader.Core.Infrustructure
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> GetAll();
        void AddRange(IEnumerable<TEntity> entity);
        void Add(TEntity entity);
        byte[] Commit();
    }
}