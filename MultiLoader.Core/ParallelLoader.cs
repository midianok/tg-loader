using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using MultiLoader.Core.Infrustructure;
using MultiLoader.Core.Model;

namespace MultiLoader.Core
{
    public class ParallelLoader : Loader
    {
        private readonly ConcurrentBag<ContentMetadata> _savedMetadata; 
        public ParallelLoader(
            IContentDownloader contentDownloader,
            IApiAdapter apiAdapter,
            IContentSaver contentSaver,
            IRepository<ContentMetadata> contentMetadataRepository)
            : base(contentDownloader, apiAdapter, contentSaver, contentMetadataRepository)
        {
            _savedMetadata = new ConcurrentBag<ContentMetadata>();
            ContentSaver.OnSave += (sender, content) => _savedMetadata.Add(content.ContentMetadata);
        }

        public override void Download()
        {
            var filteredMetadata = ApiAdapter
                .GetContentMetadata()
                .Except(ContentMetadataRepository.GetAll()).ToArray();

            InvokeOnAlreadyExistItemsFiltered(filteredMetadata.Length);

            Parallel.ForEach(
                filteredMetadata,
                new ParallelOptions { MaxDegreeOfParallelism = 5 },
                metadata =>
                {
                    var downloadedContent = ContentDownloader.DownloadContent(metadata);
                    ContentSaver.SaveContent(downloadedContent);
                });

            ContentMetadataRepository.AddRange(_savedMetadata);
        }

    }
}