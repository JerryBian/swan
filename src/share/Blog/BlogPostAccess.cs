using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laobian.Share.Blog
{
    public class BlogPostAccess
    {
        public DateTime Date { get; set; }

        public string DateString => Date.ToString("yyyy-MM");

        public int Count { get; set; }
    }
}
