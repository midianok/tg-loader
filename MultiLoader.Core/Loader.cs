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

        public Loader AddOnContentSavedHandler(EventHandler<Content> saveHandler)
        {
            _contentSaver.OnSave += saveHandler;
            return this;
        }

        public Loader AddOnSaveErrorHandler(EventHandler<Exception> saveErrorHandler)
        {
            _contentSaver.OnSaveError += saveErrorHandler;
            return this;
        }

        public void Download(string request) =>
            _apiAdapter
                .GetContentMetadata(request)
                .Except(_contentMetadataRepository.GetAll())
                .Map(_contentDownloader.DownloadContent)
                .Each(_contentSaver.SaveContent);

        public void ClearData(string filesPath)
        {
            _contentMetadataRepository.GetAll();
        }
    }
}
