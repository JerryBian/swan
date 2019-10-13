using System;
using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Laobian.Blog.TagHelpers
{
    public class PaginationTagHelper : TagHelper
    {
        private const string QueryName = "p";
        private const string PreviousLabel = "&larr;";
        private const string NextLabel = "&rarr;";

        public int TotalPages { get; set; }

        public int CurrentPage { get; set; }

        public string Url { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "nav";
            output.Attributes.SetAttribute("id", "pagination");

            // No need to generate HTML markup as we only have one pagination
            if (TotalPages < 2)
            {
                return;
            }

            if (CurrentPage < 1 || CurrentPage > TotalPages)
            {
                throw new ArgumentOutOfRangeException(nameof(CurrentPage));
            }

            var html = new StringBuilder();
            html.AppendLine("<ul class='pagination justify-content-center'>");

            // Prev item only be visible if CurrentPage is greater than one
            if (CurrentPage > 1)
            {
                html.AppendLine(GetLinkItem($"<span>{PreviousLabel}</span>", GetUrl(CurrentPage - 1)));
            }

            for (var i = 1; i <= TotalPages; i++)
            {
                // display CurrentPage item as active
                if (i == CurrentPage)
                {
                    html.AppendLine(GetActiveItem(i));
                    continue;
                }

                html.AppendLine(GetLinkItem(i, GetUrl(i)));
            }

            // Next item only be visible if CurrentPage is less than TotalPages
            if (CurrentPage < TotalPages)
            {
                html.AppendLine(GetLinkItem($"<span>{NextLabel}</span>", GetUrl(CurrentPage + 1)));
            }

            html.AppendLine("</ul>");
            output.Content.SetHtmlContent(html.ToString());
        }

        private string GetUrl(int item)
        {
            return item == 1 ? Url : $"{Url}?{QueryName}={item}";
        }

        private string GetActiveItem(object item)
        {
            return $"<li class='page-item active'><span>{item}</span></li>";
        }

        private string GetLinkItem(object item, string url)
        {
            return $"<li class='page-item'><a class='page-link' href='{url}'>{item}</a></li>";
        }
    }
}