using Newtonsoft.Json;

namespace MultiLoader.Core.Adapter.Responces
{
    class DanbooruResponce
    {
        [JsonProperty(PropertyName = "large_file_url")]
        public string LargeFileUrl { get; set; } = string.Empty;
    }
}
