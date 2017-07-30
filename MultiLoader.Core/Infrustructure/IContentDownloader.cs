using System;
using MultiLoader.Core.Model;

namespace MultiLoader.Core.Infrustructure
{
    public interface IContentDownloader
    {
        Content DownloadContent(ContentMetadata contentMetadata);
        event EventHandler<ContentMetadata> OnDownload;
        event EventHandler<Exception> OnDownloadError;
    }
}
