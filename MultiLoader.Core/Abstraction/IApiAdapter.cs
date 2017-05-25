using System.Collections.Generic;
using MultiLoader.Core.Model;

namespace MultiLoader.Core.Abstraction
{
    public interface IApiAdapter
    {
        IEnumerable<ContentMetadata> GetContentMetadata(string searchRequest);
    }
}
