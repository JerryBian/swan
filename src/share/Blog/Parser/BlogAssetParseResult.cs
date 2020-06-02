using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public string AggregateMessages()
        {
            var sb = new StringBuilder();

            if (WarningMessages.Any())
            {
                sb.AppendLine("Warnings:");
                foreach (var warningMessage in WarningMessages)
                {
                    sb.AppendLine("\t" + warningMessage);
                }
            }

            if (ErrorMessages.Any())
            {
                sb.AppendLine("Errors:");
                foreach (var errorMessage in ErrorMessages)
                {
                    sb.AppendLine("\t" + errorMessage);
                }
            }

            return sb.ToString();
        }
    }
}