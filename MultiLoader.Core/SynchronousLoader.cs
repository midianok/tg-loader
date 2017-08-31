using System;
using System.Linq;
using MultiLoader.Core.Infrustructure;
using MultiLoader.Core.Model;
using MultiLoader.Core.Tool;

namespace MultiLoader.Core
{
    public class SynchronousLoader : Loader
    {
        public SynchronousLoader(
            IContentDownloader contentDownloader,
            IApiAdapter apiAdapter,
            IContentSaver contentSaver,
            IRepository<ContentMetadata> contentMetadataRepository)
            : base(contentDownloader, apiAdapter, contentSaver, contentMetadataRepository)
        {
            ContentSaver.OnSave += (obj, content) => ContentMetadataRepository.Add(content.ContentMetadata);
        }

        public override void Download()
        {
            var itemsToAdd = ApiAdapter
                .GetContentMetadata()
                .Except(ContentMetadataRepository.GetAll()).ToArray();

            InvokeOnAlreadyExistItemsFiltered(itemsToAdd.Length);

            itemsToAdd
                .Select(ContentDownloader.DownloadContent)
                .Each(ContentSaver.SaveContent);

            OnDownloadFinishedHandler?.Invoke(this, EventArgs.Empty);
        }
            
    }
}
