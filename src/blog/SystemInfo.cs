using System;
using System.Reflection;
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
            RuntimeVersion = RuntimeInformation.FrameworkDescription;
        }

        public string AppVersion => Assembly.GetEntryAssembly()?.GetName().Version?.Major.ToString() ?? "1.0";

        public DateTime BootTime { get;  }

        public string RuntimeVersion { get;  }
    }
}
