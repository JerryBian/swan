using System.Collections.Generic;
using Laobian.Share.Model.Read;

namespace Laobian.Blog.Models;

public class ReadItemViewModel
{
    public string Title { get; set; }

    public int Count { get; set; }

    public string Id { get; set; }

    public List<ReadItemRuntime> Items { get; } = new();
}