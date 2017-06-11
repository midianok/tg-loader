using System.Linq;
using System.Threading.Tasks;
using MultiLoader.Core.Abstraction;
using MultiLoader.Core.Tool;

namespace MultiLoader.Core
{
    public class ParallelLoader : LoaderBase
    {
        public ParallelLoader(
            IContentDownloader contentDownloader,
            IApiAdapter apiAdapter,
            IContentSaver contentSaver,
            IContentMetadataRepository contentMetadataRepository) 
            : base(contentDownloader, apiAdapter, contentSaver, contentMetadataRepository) { }

        public override void Download(string request)
        {
            var filteredMetadata = ApiAdapter
                    .GetContentMetadata(request)
                    .Except(ContentMetadataRepository.GetAll());

            Parallel.ForEach(
                filteredMetadata,
                new ParallelOptions { MaxDegreeOfParallelism = 5 },
                metadata =>
                {
                    var t = ContentDownloader.DownloadContent(metadata);
                    ContentSaver.SaveContent(t);
                });

        }

    }
}