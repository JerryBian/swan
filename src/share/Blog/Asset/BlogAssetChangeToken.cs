using System;
using System.Linq;
using Microsoft.Extensions.Primitives;

namespace Laobian.Share.Blog.Asset
{
    public class BlogAssetChangeToken : IChangeToken
    {
        private readonly DateTime? _nextHardRefreshAt;
        private readonly DateTime _assetLastUpdate;

        public BlogAssetChangeToken()
        {
            _assetLastUpdate = BlogState.AssetLastUpdate;
            _nextHardRefreshAt = BlogState.PostsPublishTime?.Where(p => p > DateTime.Now).FirstOrDefault();
        }

        public IDisposable RegisterChangeCallback(Action<object> callback, object state)
        {
            throw new NotImplementedException();
        }

        public bool HasChanged
        {
            get
            {
                if (_assetLastUpdate != BlogState.AssetLastUpdate)
                {
                    return true;
                }

                if (_nextHardRefreshAt != null && 
                    _nextHardRefreshAt != default(DateTime) && 
                    DateTime.Now >= _nextHardRefreshAt)
                {
                    return true;
                }

                return false;
            }
        }

        public bool ActiveChangeCallbacks => false;
    }
}