using Laobian.Lib.Model;

namespace Laobian.Web.Areas.Read.Models
{
    public class ReadIndexViewModel
    {
        public string Title { get; set; }

        public string Id { get; set; }

        public int Count { get; set; }

        public List<ReadItemView> Items { get; set; }
    }
}
