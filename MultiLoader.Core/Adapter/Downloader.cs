using System;
using System.Collections.Generic;
using System.Net.Http;
using MultiLoader.Core.Abstraction;
using MultiLoader.Core.Model;
using System.Linq;

namespace MultiLoader.Core.Adapter
{
    public class Downloader : IContentDownloader
    {
        private readonly HttpClient _httpClient;
        
        public Downloader() => _httpClient = new HttpClient();

        public event EventHandler<ContentMetadata> OnDownload;
        public event EventHandler<Exception> OnDownloadError;

        public IEnumerable<Content> DownloadContent(IEnumerable<ContentMetadata> contentMetadatas)
        {
            foreach (var contentMetadata in contentMetadatas)
            {
                byte[] byteArrayResult;
                try
                {
                    byteArrayResult = _httpClient.GetByteArrayAsync(contentMetadata.Uri).Result;
                    OnDownload?.Invoke(this, contentMetadata);
                }
                catch(Exception ex)
                {
                    OnDownloadError?.Invoke(this, ex);
                    continue;
                }

                yield return new Content
                {
                    Data = byteArrayResult,
                    ContentMetadata = contentMetadata
                };
            }
        }
             
    }
}
