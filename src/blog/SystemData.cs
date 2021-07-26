using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Laobian.Blog.HttpService;
using Laobian.Share.Blog;
using Microsoft.Extensions.Options;

namespace Laobian.Blog
{
    public class SystemData : ISystemData
    {
        private readonly ApiHttpService _apiHttpService;

        public SystemData(IOptions<BlogConfig> config, ApiHttpService apiHttpService)
        {
            BootTime = DateTime.Now;
            _apiHttpService = apiHttpService;
            RuntimeVersion = RuntimeInformation.FrameworkDescription;
        }

        public string AppVersion
        {
            get
            {
                var ver = Assembly.GetEntryAssembly()?.GetName().Version;
                if (ver == null)
                {
                    return "1.0";
                }

                return $"{ver.Major}.{ver.Minor}";
            }
        }

        public DateTime BootTime { get; }

        public string RuntimeVersion { get; }

        public List<BlogPost> Posts { get; } = new List<BlogPost>();

        public List<BlogTag> Tags { get; } = new List<BlogTag>();

        public DateTime LastLoadTimestamp { get; private set; }

        public async Task LoadAsync()
        {
            Posts.Clear();
            Posts.AddRange(await _apiHttpService.GetPostsAsync(false));

            Tags.Clear();
            Tags.AddRange(await _apiHttpService.GetTagsAsync());

            LastLoadTimestamp = DateTime.Now;
        }
    }
}