using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Laobian.Share.Blog;

namespace Laobian.Blog.Data
{
    public interface ISystemData
    {
        string AppVersion { get; }

        DateTime BootTime { get; }

        string RuntimeVersion { get; }

        List<BlogPostRuntime> Posts { get; }

        List<BlogTag> Tags { get; }

        DateTime LastLoadTimestamp { get; }

        Task LoadAsync();
    }
}