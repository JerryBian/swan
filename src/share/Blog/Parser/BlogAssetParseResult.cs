using System.Collections.Generic;

namespace Laobian.Share.Blog.Parser
{
    public class BlogAssetParseResult<T>
    {
        public BlogAssetParseResult()
        {
            Success = true;
            WarningMessages = new List<string>();
            ErrorMessages = new List<string>();
        }

        public bool Success { get; set; }

        public List<string> WarningMessages { get; }

        public List<string> ErrorMessages { get; }

        public T Instance { get; set; }
    }
}