namespace Swan.Core.Option
{
    public class GeneralOption
    {
        public GeneralOption()
        {
            SkipGitOperation = true;
            AssetLocation = Path.Combine(Path.GetTempPath(), "swan");
        }

        public string AssetLocation { get; set; }

        public bool SkipGitOperation { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
    }
}
