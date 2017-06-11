using System;
using MultiLoader.Core.Adapter;
using MultiLoader.Core.Db;
using MultiLoader.Core.Model;
using MultiLoader.Core.Services;

namespace MultiLoader.Core.Abstraction
{
    public abstract class Loader
    {
        protected readonly IApiAdapter ApiAdapter;
        protected readonly IContentDownloader ContentDownloader;
        protected readonly IContentSaver ContentSaver;
        protected readonly IRepository<ContentMetadata> ContentMetadataRepository;

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

        public abstract void Download(string request);

        public static Loader CreateLoader(SourceType source, string savePath)
        {
            var apiAdapter = ResolveAdapter(source);
            var metadataRepository = new LiteDbRepository<ContentMetadata>(savePath);
            var fileSaver = new FileSaver(savePath);
            var contentDownloader = new HttpDownloader();

            if (apiAdapter.ParallelDownloadSupported)
                return new ParallelLoader(contentDownloader, apiAdapter, fileSaver, metadataRepository);
            else
                return new SynchronousLoader(contentDownloader, apiAdapter, fileSaver, metadataRepository);
        }

        private static IApiAdapter ResolveAdapter(SourceType source)
        {
            switch (source)
            {
                case SourceType.Danbooru:
                    return new DanbooruAdapter();
                case SourceType.Dvach:
                    return new DvachAdapter();
                case SourceType.AnonIb:
                    return new AnonIbAdapter();
                case SourceType.Imgur:
                    return new ImgurAdapter();
                default:
                    throw new ArgumentOutOfRangeException(nameof(source), source, null);
            }
        }
    }
}
