using System;
using System.Linq;
using Laobian.Blog.Service;
using Microsoft.Extensions.Primitives;

namespace Laobian.Blog.Cache
{
    public class BlogChangeToken : IChangeToken
    {
        private readonly DateTime _assetLastUpdate;
        private readonly IBlogService _blogService;
        private readonly DateTime? _nextHardRefreshAt;

        public BlogChangeToken(IBlogService blogService)
        {
            _blogService = blogService;
            _assetLastUpdate = blogService.GetLastReloadTime();
            _nextHardRefreshAt = blogService.GetAllPosts().FirstOrDefault(p => p.Raw.PublishTime > DateTime.Now)?.Raw
                .PublishTime;
        }

        public IDisposable RegisterChangeCallback(Action<object> callback, object state)
        {
            return null;
        }

        public bool ActiveChangeCallbacks => false;

        public bool HasChanged
        {
            get
            {
                if (_assetLastUpdate != _blogService.GetLastReloadTime())
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
    }
}