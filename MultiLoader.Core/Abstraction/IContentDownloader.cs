using System.Collections.Generic;
using MultiLoader.Core.Model;

namespace MultiLoader.Core.Abstraction
{
    public interface IContentDownloader
    {
        IEnumerable<Content> DownloadContent(IEnumerable<ContentMetadata> contentMetadata);
    }
}
