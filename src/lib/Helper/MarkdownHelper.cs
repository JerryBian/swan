using Markdig;

namespace Swan.Lib.Helper
{
    internal class MarkdownHelper
    {
        private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

        public static string ToHtml(string md)
        {
            return string.IsNullOrEmpty(md) ? string.Empty : Markdown.ToHtml(md, Pipeline);
        }
    }
}
