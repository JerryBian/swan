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

        public string AppVersion
        {
            get
            {
                var ver = Assembly.GetEntryAssembly()?.GetName().Version;
                if (ver == null)
                {
                    return "1.0";
                }
                else
                {
                    return $"{ver.Major}.{ver.Minor}";
                }
            }
        }

        public DateTime BootTime { get;  }

        public string RuntimeVersion { get;  }
    }
}
