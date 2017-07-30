using System;
using System.Net.Http;
using MultiLoader.Core.Infrustructure;
using MultiLoader.Core.Model;

namespace MultiLoader.Core.Services
{
    public class HttpDownloader : IContentDownloader
    {
        private readonly HttpClient _httpClient;
        
        public HttpDownloader() => _httpClient = new HttpClient();

        public event EventHandler<ContentMetadata> OnDownload;
        public event EventHandler<Exception> OnDownloadError;

        public Content DownloadContent(ContentMetadata contentMetadata)
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
                    return null;
                }

                return new Content
                {
                    Data = byteArrayResult,
                    ContentMetadata = contentMetadata
                };
        }
             
    }
}
