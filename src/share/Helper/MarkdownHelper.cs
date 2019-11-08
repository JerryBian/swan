using Markdig;

namespace Laobian.Share.Helper
{
    public class MarkdownHelper
    {
        public static string ToHtml(string markdown)
        {
            return Markdown.ToHtml(markdown);
        }
    }
}
