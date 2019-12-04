using System;
using Laobian.Blog.HostedService;
using Laobian.Share.Blog;
using Microsoft.Extensions.Primitives;

namespace Laobian.Blog
{
    public class BlogAssetChangeToken : IChangeToken
    {
        private DateTime _nextRefreshAt;
        private readonly DateTime _assetLastUpdate;

        public BlogAssetChangeToken()
        {
            _assetLastUpdate = BlogState.AssetLastUpdate;
            AssetHostedService.BlogAssetChangeEvent += (sender, change) => { _nextRefreshAt = change.NextRefreshAt; };
        }

        public IDisposable RegisterChangeCallback(Action<object> callback, object state)
        {
            throw new NotImplementedException();
        }

        public bool HasChanged
        {
            get
            {
                if (_assetLastUpdate == BlogState.AssetLastUpdate)
                {
                    return true;
                }

                if (_nextRefreshAt != default && DateTime.Now >= _nextRefreshAt)
                {
                    return true;
                }

                return false;
            }
        }

        public bool ActiveChangeCallbacks => false;
    }
}