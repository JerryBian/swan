using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Laobian.Share.Blog;

namespace Laobian.Blog
{
    public interface ISystemData
    {
        string AppVersion { get; }

        DateTime BootTime { get; }

        string RuntimeVersion { get; }

        List<BlogPost> Posts { get; }

        List<BlogTag> Tags { get; }

        DateTime LastLoadTimestamp { get; }

        Task LoadAsync();
    }
}