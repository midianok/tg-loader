﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using MultiLoader.Core.Abstraction;
using MultiLoader.Core.Model;

namespace MultiLoader.Core.Services
{
    public class Downloader : IContentDownloader
    {
        private readonly HttpClient _httpClient;
        
        public Downloader() => _httpClient = new HttpClient();

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
