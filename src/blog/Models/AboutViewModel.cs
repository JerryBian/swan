using System.Collections.Generic;
using Laobian.Share.Model.Blog;

namespace Laobian.Blog.Models;

public class AboutViewModel
{
    #region System

    public string SystemLastBoot { get; set; }

    public string SystemRunningInterval { get; set; }

    public string SystemDotNetVersion { get; set; }

    public string SystemAppVersion { get; set; }

    #endregion

    #region Post

    public BlogPostRuntime LatestPostRuntime { get; set; }

    public string PostTotalCount { get; set; }

    public string PostTotalAccessCount { get; set; }

    public IEnumerable<BlogPostRuntime> TopPosts { get; set; }

    public string TagTotalCount { get; set; }

    public IDictionary<BlogTag, int> TopTags { get; set; }

    #endregion
}