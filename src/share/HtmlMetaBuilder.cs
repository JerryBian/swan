using System.Text;

namespace Laobian.Share
{
    public class HtmlMetaBuilder
    {
        public bool RobotsAllowed { get; set; }

        public string Description { get; set; }

        public string Keywords { get; set; }

        public string Build()
        {
            var sb = new StringBuilder();

            return sb.ToString();
        }
    }
}