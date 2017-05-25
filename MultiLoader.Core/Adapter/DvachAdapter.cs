using MultiLoader.Core.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;
using MultiLoader.Core.Model;
using System.Net.Http;
using Newtonsoft.Json;
using System.Linq;

namespace MultiLoader.Core.Adapter
{
    public class DvachAdapter : IApiAdapter
    {
        private const string BaseUrl = "https://2ch.hk";
        private readonly HttpClient _httpClient;

        public DvachAdapter()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }
        public IEnumerable<ContentMetadata> GetContentMetadata(string searchRequest)
        {
            var board = searchRequest.Split('_')[0] ;
            var thread = searchRequest.Split('_')[1];

            var postsString = _httpClient.GetStringAsync($"{BaseUrl}/{board}/res/{thread}.json").Result;
            var result = JsonConvert.DeserializeObject<DvachPost>(postsString);

            return result
                .Threads
                .SelectMany(x => x.Posts)
                .SelectMany(x => x.Files)
                .Select(x => new ContentMetadata
                {
                    Name = x.Name,
                    Uri = new Uri(BaseUrl + x.Path),
                    RequestString = searchRequest,

                });
        }
    }
}
