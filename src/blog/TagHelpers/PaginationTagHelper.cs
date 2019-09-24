using System;
using System.Text;
using Laobian.Blog.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Laobian.Blog.TagHelpers
{
    public class PaginationTagHelper : TagHelper
    {
        private const string QueryName = "p";
        private const string PreviousLabel = "&larr;";
        private const string NextLabel = "&rarr;";

        public Pagination Pagination { get; set; }

        public string Url { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "nav";
            output.Attributes.SetAttribute("id", "pagination");

            // No need to generate HTML markup as we only have one pagination
            if (Pagination.TotalPages < 2)
            {
                return;
            }

            if (Pagination.CurrentPage < 1 || Pagination.CurrentPage > Pagination.TotalPages)
            {
                throw new ArgumentOutOfRangeException(nameof(Pagination.CurrentPage));
            }

            var html = new StringBuilder();
            html.AppendLine("<ul class='pagination justify-content-center'>");

            // Prev item only be visible if Pagination.CurrentPage is greater than one
            if (Pagination.CurrentPage > 1)
            {
                html.AppendLine(GetLinkItem($"<span>{PreviousLabel}</span>", GetUrl(Pagination.CurrentPage - 1)));
            }

            for (var i = 1; i <= Pagination.TotalPages; i++)
            {
                // display Pagination.CurrentPage item as active
                if (i == Pagination.CurrentPage)
                {
                    html.AppendLine(GetActiveItem(i));
                    continue;
                }

                html.AppendLine(GetLinkItem(i, GetUrl(i)));
            }

            // Next item only be visible if Pagination.CurrentPage is less than Pagination.TotalPages
            if (Pagination.CurrentPage < Pagination.TotalPages)
            {
                html.AppendLine(GetLinkItem($"<span>{NextLabel}</span>", GetUrl(Pagination.CurrentPage + 1)));
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