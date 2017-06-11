using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MultiLoader.Core.Abstraction;
using MultiLoader.Core.Tool;

namespace MultiLoader.Core
{
    public class SynchronousLoader : LoaderBase
    {
        public SynchronousLoader(
            IContentDownloader contentDownloader, 
            IApiAdapter apiAdapter, 
            IContentSaver contentSaver, 
            IContentMetadataRepository contentMetadataRepository) 
            : base(contentDownloader, apiAdapter, contentSaver, contentMetadataRepository) {}

        public override void Download(string request)
        {
            ApiAdapter
                    .GetContentMetadata(request)
                    .Except(ContentMetadataRepository.GetAll())
                    .Select(ContentDownloader.DownloadContent)
                    .Each(ContentSaver.SaveContent);
        }
            
    }
}
