﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using MultiLoader.Core.Abstraction;
using MultiLoader.Core.Model;
using Newtonsoft.Json;

namespace MultiLoader.Core.Adapter
{
    public class DanbooruAdapter : IApiAdapter
    {
        private const string BaseUrl = "http://danbooru.donmai.us";
        private readonly HttpClient _httpClient;

        public DanbooruAdapter()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        public event EventHandler<Exception> OnGetContentMetadataError;
        public event EventHandler<int> OnGetContentMetadata;

        public IEnumerable<ContentMetadata> GetContentMetadata(string searchRequest)
        {
            var postMetadatas = new List<DanbooruPost>();
            var page = 0;

            while (true)
            {
                string postsResponce;
                try
                {
                    postsResponce = _httpClient.GetStringAsync($"/posts.json?limit=200&page={page++}&tags={searchRequest}").Result;
                    var posts = JsonConvert.DeserializeObject<IEnumerable<DanbooruPost>>(postsResponce).ToList();
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
                    RequestString = searchRequest,
                    SourceType = SourceType.Danbooru,
                    Uri = new Uri(BaseUrl + post.LargeFileUrl),
                }).ToList();


        }
    }
}
