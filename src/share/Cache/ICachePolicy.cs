using System;
using System.Collections.Generic;
using System.Text;

namespace Laobian.Share.Cache
{
    public interface ICachePolicy
    {
        CacheState State { get; }

        TimeSpan? ExpirationRelativeToNow { get; }

        void Expire();

        void Cache();

        bool NeedExpire();
    }
}
