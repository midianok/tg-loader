using System.Collections.Generic;
using MultiLoader.Core.Model;
using System;

namespace MultiLoader.Core.Abstraction
{
    public interface IContentDownloader
    {
        IEnumerable<Content> DownloadContent(IEnumerable<ContentMetadata> contentMetadata);
        event EventHandler<ContentMetadata> OnDownload;
        event EventHandler<Exception> OnDownloadError;
    }
}
