using System;
using System.IO;
using MultiLoader.Core.Adapter;
using MultiLoader.Core.Db;
using MultiLoader.Core.Model;
using MultiLoader.Core.Services;

namespace MultiLoader.Core.Infrustructure
{
    public abstract class Loader
    {
        protected readonly IApiAdapter ApiAdapter;
        protected readonly IContentDownloader ContentDownloader;
        protected readonly IContentSaver ContentSaver;
        protected readonly IRepository<ContentMetadata> ContentMetadataRepository;
        protected EventHandler OnDownloadFinishedHandler;
        private EventHandler<int> _onAlreadyExistItemsFiltered;
        

        protected Loader(
            IContentDownloader contentDownloader, 
            IApiAdapter apiAdapter, 
            IContentSaver contentSaver,
            IRepository<ContentMetadata> contentMetadataRepository)
        {
            ContentDownloader = contentDownloader;
            ApiAdapter = apiAdapter;
            ContentSaver = contentSaver;
            ContentMetadataRepository = contentMetadataRepository;
        }

        public Loader AddOnAlreadyExistItemsFilteredHandler(EventHandler<int> onAlreadyExistItemsFilteredHandler)
        {
            _onAlreadyExistItemsFiltered += onAlreadyExistItemsFilteredHandler;
            return this;
        }

        public Loader AddOnSavedHandler(EventHandler<Content> onSaveHandler)
        {
            ContentSaver.OnSave += onSaveHandler;
            return this;
        }

        public Loader AddOnSaveErrorHandler(EventHandler<Exception> onSaveErrorHandler)
        {
            ContentSaver.OnSaveError += onSaveErrorHandler;
            return this;
        }

        public Loader AddOnContentDownloadHandler(EventHandler<ContentMetadata> onDownloadHandler)
        {
            ContentDownloader.OnDownload += onDownloadHandler;
            return this;
        }

        public Loader AddOnContentDownloadErrorHandler(EventHandler<Exception> onDownloadErrorHandler)
        {
            ContentDownloader.OnDownloadError += onDownloadErrorHandler;
            return this;
        }

        public Loader AddOnGetContentMetadataHandler(EventHandler<int> onGetContentMetadataHandler)
        {
            ApiAdapter.OnGetContentMetadata += onGetContentMetadataHandler;
            return this;
        }

        public Loader AddOnGetContentMetadataErrorHandler(EventHandler<Exception> onGetContentMetadataErrorHandler)
        {
            ApiAdapter.OnGetContentMetadataError += onGetContentMetadataErrorHandler;
            return this;
        }

        public Loader AddOnDownloadFinishedHandler(EventHandler onDownloadFinishedHandler)
        {
            OnDownloadFinishedHandler += onDownloadFinishedHandler;
            return this;
        }

        public abstract void Download();

        protected virtual void InvokeOnAlreadyExistItemsFiltered(int filteredItemsCount) =>
            _onAlreadyExistItemsFiltered?.Invoke(this, filteredItemsCount);

        public static Loader CreateLoader(string request, string savePath)
        {
            if (!Uri.IsWellFormedUriString(request, UriKind.Absolute)) return null;

            var apiAdapter = ResolveAdapter(request);

            var contentSaver = GetContentSaver(apiAdapter.RequestName, savePath);
          
            var metadataRepository = contentSaver.GetContentMetadataRepository();
            var contentDownloader = new HttpDownloader();

            if (apiAdapter.ParallelDownloadSupported)
                return new ParallelLoader(contentDownloader, apiAdapter, contentSaver, metadataRepository);
            else
                return new SynchronousLoader(contentDownloader, apiAdapter, contentSaver, metadataRepository);
        }

        private static IContentSaver GetContentSaver(string requestName, string savePath)
        {
            if (savePath == "g")
                return new GoogleSaver(requestName);
            
            var path = Path.Combine(savePath, requestName);
            return new FileSaver(path);
        }

        private static IApiAdapter ResolveAdapter(string request)
        {
            var host = new Uri(request);
            switch (host.Authority)
            {
                case DanbooruAdapter.HostName:
                    return new DanbooruAdapter(request);
                case DvachAdapter.HostName:
                    return new DvachAdapter(request);
                case AnonIbAdapter.HostName:
                    return new AnonIbAdapter(request);
                case ImgurAdapter.HostName:
                    return new ImgurAdapter(request);
                default:
                    throw new ArgumentOutOfRangeException(nameof(request), request, null);
            }
        }
        
        
    }
}
