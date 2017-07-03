using System.Linq;
using MultiLoader.Core.Abstraction;
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

        public override void Download(string request)
        {
            var itemsToAdd = ApiAdapter
                .GetContentMetadata(request)
                .Except(ContentMetadataRepository.GetAll()).ToArray();

            InvokeOnAlreadyExistItemsFiltered(itemsToAdd.Length);

            itemsToAdd
                .Select(ContentDownloader.DownloadContent)
                .Each(ContentSaver.SaveContent);
        }
            
    }
}
