using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Laobian.Share.Blog;
using Laobian.Share.Read;

namespace Laobian.Blog.Service
{
    public interface IBlogService
    {
        string AppVersion { get; }

        DateTime BootTime { get; }

        string RuntimeVersion { get; }
        List<BlogPostRuntime> GetAllPosts();

        List<BlogTag> GetAllTags();

        List<BookItem> GetBookItems();

        Task ReloadAsync();

        DateTime GetLastReloadTime();
    }
}