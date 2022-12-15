using Swan.Lib.Model;

namespace Swan.Areas.Admin.Models
{
    public class ReadItemViewModel
    {
        public List<BlogPostView> Posts { get; set; }

        public ReadItem Item { get; set; }
    }
}
