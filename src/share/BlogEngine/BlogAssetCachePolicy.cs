using System;
using System.Collections.Generic;
using System.Text;
using Laobian.Share.Cache;

namespace Laobian.Share.BlogEngine
{
    public class BlogAssetCachePolicy : CachePolicyBase
    {
        private BlogAssetCachePolicy() { }

        private static readonly Lazy<BlogAssetCachePolicy> DefaultInstance = new Lazy<BlogAssetCachePolicy>(() => new BlogAssetCachePolicy(), true);

        public static BlogAssetCachePolicy Instance => DefaultInstance.Value;
    }
}
