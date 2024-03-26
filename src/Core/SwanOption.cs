namespace Swan.Core;

public class SwanOption
{
    public SwanOption()
    {
        DataLocation = Path.GetTempPath();
        Title = "Swan";
        SkipGitOperation = true;
    }

    public string DataLocation { get; set; }

    public string Title { get; set; }

    public bool SkipGitOperation { get; set; }
}
