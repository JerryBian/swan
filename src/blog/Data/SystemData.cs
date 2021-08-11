using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Blog.HttpService;
using Laobian.Share.Blog;
using Microsoft.Extensions.Logging;

namespace Laobian.Blog.Data
{
    public class SystemData : ISystemData
    {
        private readonly ApiHttpService _apiHttpService;
        private readonly ILogger<SystemData> _logger;
        private readonly ManualResetEventSlim _manualResetEventSlim;
        private readonly List<BlogPost> _posts;
        private readonly List<BlogTag> _tags;

        public SystemData(ILogger<SystemData> logger, ApiHttpService apiHttpService)
        {
            _logger = logger;
            BootTime = DateTime.Now;
            _apiHttpService = apiHttpService;
            _tags = new List<BlogTag>();
            _posts = new List<BlogPost>();
            RuntimeVersion = RuntimeInformation.FrameworkDescription;
            _manualResetEventSlim = new ManualResetEventSlim(true);
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

        public List<BlogPost> Posts
        {
            get
            {
                _manualResetEventSlim.Wait();
                return _posts;
            }
        }

        public List<BlogTag> Tags
        {
            get
            {
                _manualResetEventSlim.Wait();
                return _tags;
            }
        }

        public DateTime LastLoadTimestamp { get; private set; }

        public async Task LoadAsync()
        {
            try
            {
                _manualResetEventSlim.Reset();
                _logger.LogInformation("Start to load system data.");
                var posts = await _apiHttpService.GetPostsAsync();
                var tags = await _apiHttpService.GetTagsAsync();

                _posts.Clear();
                _posts.AddRange(posts.OrderByDescending(x => x.Metadata.PublishTime));

                _tags.Clear();
                _tags.AddRange(tags);

                LastLoadTimestamp = DateTime.Now;
                _logger.LogInformation("End to load system data.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "System data load failed.");
            }
            finally
            {
                _manualResetEventSlim.Set();
            }
        }
    }
}