using System;
using System.Collections.Generic;
using System.Text;

namespace MultiLoader.Core.Model
{
    public class AnonIbPost
    {
        public List<Post> posts { get; set; }

        public class Post
        {
            public int no { get; set; }
            public string sub { get; set; }
            public string com { get; set; }
            public string name { get; set; }
            public int time { get; set; }
            public int omitted_posts { get; set; }
            public int omitted_images { get; set; }
            public int sticky { get; set; }
            public int locked { get; set; }
            public int last_modified { get; set; }
            public int tn_h { get; set; }
            public int tn_w { get; set; }
            public int h { get; set; }
            public int w { get; set; }
            public int fsize { get; set; }
            public string filename { get; set; }
            public string ext { get; set; }
            public string tim { get; set; }
            public string md5 { get; set; }
            public int resto { get; set; }
            public string email { get; set; }
        }

        
    }
}
