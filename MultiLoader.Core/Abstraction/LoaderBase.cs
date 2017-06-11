using System;
using MultiLoader.Core.Model;

namespace MultiLoader.Core.Abstraction
{
    public abstract class LoaderBase
    {
        protected readonly IApiAdapter ApiAdapter;
        protected readonly IContentDownloader ContentDownloader;
        protected readonly IContentSaver ContentSaver;
        protected readonly IContentMetadataRepository ContentMetadataRepository;

        protected LoaderBase(
            IContentDownloader contentDownloader, 
            IApiAdapter apiAdapter, 
            IContentSaver contentSaver,
            IContentMetadataRepository contentMetadataRepository)
        {
            ContentDownloader = contentDownloader;
            ApiAdapter = apiAdapter;
            ContentSaver = contentSaver;
            ContentMetadataRepository = contentMetadataRepository;

            //ContentSaver.OnSave += (obj, content) => ContentMetadataRepository.Add(content.ContentMetadata);
        }

        public LoaderBase AddOnSavedHandler(EventHandler<Content> onSaveHandler)
        {
            ContentSaver.OnSave += onSaveHandler;
            return this;
        }

        public LoaderBase AddOnSaveErrorHandler(EventHandler<Exception> onSaveErrorHandler)
        {
            ContentSaver.OnSaveError += onSaveErrorHandler;
            return this;
        }

        public LoaderBase AddOnContentDownloadHandler(EventHandler<ContentMetadata> onDownloadHandler)
        {
            ContentDownloader.OnDownload += onDownloadHandler;
            return this;
        }

        public LoaderBase AddOnContentDownloadErrorHandler(EventHandler<Exception> onDownloadErrorHandler)
        {
            ContentDownloader.OnDownloadError += onDownloadErrorHandler;
            return this;
        }

        public LoaderBase AddOnGetContentMetadataHandler(EventHandler<int> onGetContentMetadataHandler)
        {
            ApiAdapter.OnGetContentMetadata += onGetContentMetadataHandler;
            return this;
        }

        public LoaderBase AddOnGetContentMetadataErrorHandler(EventHandler<Exception> onGetContentMetadataErrorHandler)
        {
            ApiAdapter.OnGetContentMetadataError += onGetContentMetadataErrorHandler;
            return this;
        }

        public abstract void Download(string request);
        //public void Download(string request) =>
        //    _apiAdapter
        //        .GetContentMetadata(request)
        //        .Except(_contentMetadataRepository.GetAll())
        //        .Map(_contentDownloader.DownloadContent)
        //        .Each(_contentSaver.SaveContent);
    }
}
