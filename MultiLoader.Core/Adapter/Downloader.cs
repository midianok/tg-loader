using System.Collections.Generic;
using System.Net.Http;
using MultiLoader.Core.Abstraction;
using MultiLoader.Core.Model;

namespace MultiLoader.Core.Adapter
{
    public class Downloader : IContentDownloader
    {
        private readonly HttpClient _httpClient;
        
        public Downloader() => _httpClient = new HttpClient();

        public IEnumerable<Content> DownloadContent(IEnumerable<ContentMetadata> contentMetadatas)
        {
            foreach (var contentMetadata in contentMetadatas)
            {
                var byteArrayResult = _httpClient.GetByteArrayAsync(contentMetadata.Uri).Result;

                yield return new Content
                {
                    Data = byteArrayResult,
                    ContentMetadata = contentMetadata
                };
            }
        }
             
    }
}
