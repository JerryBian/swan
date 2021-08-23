using System.Collections.Generic;
using Laobian.Share.Read;

namespace Laobian.Read.Models
{
    public class GroupedReadItems
    {
        public string Title { get; set; }

        public int Count { get; set; }

        public string Id { get; set; }

        public List<BookItem> Items { get; } = new();
    }
}