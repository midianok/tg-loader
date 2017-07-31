using System;
using System.Collections.Generic;
using MultiLoader.Core.Model;
using System.Linq;
using HtmlAgilityPack;
using MultiLoader.Core.Infrustructure;

namespace MultiLoader.Core.Adapter
{
    public class AnonIbAdapter : IApiAdapter
    {
        public const string HostName = "anon-ib.co";
        private const string BaseUrl = "http://anon-ib.co";

        public event EventHandler<Exception> OnGetContentMetadataError;
        public event EventHandler<int> OnGetContentMetadata;
        public bool ParallelDownloadSupported { get; } = true;
        public string RequestName => $"anonib_{_board}_{_thread}";

        private readonly string _board;
        private readonly string _thread;

        public AnonIbAdapter(string request)
        {
            _board = request.Split('/')[3];
            _thread = request.Split('/')[5].Replace(".html", string.Empty);
        }

        public IEnumerable<ContentMetadata> GetContentMetadata()
        {
            try
            {
                var web = new HtmlWeb();
                var threadHtml = web.Load($"{BaseUrl}/{_board}/res/{_thread}.html");

                var urls = threadHtml.DocumentNode
                    .Descendants("p")
                    .Where(x => x.Attributes["class"].Value == "fileinfo")
                    .Select(x => x.Descendants("a").First())
                    .Select(x => $"http://anon-ib.co{x.Attributes["href"].Value}")
                    .Select(uri => new ContentMetadata
                    {
                        Name = uri.Split('/').Last(),
                        Request = $"anonib_{_board}_{_thread}",
                        SourceType = SourceType.AnonIb,
                        Uri = new Uri(uri)
                    }).ToArray();

                OnGetContentMetadata?.Invoke(this, urls.Length);
                return urls;
            }
            catch (Exception exception)
            {
                OnGetContentMetadataError?.Invoke(this, exception);
            }
            return null;
        }
    }
}
