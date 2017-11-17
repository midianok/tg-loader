using System.Collections.Generic;
using System.IO;
using System.Linq;
using LiteDB;
using MultiLoader.Core.Infrustructure;

namespace MultiLoader.Core.Db
{
    public class LiteDbRepository<TEntity> : IRepository<TEntity> where TEntity : class 
    {
        private readonly string _dbPath;

        public LiteDbRepository(string dbLocation)
        {
            if (!Directory.Exists(dbLocation)) Directory.CreateDirectory(dbLocation);
            _dbPath = $"{dbLocation}\\metadata.db";
        }
            
        #region Repository members
        public IEnumerable<TEntity> GetAll()
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection<TEntity>();
                if (collection.Count() == 0) return new List<TEntity>();  
                return collection.FindAll().ToList();
            }
        }

        public void Add(TEntity entity)
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection<TEntity>();
                collection.Insert(entity);
            }
        }

        public byte[] Commit()
        {
            throw new System.NotImplementedException();
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection<TEntity>();
                using (var trans = db.BeginTrans())
                {
                    foreach (var entity in entities)
                        collection.Insert(entity);
                    trans.Commit();
                }
            }
        }
        #endregion
    }
}