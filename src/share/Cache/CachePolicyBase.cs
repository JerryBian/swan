using System;
using System.Collections.Generic;
using System.Text;

namespace Laobian.Share.Cache
{
    public class CachePolicyBase : ICachePolicy
    {
        public CacheState State { get; private set; }
        public virtual TimeSpan? ExpirationRelativeToNow => null;

        public void Expire()
        {
            State = CacheState.Expire;
        }

        public void Cache()
        {
            State = CacheState.InCache;
        }

        public bool NeedExpire()
        {
            return State == CacheState.Expire;
        }
    }
}
