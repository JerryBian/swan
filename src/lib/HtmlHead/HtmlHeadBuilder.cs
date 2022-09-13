using Laobian.Lib.Helper;
using Laobian.Lib.Option;
using System.Text;

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
        StringBuilder sb = new();
        _ = sb.AppendLine("<meta charset=\"utf-8\">");
        _ = sb.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">");
        _ = sb.AppendLine($"<meta name=\"msapplication-TileColor\" content=\"{_buildOption.ApplicationTitleColor}\">");
        _ = sb.AppendLine($"<meta name=\"theme-color\" content=\"{_buildOption.ThemeColor}\">");
        _ = sb.AppendLine($"<meta name=\"copyright\"content=\"{_option.AdminUserFullName}\">");
        _ = sb.AppendLine("<meta name=\"language\" content=\"zh\">");
        _ = sb.AppendLine($"<meta name=\"author\" content=\"{_option.AdminUserFullName}, {_option.AdminEmail}\">");

        if (!string.IsNullOrEmpty(_buildOption.Description))
        {
            string desc = StringHelper.Truncate(_buildOption.Description, 149);
            _ = sb.AppendLine($"<meta name=\"description\" content=\"{desc}\"/>");
        }

        _ = sb.AppendLine("<link rel=\"apple-touch-icon\" sizes=\"180x180\" href=\"/apple-touch-icon.png\">");
        _ = sb.AppendLine("<link rel=\"icon\" type=\"image/png\" sizes=\"32x32\" href=\"/favicon-32x32.png\">");
        _ = sb.AppendLine("<link rel=\"icon\" type=\"image/png\" sizes=\"16x16\" href=\"/favicon-16x16.png\">");
        _ = sb.AppendLine("<link rel=\"manifest\" href=\"/site.webmanifest\">");
        _ = sb.AppendLine(
            $"<link rel=\"mask-icon\" href=\"/safari-pinned-tab.svg\" color=\"{_buildOption.SafariPinnedTabColor}\">");

        string title = _buildOption.BaseTitle;
        if (!string.IsNullOrEmpty(_buildOption.Title))
        {
            title = $"{_buildOption.Title} &middot; " + title;
        }

        _ = sb.AppendLine($"<title>{title}</title>");

        if (_buildOption.RobotsEnabled)
        {
            GoogleStructuredAuthor googleStructuredAuthor = new()
            {
                Name = _option.AdminUserFullName,
                Type = "Person",
                Url = _option.BaseUrl
            };
            GoogleStructuredData googleStructuredData = new()
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

            _ = sb.AppendLine("<meta name=\"robots\" content=\"index,follow,archive\"/>");
            _ = sb.AppendLine(
                $"<script type=\"application/ld+json\">{JsonHelper.Serialize(googleStructuredData)}</script>");
        }
        else
        {
            _ = sb.AppendLine("<meta name=\"robots\" content=\"noindex,nofollow,noarchive\"/>");
            _ = sb.AppendLine("<meta name=\"googlebot\" content=\"noindex,nofollow,noarchive\"/>");
        }

        return sb.ToString();
    }
}