using Markdig;

namespace Laobian.Share.Util;

public static class MarkdownUtil
{
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

    public static string ToHtml(string md)
    {
        return Markdown.ToHtml(md, Pipeline);
    }
}