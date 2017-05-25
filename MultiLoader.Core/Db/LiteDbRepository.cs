using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MultiLoader.Core.Abstraction;
using LiteDB;

namespace MultiLoader.Core.Db
{
    public class LiteDbRepository<TEntity> : IRepository<TEntity> where TEntity : class 
    {
        protected readonly string DbPath;

        public LiteDbRepository(string dbLocation)
        {
            if (!Directory.Exists(dbLocation)) Directory.CreateDirectory(dbLocation);
            DbPath = $"{dbLocation}\\metadata.db";
        }
            

        #region Repository members
        public IEnumerable<TEntity> GetAll()
        {
            using (var db = new LiteDatabase(DbPath))
            {
                var collection = db.GetCollection<TEntity>();
                if (collection.Count() == 0) return new List<TEntity>();  
                return collection.FindAll().ToList();
            }
        }

        public void Add(TEntity entity)
        {
            using (var db = new LiteDatabase(DbPath))
            {
                var collection = db.GetCollection<TEntity>();
                collection.Insert(entity);
            }
        }

        public void AddRange(IEnumerable<TEntity> offers)
        {
            foreach (var offer in offers)
            {
                using (var db = new LiteDatabase(DbPath))
                {
                    var collection = db.GetCollection<TEntity>();
                    collection.Insert(offer);
                }
            }
        }

        public void Clear()
        {
            try
            {
                File.Delete(DbPath);
            }
            catch { }
        }
        #endregion
    }
}