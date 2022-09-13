using Laobian.Lib.Model;

namespace Laobian.Web.Areas.Admin.Models
{
    public class ReadItemViewModel
    {
        public List<BlogPostView> Posts { get; set; }

        public ReadItem Item { get; set; }
    }
}
