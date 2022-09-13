using System.Text;
using Laobian.Lib.Helper;
using Laobian.Lib.Option;

namespace Laobian.Lib.HtmlHead;

public class HtmlHeadBuilder
{
    private readonly HtmlHeadBuildOption _buildOption;
    private readonly LaobianOption _option;

    public HtmlHeadBuilder(LaobianOption option, HtmlHeadBuildOption buildOption)
    {
        _option = option;
        _buildOption = buildOption;
    }

    public string Build()
    {
        var sb = new StringBuilder();
        sb.AppendLine("<meta charset=\"utf-8\">");
        sb.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
        sb.AppendLine($"<meta name=\"msapplication-TileColor\" content=\"{_buildOption.ApplicationTitleColor}\">");
        sb.AppendLine($"<meta name=\"theme-color\" content=\"{_buildOption.ThemeColor}\">");
        sb.AppendLine($"<meta name=\"copyright\"content=\"{_option.AdminUserFullName}\">");
        sb.AppendLine("<meta name=\"language\" content=\"zh\">");
        sb.AppendLine($"<meta name=\"author\" content=\"{_option.AdminUserFullName}, {_option.AdminEmail}\">");

        if (!string.IsNullOrEmpty(_buildOption.Description))
        {
            var desc = StringHelper.Truncate(_buildOption.Description, 149);
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
            title = $"{_buildOption.Title} &middot; " + title;
        }

        sb.AppendLine($"<title>{title}</title>");

        if (_buildOption.RobotsEnabled)
        {
            var googleStructuredAuthor = new GoogleStructuredAuthor
            {
                Name = _option.AdminUserFullName,
                Type = "Person",
                Url = _option.BaseUrl
            };
            var googleStructuredData = new GoogleStructuredData
            {
                Context = "https://schema.org",
                Type = "NewsArticle",
                Headline = title,
                DatePublished = _buildOption.DatePublished,
                DateModified = _buildOption.DateModified
            };
            googleStructuredData.Authors.Add(googleStructuredAuthor);
            googleStructuredData.Images.Add(!string.IsNullOrEmpty(_buildOption.Image)
                ? _buildOption.Image
                : _buildOption.BaseImage);

            sb.AppendLine("<meta name=\"robots\" content=\"index,follow,archive\"/>");
            sb.AppendLine(
                $"<script type=\"application/ld+json\">{JsonHelper.Serialize(googleStructuredData)}</script>");
        }
        else
        {
            sb.AppendLine("<meta name=\"robots\" content=\"noindex,nofollow,noarchive\"/>");
            sb.AppendLine("<meta name=\"googlebot\" content=\"noindex,nofollow,noarchive\"/>");
        }

        return sb.ToString();
    }
}