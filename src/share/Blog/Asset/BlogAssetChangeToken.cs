using System;
using Microsoft.Extensions.Primitives;

namespace Laobian.Share.Blog.Asset
{
    public class BlogAssetChangeToken : IChangeToken
    {
        private readonly DateTime _assetLastUpdate;

        public BlogAssetChangeToken()
        {
            _assetLastUpdate = BlogState.AssetLastUpdate;
        }

        public IDisposable RegisterChangeCallback(Action<object> callback, object state)
        {
            throw new NotImplementedException();
        }

        public bool HasChanged => _assetLastUpdate != BlogState.AssetLastUpdate;

        public bool ActiveChangeCallbacks => false;
    }
}
