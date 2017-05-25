using System.Collections.Generic;
using MultiLoader.Core.Model;
using System;

namespace MultiLoader.Core.Abstraction
{
    public interface IApiAdapter
    {
        IEnumerable<ContentMetadata> GetContentMetadata(string searchRequest);
        event EventHandler<Exception> OnGetContentMetadataError;
        event EventHandler<int> OnGetContentMetadata;
    }
}
