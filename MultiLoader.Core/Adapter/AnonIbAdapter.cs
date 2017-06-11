using MultiLoader.Core.Abstraction;
using System;
using System.Collections.Generic;
using MultiLoader.Core.Model;
using System.Net.Http;
using Newtonsoft.Json;
using System.Linq;
using MultiLoader.Core.Adapter.Responces;
using MultiLoader.Core.Tool;

namespace MultiLoader.Core.Adapter
{
    public class AnonIbAdapter : IApiAdapter
    {
        private const string BaseUrl = "http://anon-ib.co";

        public event EventHandler<Exception> OnGetContentMetadataError;
        public event EventHandler<int> OnGetContentMetadata;
        public bool ParallelDownloadSupported { get; } = true;

        private readonly HttpClient _httpClient;

        public AnonIbAdapter()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
        }

        public IEnumerable<ContentMetadata> GetContentMetadata(string searchRequest)
        {
            var result = new List<ContentMetadata>();

            try
            {
                var board = searchRequest.Split('_')[0];
                var thread = searchRequest.Split('_')[1];

                var postsString = _httpClient.GetStringAsync($"{BaseUrl}/{board}/res/{thread}.json").Result;
                var posts = JsonConvert.DeserializeObject<AnonIbResponce>(postsString).posts;

                foreach (var post in posts)
                {
                    if (post.tim == null) continue;

                    if (post.tim.Contains("-"))
                    {
                        int counter = -1;
                        bool resp;
                        do
                        {
                            counter++;
                            var fileName = post.tim.Replace("-0", "");
                            resp = _httpClient
                                .GetAsync($"{BaseUrl}/{board}/src/{fileName}-{counter}{post.ext}")
                                .Result
                                .IsSuccessStatusCode;
                            

                        } while (resp);

                        var contentList = counter.For(x => new ContentMetadata
                        {
                            Name = $"{post.tim.Replace("-0", "")}-{x}{post.ext}",
                            Request = $"anonib_{board}_{thread}",
                            SourceType = SourceType.AnonIb,
                            Uri = new Uri($"{BaseUrl}/{board}/src/{post.tim.Replace("-0", "")}-{x}{post.ext}")
                        });
                        
                        result.AddRange(contentList);
                        continue;
                    }

                    result.Add(new ContentMetadata
                    {
                        Name = $"{post.tim}{post.ext}",
                        Request = $"anonib_{board}_{thread}",
                        SourceType = SourceType.AnonIb,
                        Uri = new Uri($"{BaseUrl}/{board}/src/{post.tim}{post.ext}")
                    });


                }

                OnGetContentMetadata?.Invoke(this, result.Count());
            }
            catch (Exception ex)
            {
                OnGetContentMetadataError?.Invoke(this, ex);
                return new List<ContentMetadata>();
            }

            return result;
        }
    }
}
