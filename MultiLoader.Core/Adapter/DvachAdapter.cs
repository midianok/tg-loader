using System;
using System.Collections.Generic;
using MultiLoader.Core.Model;
using System.Net.Http;
using Newtonsoft.Json;
using System.Linq;
using MultiLoader.Core.Adapter.Responces;
using MultiLoader.Core.Infrustructure;

namespace MultiLoader.Core.Adapter
{
    public class DvachAdapter : IApiAdapter
    {
        public const string HostName = "2ch.hk";
        private const string BaseUrl = "https://2ch.hk";

        public event EventHandler<Exception> OnGetContentMetadataError;
        public event EventHandler<int> OnGetContentMetadata;
        public bool ParallelDownloadSupported { get; } = true;
        public string RequestName => $"2ch_{_board}_{_thread}";

        private readonly HttpClient _httpClient;
        private readonly string _board;
        private readonly string _thread;

        public DvachAdapter(string request)
        {
            _board = request.Split('/')[3];
            _thread = request.Split('/')[5].Replace(".html", string.Empty);
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        public IEnumerable<ContentMetadata> GetContentMetadata()
        {
            IEnumerable<ContentMetadata> result;

            try
            {

                var postsString = _httpClient.GetStringAsync($"{BaseUrl}/{_board}/res/{_thread}.json").Result;
                result = JsonConvert.DeserializeObject<DvachPost>(postsString)
                .Threads
                .SelectMany(x => x.Posts)
                .SelectMany(x => x.Files)
                .Select(x => new ContentMetadata
                {
                    Name = x.Name,
                    Uri = new Uri(BaseUrl + x.Path),
                    Request = $"2ch_{_board}_{_thread}",
                    SourceType = SourceType.Dvach
                }); 

                OnGetContentMetadata?.Invoke(this, result.Count());
            }
            catch(Exception ex)
            {
                OnGetContentMetadataError?.Invoke(this, ex);
                return new List<ContentMetadata>();
            }

            return result;
                
        }
    }
}
