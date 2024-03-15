using Swan.Core.Model;

namespace Swan.Core.Service
{
    internal class SwanInternalObject
    {
        public List<SwanTag> Tags { get; init; } = [];

        public List<SwanPost> Posts { get; init; } = [];

        public List<SwanRead> Reads { get; init; } = [];
    }
}
