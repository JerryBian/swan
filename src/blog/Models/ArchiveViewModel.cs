using System.Collections.Generic;
using Laobian.Share.BlogEngine.Model;

namespace Laobian.Blog.Models
{
    public class ArchiveViewModel
    {
        public ArchiveViewModel(string name, string link)
        {
            Name = name;
            Link = link;
            Posts = new List<BlogPost>();
        }

        public string Name { get; }

        public string Link { get; }

        public List<BlogPost> Posts { get; }
    }
}
