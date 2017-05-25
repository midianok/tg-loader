using System.Collections.Generic;
using MultiLoader.Core.Model;

namespace MultiLoader.Core.Abstraction
{
    public interface IContentMetadataRepository : IRepository<ContentMetadata>
    {
        IEnumerable<ContentMetadata> GetMetdataByRequest(string request);
    }
}
