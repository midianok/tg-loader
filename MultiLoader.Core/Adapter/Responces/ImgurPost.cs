using System.Collections.Generic;
using Newtonsoft.Json;

namespace MultiLoader.Core.Adapter.Responces
{
    public class ImgurPost
    {
        [JsonProperty(PropertyName = "data")]
        public Data Data { get; set; }
    }

    public class Image
    {
        [JsonProperty(PropertyName = "link")]
        public string Link { get; set; }
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }

    public class Data
    {
        [JsonProperty(PropertyName = "images")]
        public List<Image> Images { get; set; }
    }
}
