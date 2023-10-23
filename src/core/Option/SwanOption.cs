namespace Swan.Core.Option
{
    public class SwanOption
    {
        public SwanOption()
        {
            SkipGitOperation = true;
            AdminUserName = "test";
            AdminPassword = "test";
            Title = "Swan";
            Description = "A blog engine";
            AssetLocation = Path.Combine(Path.GetTempPath(), "swan");
        }

        public string AssetLocation { get; set; }

        public bool SkipGitOperation { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string AdminUserName { get; set; }

        public string AdminPassword { get; set; }
    }
}
