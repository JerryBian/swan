using System;
using System.Runtime.InteropServices;
using Laobian.Share;
using Microsoft.Extensions.Options;

namespace Laobian.Blog
{
    public class SystemInfo : ISystemInfo
    {
        public SystemInfo(IOptions<BlogConfig> config)
        {
            BootTime = DateTime.Now;
            AppVersion = config.Value.AppVersion;
            RuntimeVersion = RuntimeInformation.FrameworkDescription;
        }

        public string AppVersion { get; }

        public DateTime BootTime { get;  }

        public string RuntimeVersion { get;  }
    }
}
