using Microsoft.Extensions.Primitives;

namespace Swan.Core.Service
{
    public class SwanChangeToken : IChangeToken
    {
        public bool ActiveChangeCallbacks => false;

        public bool HasChanged { get; set; }

        public IDisposable RegisterChangeCallback(Action<object> callback, object state)
        {
            throw new NotImplementedException();
        }
    }
}
