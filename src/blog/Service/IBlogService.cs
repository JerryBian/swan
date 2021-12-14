using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Laobian.Share.Site.Blog;
using Laobian.Share.Site.Read;

namespace Laobian.Blog.Service;

public interface IBlogService
{
    DateTime BootTime { get; }

    List<BlogPostRuntime> GetAllPosts();

    List<BlogTag> GetAllTags();

    List<ReadItemRuntime> GetReadItems();

    Task ReloadAsync();

    DateTime GetLastReloadTime();

    void EnqueuePostAccess(string link);

    bool TryDequeuePostAccess(out string link);
}