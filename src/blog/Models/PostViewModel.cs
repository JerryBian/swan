using System.Collections.Generic;
using System.Linq;
using Laobian.Share.Extension;
using Laobian.Share.Site.Blog;

namespace Laobian.Blog.Models
{
    public class PostViewModel
    {
        public BlogPostRuntime Current { get; set; }

        public BlogPostRuntime Previous { get; set; }

        public BlogPostRuntime Next { get; set; }

        public string MetadataHtml { get; private set; }

        public string TagHtml { get; set; }

        public void SetAdditionalInfo()
        {
            var metadata = new List<string>
            {
                $"<time class=\"muted-bolder\" datetime=\"{Current.Raw.PublishTime.ToDateAndTime()}\" title=\"{Current.Raw.PublishTime.ToChinaDateAndTime()}\">{Current.Raw.PublishTime.ToChinaDate()}</time>",
                $"<span class=\"muted-bolder\" title=\"{Current.GetAccessCount()}\">{Current.GetAccessCount().ToHuman()}</span> <span>次阅读</span>"
            };

            MetadataHtml = string.Join(" &middot; ", metadata);

            var tags = new List<string>();

            foreach (var tag in Current.Tags)
            {
                tags.Add($"<a href='/tag#{tag.Link}' title='{tag.DisplayName}'>{tag.DisplayName}</a>");
            }

            if (tags.Any())
            {
                TagHtml = $"标签：<span>{string.Join(", ", tags)}</span>";
            }
        }
    }
}