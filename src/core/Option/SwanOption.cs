namespace Swan.Core.Option
{
    public class SwanOption
    {
        public SwanOption()
        {
            SkipGitOperation = true;
            AdminUserName = "test";
            AdminPassword = "test";
            DataLocation = Path.Combine(Path.GetTempPath(), "swan");
        }

        public string AdminUserName { get; set; }

        public string AdminPassword { get; set; }

        public string DataLocation { get; set; }

        public bool SkipGitOperation { get; set; }
    }
}
