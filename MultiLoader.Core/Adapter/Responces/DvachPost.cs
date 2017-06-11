using System.Collections.Generic;
using Newtonsoft.Json;

namespace MultiLoader.Core.Adapter.Responces
{
    class DvachPost
    {
        [JsonProperty(PropertyName = "threads")]
        public IList<Thread> Threads { get; set; }

        public class Thread
        {
            [JsonProperty(PropertyName = "posts")]
            public IList<Post> Posts { get; set; }
        }

        public class Post
        {
            [JsonProperty(PropertyName = "files")]
            public IList<DvachFile> Files { get; set; }
        }

        public class DvachFile
        {
            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }
            [JsonProperty(PropertyName = "path")]
            public string Path { get; set; }
        }

    }
}
