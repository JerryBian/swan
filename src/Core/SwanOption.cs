namespace Swan.Core;

public class SwanOption
{
    public SwanOption()
    {
        DataLoaction = Path.GetTempPath();
        Title = "Swan";
        SkipGitOperation = true;
    }

    public string DataLoaction { get; set; }

    public string Title { get; set; }

    public bool SkipGitOperation { get; set; }
}
