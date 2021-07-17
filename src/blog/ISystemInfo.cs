using System;

namespace Laobian.Blog
{
    public interface ISystemInfo
    {
        string AppVersion { get;  }

        DateTime BootTime { get;  }

        string RuntimeVersion { get; }
    }
}
