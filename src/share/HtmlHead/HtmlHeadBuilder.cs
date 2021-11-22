using System.Text;
using Laobian.Share.Option;
using Laobian.Share.Util;

namespace Laobian.Share.HtmlHead;

public class HtmlHeadBuilder
{
    private readonly HtmlHeadBuildOption _buildOption;
    private readonly SharedOptions _option;

    public HtmlHeadBuilder(SharedOptions option, HtmlHeadBuildOption buildOption)
    {
        _option = option;
        _buildOption = buildOption;
    }

    public string Build()
    {
        var sb = new StringBuilder();
        sb.AppendLine("<meta charset=\"utf-8\">");
        sb.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
        sb.AppendLine(_buildOption.RobotsEnabled
            ? "<meta name=\"robots\" content=\"index,follow\"/>"
            : "<meta name=\"robots\" content=\"noindex,nofollow,noarchive\"/><meta name=\"googlebot\" content=\"noindex,nofollow,noarchive\"/>");
        sb.AppendLine($"<meta name=\"msapplication-TileColor\" content=\"{_buildOption.ApplicationTitleColor}\">");
        sb.AppendLine($"<meta name=\"theme-color\" content=\"{_buildOption.ThemeColor}\">");
        sb.AppendLine($"<meta name=\"copyright\"content=\"{_option.AdminChineseName}\">");
        sb.AppendLine("<meta name=\"language\" content=\"zh\">");
        sb.AppendLine($"<meta name=\"author\" content=\"{_option.AdminChineseName}, {_option.AdminEmail}\">");

        if (!string.IsNullOrEmpty(_buildOption.Description))
        {
            var desc = _buildOption.Description.Length < 150
                ? _buildOption.Description
                : _buildOption.Description.Substring(0, 150);
            sb.AppendLine($"<meta name=\"description\" content=\"{desc}\"/>");
        }

        sb.AppendLine("<link rel=\"apple-touch-icon\" sizes=\"180x180\" href=\"/apple-touch-icon.png\">");
        sb.AppendLine("<link rel=\"icon\" type=\"image/png\" sizes=\"32x32\" href=\"/favicon-32x32.png\">");
        sb.AppendLine("<link rel=\"icon\" type=\"image/png\" sizes=\"16x16\" href=\"/favicon-16x16.png\">");
        sb.AppendLine("<link rel=\"manifest\" href=\"/site.webmanifest\">");
        sb.AppendLine(
            $"<link rel=\"mask-icon\" href=\"/safari-pinned-tab.svg\" color=\"{_buildOption.SafariPinnedTabColor}\">");

        var title = _buildOption.BaseTitle;
        if (!string.IsNullOrEmpty(_buildOption.Title))
        {
            title = $"{_buildOption.Title} - " + title;
        }

        sb.AppendLine($"<title>{title}</title>");

        if (_buildOption.RobotsEnabled)
        {
            var googleStructuredAuthor = new GoogleStructuredAuthor
            {
                Name = _option.AdminChineseName,
                Type = "Person",
                Url = _option.HomePageEndpoint
            };
            var googleStructuredData = new GoogleStructuredData();
            googleStructuredData.Context = "https://schema.org";
            googleStructuredData.Type = "NewsArticle";
            googleStructuredData.Headline = title;
            googleStructuredData.DatePublished = _buildOption.DatePublished;
            googleStructuredData.DateModified = _buildOption.DateModified;
            googleStructuredData.Authors.Add(googleStructuredAuthor);
            googleStructuredData.Images.Add(!string.IsNullOrEmpty(_buildOption.Image)
                ? _buildOption.Image
                : _buildOption.BaseImage);

            sb.AppendLine(
                $"<script type=\"application/ld+json\">{JsonUtil.Serialize(googleStructuredData)}</script>");
        }

        return sb.ToString();
    }
}