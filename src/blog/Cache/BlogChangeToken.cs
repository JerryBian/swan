using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace Laobian.Blog.Cache
{
    public class BlogChangeToken : IChangeToken
    {
        private readonly ISystemData _systemData;
        private readonly DateTime _assetLastUpdate;
        private readonly DateTime? _nextHardRefreshAt;

        public BlogChangeToken(ISystemData systemData)
        {
            _systemData = systemData;
            _assetLastUpdate = systemData.LastLoadTimestamp;
            _nextHardRefreshAt = systemData.Posts.FirstOrDefault(p => p.Metadata.PublishTime > DateTime.Now)?.Metadata.PublishTime;
        }

        public IDisposable RegisterChangeCallback(Action<object> callback, object state)
        {
            return null;
        }

        public bool ActiveChangeCallbacks { get; } = false;

        public bool HasChanged
        {
            get
            {
                if (_assetLastUpdate != _systemData.LastLoadTimestamp)
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
