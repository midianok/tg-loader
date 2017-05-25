using System;
using System.Linq;
using MultiLoader.Core.Abstraction;
using MultiLoader.Core.Model;
using MultiLoader.Core.Tool;

namespace MultiLoader.Core
{
    public class Loader
    {
        private readonly IApiAdapter _apiAdapter;
        private readonly IContentDownloader _contentDownloader;
        private readonly IContentSaver _contentSaver;
        private readonly IContentMetadataRepository _contentMetadataRepository;

        public Loader(
            IContentDownloader contentDownloader, 
            IApiAdapter apiAdapter, 
            IContentSaver contentSaver,
            IContentMetadataRepository contentMetadataRepository)
        {
            _contentDownloader = contentDownloader;
            _apiAdapter = apiAdapter;
            _contentSaver = contentSaver;
            _contentMetadataRepository = contentMetadataRepository;

            _contentSaver.OnSave += (obj, content) => _contentMetadataRepository.Add(content.ContentMetadata);
        }

        public Loader AddOnSavedHandler(EventHandler<Content> onSaveHandler)
        {
            _contentSaver.OnSave += onSaveHandler;
            return this;
        }

        public Loader AddOnSaveErrorHandler(EventHandler<Exception> onSaveErrorHandler)
        {
            _contentSaver.OnSaveError += onSaveErrorHandler;
            return this;
        }

        public Loader AddOnContentDownloadHandler(EventHandler<ContentMetadata> onDownloadHandler)
        {
            _contentDownloader.OnDownload += onDownloadHandler;
            return this;
        }

        public Loader AddOnContentDownloadErrorHandler(EventHandler<Exception> onDownloadErrorHandler)
        {
            _contentDownloader.OnDownloadError += onDownloadErrorHandler;
            return this;
        }

        public Loader AddOnGetContentMetadataHandler(EventHandler<int> onGetContentMetadataHandler)
        {
            _apiAdapter.OnGetContentMetadata += onGetContentMetadataHandler;
            return this;
        }

        public Loader AddGetContentMetadataErrorHandler(EventHandler<Exception> onGetContentMetadataErrorHandler)
        {
            _apiAdapter.OnGetContentMetadataError += onGetContentMetadataErrorHandler;
            return this;
        }

        public void Download(string request) =>
            _apiAdapter
                .GetContentMetadata(request)
                .Except(_contentMetadataRepository.GetAll())
                .Map(_contentDownloader.DownloadContent)
                .Each(_contentSaver.SaveContent);
    }
}
