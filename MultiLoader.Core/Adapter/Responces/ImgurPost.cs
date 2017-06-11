using System.Collections.Generic;

namespace MultiLoader.Core.Adapter.Responces
{
    public class ImgurPost
    {
        public Data data { get; set; }

        public class Image
        {
            public string link { get; set; }
            public string id { get; set; }
        }

        public class Data
        {
            public List<Image> images { get; set; }
        }
            
    }
}
