﻿using System;
using System.Linq;
using Laobian.Blog.Data;
using Microsoft.Extensions.Primitives;

namespace Laobian.Blog.Cache
{
    public class BlogChangeToken : IChangeToken
    {
        private readonly DateTime _assetLastUpdate;
        private readonly DateTime? _nextHardRefreshAt;
        private readonly ISystemData _systemData;

        public BlogChangeToken(ISystemData systemData)
        {
            _systemData = systemData;
            _assetLastUpdate = systemData.LastLoadTimestamp;
            _nextHardRefreshAt = systemData.Posts.FirstOrDefault(p => p.Raw.PublishTime > DateTime.Now)?.Raw
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