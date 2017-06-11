using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using MultiLoader.Core.Abstraction;
using MultiLoader.Core.Adapter.Responces;
using MultiLoader.Core.Model;
using Newtonsoft.Json;

namespace MultiLoader.Core.Adapter
{
    public class DanbooruAdapter : IApiAdapter
    {
        private const string BaseUrl = "http://danbooru.donmai.us";

        public event EventHandler<Exception> OnGetContentMetadataError;
        public event EventHandler<int> OnGetContentMetadata;
        public bool ParallelDownloadSupported { get; } = false;

        private readonly HttpClient _httpClient;

        public DanbooruAdapter()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        public IEnumerable<ContentMetadata> GetContentMetadata(string searchRequest)
        {
            var postMetadatas = new List<DanbooruResponce>();
            var page = 0;

            while (true)
            {
                try
                {
                    var postsResponce = _httpClient.GetStringAsync($"/posts.json?limit=200&page={page++}&tags={searchRequest}").Result;
                    var posts = JsonConvert.DeserializeObject<IEnumerable<DanbooruResponce>>(postsResponce).ToList();
                    if (!posts.Any()) break;
                    postMetadatas.AddRange(posts);
                }
                catch(Exception ex)
                {
                    OnGetContentMetadataError?.Invoke(this, ex);
                    return new List<ContentMetadata>();
                }
            }

            OnGetContentMetadata?.Invoke(this, postMetadatas.Count);

            return postMetadatas
                .Where(x => !string.IsNullOrEmpty(x.LargeFileUrl))
                .Select(post => new ContentMetadata
                {
                    Name = post.LargeFileUrl.Split('/').Last(),
                    Request = searchRequest,
                    SourceType = SourceType.Danbooru,
                    Uri = new Uri(BaseUrl + post.LargeFileUrl),
                }).ToList();


        }
    }
}
