using System.Collections.Generic;
using MultiLoader.Core.Abstraction;
using MultiLoader.Core.Model;
using LiteDB;

namespace MultiLoader.Core.Db
{
    public class ContentMetadataRepository : LiteDbRepository<ContentMetadata>, IContentMetadataRepository
    {
        public ContentMetadataRepository(string dbpath) : base(dbpath){}

        public ContentMetadataRepository() : base(@"C:\Users\Midian\test\persona_5") { }

        public IEnumerable<ContentMetadata> GetMetdataByRequest(string request)
        {
            using (var db = new LiteDatabase(DbPath))
            {
                var collection = db.GetCollection<ContentMetadata>();
                return collection.Find(x => x.RequestString == request);
            }
        }
    }
}
