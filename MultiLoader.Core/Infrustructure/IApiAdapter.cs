using System;
using System.Collections.Generic;
using MultiLoader.Core.Model;

namespace MultiLoader.Core.Infrustructure
{
    public interface IApiAdapter
    {
        bool ParallelDownloadSupported { get; }
        string RequestName { get; }
        IEnumerable<ContentMetadata> GetContentMetadata();
        event EventHandler<Exception> OnGetContentMetadataError;
        event EventHandler<int> OnGetContentMetadata;
    }
}
