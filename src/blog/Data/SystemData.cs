using System;
using System.Collections.Generic;
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
        private readonly ILogger<SystemData> _logger;
        private readonly ApiHttpService _apiHttpService;
        private readonly List<BlogTag> _tags;
        private readonly List<BlogPost> _posts;
        private readonly ManualResetEventSlim _manualResetEventSlim;

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
                Posts.Clear();
                Posts.AddRange(await _apiHttpService.GetPostsAsync(false));

                Tags.Clear();
                Tags.AddRange(await _apiHttpService.GetTagsAsync());

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