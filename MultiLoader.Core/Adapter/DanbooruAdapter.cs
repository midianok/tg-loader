using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using MultiLoader.Core.Adapter.Responces;
using MultiLoader.Core.Infrustructure;
using MultiLoader.Core.Model;
using Newtonsoft.Json;

namespace MultiLoader.Core.Adapter
{
    public class DanbooruAdapter : IApiAdapter
    {
        public const string HostName = "danbooru.donmai.us";
        private const string BaseUrl = "http://danbooru.donmai.us";
        private readonly string _serchTag;

        public event EventHandler<Exception> OnGetContentMetadataError;
        public event EventHandler<int> OnGetContentMetadata;
        public bool ParallelDownloadSupported { get; } = false;
        public string RequestName => $"danbooru_{_serchTag}";

        private readonly HttpClient _httpClient;

        public DanbooruAdapter(string request)
        {
            _serchTag = request.Split('&').First(x => x.Contains("tags")).Split('=').Last();
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        public IEnumerable<ContentMetadata> GetContentMetadata()
        {
            var postMetadatas = new List<DanbooruResponce>();
            var page = 0;

            while (true)
            {
                try
                {
                    var postsResponce = _httpClient.GetStringAsync($"/posts.json?limit=200&page={page++}&tags={_serchTag}").Result;
                    var posts = JsonConvert.DeserializeObject<IEnumerable<DanbooruResponce>>(postsResponce).ToList();
                    if (!posts.Any()) break;
                    postMetadatas.AddRange(posts);
                }
                catch(Exception ex)
                {
                    OnGetContentMetadataError?.Invoke(this, ex);
                    return Enumerable.Empty<ContentMetadata>();
                }
            }

            OnGetContentMetadata?.Invoke(this, postMetadatas.Count);

            return postMetadatas
                .Where(x => !string.IsNullOrEmpty(x.LargeFileUrl))
                .Select(post => new ContentMetadata
                {
                    Name = post.LargeFileUrl.Split('/').Last(),
                    Request = _serchTag,
                    SourceType = SourceType.Danbooru,
                    Uri = new Uri(BaseUrl + post.LargeFileUrl),
                }).ToList();


        }
    }
}
