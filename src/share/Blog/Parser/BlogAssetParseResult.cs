using System;
using System.Collections.Generic;

namespace Laobian.Share.Blog.Parser
{
    public class BlogAssetParseResult
    {
        public BlogAssetParseResult()
        {
            Success = true;
            WarningMessages = new List<string>();
            ErrorMessages = new List<string>();
            NameValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public bool Success { get; set; }

        public List<string> WarningMessages { get; }

        public List<string> ErrorMessages { get; }

        public Dictionary<string, string> NameValues { get; }

        public string UnParsedContent { get; set; }
    }

    public class BlogAssetParseResult<T> : BlogAssetParseResult
    {
        public BlogAssetParseResult(BlogAssetParseResult result)
        {
            WarningMessages.AddRange(result.WarningMessages);
            ErrorMessages.AddRange(result.ErrorMessages);
            Success = result.Success;
            UnParsedContent = result.UnParsedContent;

            foreach (var nameValue in result.NameValues)
            {
                NameValues[nameValue.Key] = nameValue.Value;
            }
        }

        public T Instance { get; set; }
    }
}