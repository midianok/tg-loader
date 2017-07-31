using System;
using System.Collections.Generic;
using MultiLoader.Core.Model;
using Newtonsoft.Json;
using System.Net.Http;
using System.Linq;
using MultiLoader.Core.Adapter.Responces;
using MultiLoader.Core.Infrustructure;

namespace MultiLoader.Core.Adapter
{
    public class ImgurAdapter : IApiAdapter
    {
        public const string HostName = "imgur.com";
        private const string BaseUrl = "https://api.imgur.com";

        public event EventHandler<Exception> OnGetContentMetadataError;
        public event EventHandler<int> OnGetContentMetadata;
        public bool ParallelDownloadSupported { get; } = true;
        public string RequestName => $"imgur_{_albumId}";
        
        private readonly HttpClient _httpClient;
        private readonly string _albumId;

        public ImgurAdapter(string request)
        {
            _albumId = request.Split('/').Last();
            _httpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
            _httpClient.DefaultRequestHeaders.Add("Authorization", "Client-ID cdd0ed30ebd0d0f");
        }

        public IEnumerable<ContentMetadata> GetContentMetadata()
        {
            IEnumerable<ContentMetadata> result;

            try
            {
                var postsString = _httpClient.GetStringAsync($"{BaseUrl}/3/album/{_albumId}").Result;

                result = JsonConvert.DeserializeObject<ImgurPost>(postsString)
                    .Data
                    .Images
                    .Select(x => new ContentMetadata
                    {
                        Name = x.Id + '.' + x.Link.Split('.').Last(),
                        Uri = new Uri(x.Link),
                        Request = $"imgur_{_albumId}",
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
