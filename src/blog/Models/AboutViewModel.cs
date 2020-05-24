using System.Collections.Generic;
using Laobian.Share.Blog.Model;

namespace Laobian.Blog.Models
{
    public class AboutViewModel
    {
        #region System
        public string SystemLastBoot { get; set; }

        public string SystemRunningInterval { get; set; }

        public string SystemDotNetVersion { get; set; }

        public string SystemAppVersion { get; set; }

        #endregion

        #region Post

        public BlogPost LatestPost { get; set; }

        public string PostTotalCount { get; set; }

        public string PostTotalAccessCount { get; set; }

        public IEnumerable<BlogPost> TopPosts { get; set; }

        public string TagTotalCount { get; set; }

        public IEnumerable<BlogTag> TopTags { get; set; }

        public string CategoryTotalCount { get; set; }

        public IEnumerable<BlogCategory> TopCategories { get; set; }

        #endregion
    }
}
