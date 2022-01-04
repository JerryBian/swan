using System.Collections.Generic;
using Laobian.Share.Model.Blog;

namespace Laobian.Admin.Models;

public class BlogPostUpdateViewModel
{
    public BlogPost Post { get; set; }

    public List<BlogTag> Tags { get; } = new();
}