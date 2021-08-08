using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
