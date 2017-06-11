using MultiLoader.Core.Model;
using System;

namespace MultiLoader.Core.Abstraction
{
    public interface IContentDownloader
    {
        Content DownloadContent(ContentMetadata contentMetadata);
        event EventHandler<ContentMetadata> OnDownload;
        event EventHandler<Exception> OnDownloadError;
    }
}
