using System.Collections.Generic;
using Laobian.Share.Site.Read;

namespace Laobian.Blog.Models;

public class BookItemViewModel
{
    public string Title { get; set; }

    public int Count { get; set; }

    public string Id { get; set; }

    public List<BookItem> Items { get; } = new();
}