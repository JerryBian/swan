using Markdig;

namespace Swan.Core.Helper;

public static class MarkdownHelper
{
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

    public static string ToHtml(string md)
    {
        return string.IsNullOrEmpty(md) ? string.Empty : Markdown.ToHtml(md, Pipeline);
    }
}
