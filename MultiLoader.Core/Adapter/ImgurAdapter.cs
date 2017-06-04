using MultiLoader.Core.Abstraction;
using System;
using System.Collections.Generic;
using System.Text;
using MultiLoader.Core.Model;
using Newtonsoft.Json;
using System.Net.Http;
using System.Linq;

namespace MultiLoader.Core.Adapter
{
    public class ImgurAdapter : IApiAdapter
    {
        private const string BaseUrl = "https://api.imgur.com";
        private readonly HttpClient _httpClient;

        public event EventHandler<Exception> OnGetContentMetadataError;
        public event EventHandler<int> OnGetContentMetadata;

        public ImgurAdapter()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Client-ID cdd0ed30ebd0d0f");
        }

        public IEnumerable<ContentMetadata> GetContentMetadata(string searchRequest)
        {
            IEnumerable<ContentMetadata> result;

            try
            {
                var postsString = _httpClient.GetStringAsync($"{BaseUrl}/3/album/{searchRequest}").Result;

                result = JsonConvert.DeserializeObject<ImgurPost>(postsString)
                    .data
                    .images
                    .Select(x => new ContentMetadata
                    {
                        Name = x.id + '.' + x.link.Split('.').Last(),
                        Uri = new Uri(x.link),
                        Request = $"imgur_{searchRequest}",
                        SourceType = SourceType.Imgur
                    });

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
