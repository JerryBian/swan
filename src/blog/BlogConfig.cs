using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Share;

namespace Laobian.Blog
{
    public class BlogConfig : CommonConfig
    {
        public int PostsPerPage { get; set; }

        public string AdminChineseName { get; set; }

        public string AdminEnglishName { get; set; }

        public string GitHubHookSecret { get; set; }
    }
}
