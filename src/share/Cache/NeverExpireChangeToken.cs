using System;
using Microsoft.Extensions.Primitives;

namespace Laobian.Share.Cache
{
    public class NeverExpireChangeToken : IChangeToken
    {
        private static readonly Lazy<NeverExpireChangeToken> LazyInstance = new Lazy<NeverExpireChangeToken>(() => new NeverExpireChangeToken(), true);

        private NeverExpireChangeToken() { }

        public static NeverExpireChangeToken Instance => LazyInstance.Value;

        public IDisposable RegisterChangeCallback(Action<object> callback, object state)
        {
            throw new NotImplementedException();
        }

        public bool HasChanged => false;

        public bool ActiveChangeCallbacks => false;
    }
}
