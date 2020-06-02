using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Laobian.Share.Blog.Asset
{
    public class BlogAssetLoadResult<T>
    {
        public List<string> Warnings { get; set; } = new List<string>();

        public List<string> Errors { get; set; } = new List<string>();

        public string Description { get; set; }

        public T Instance { get; set; }

        public string AggregateMessages()
        {
            var sb = new StringBuilder();
            if (Warnings.Any() || Errors.Any())
            {
                sb.AppendLine(Description);
            }


            if (Warnings.Any())
            {
                sb.AppendLine("\tWarnings:");
                foreach (var warningMessage in Warnings)
                {
                    sb.AppendLine("\t\t" + warningMessage);
                }
            }

            if (Errors.Any())
            {
                sb.AppendLine("\tErrors:");
                foreach (var errorMessage in Warnings)
                {
                    sb.AppendLine("\t\t" + errorMessage);
                }
            }

            return sb.ToString();
        }
    }
}
