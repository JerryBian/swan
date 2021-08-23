using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Laobian.Share.Blog;

namespace Laobian.Blog.Service
{
    public interface IBlogService
    {
        string AppVersion { get; }

        DateTime BootTime { get; }

        string RuntimeVersion { get; }
        List<BlogPostRuntime> GetAllPosts();

        List<BlogTag> GetAllTags();

        Task ReloadAsync();

        DateTime GetLastReloadTime();
    }
}