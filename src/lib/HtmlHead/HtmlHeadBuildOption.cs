namespace Laobian.Lib.HtmlHead;

public class HtmlHeadBuildOption
{
    public bool RobotsEnabled { get; set; }

    public string ApplicationTitleColor { get; set; }

    public string ThemeColor { get; set; }

    public string Description { get; set; }

    public string SafariPinnedTabColor { get; set; }

    public string Title { get; set; }

    public string BaseTitle { get; set; }

    public string BaseImage { get; set; }

    public string Image { get; set; }

    public DateTime DatePublished { get; set; } = DateTime.Now;

    public DateTime DateModified { get; set; } = DateTime.Now;
}