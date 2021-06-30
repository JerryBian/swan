using Laobian.Share;

namespace Laobian.Api
{
    public class ApiConfig : CommonConfig
    {
        public SourceMode Source { get; set; }

        public string DbLocation { get; set; }

        public string BlogPostLocation { get; set; }
    }
}
