﻿using System;
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
        protected string Request;
        private EventHandler<int> OnAlreadyExistItemsFiltered;

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
            OnAlreadyExistItemsFiltered += onAlreadyExistItemsFilteredHandler;
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

        public abstract void Download();

        protected virtual void InvokeOnAlreadyExistItemsFiltered(int filteredItemsCount) =>
            OnAlreadyExistItemsFiltered?.Invoke(this, filteredItemsCount);

        public static Loader CreateLoader(string request, string savePath)
        {
            var apiAdapter = ResolveAdapter(request);
            var path = Path.Combine(savePath, apiAdapter.RequestName);
            var metadataRepository = new LiteDbRepository<ContentMetadata>(path);
            var fileSaver = new FileSaver(path);
            var contentDownloader = new HttpDownloader();

            if (apiAdapter.ParallelDownloadSupported)
                return new ParallelLoader(contentDownloader, apiAdapter, fileSaver, metadataRepository);
            else
                return new SynchronousLoader(contentDownloader, apiAdapter, fileSaver, metadataRepository);
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