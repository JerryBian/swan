using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
