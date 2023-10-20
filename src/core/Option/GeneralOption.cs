﻿namespace Swan.Core.Option
{
    public class GeneralOption
    {
        public GeneralOption()
        {
            SkipGitOperation = true;
            AdminUserName = "test";
            AdminPassword = "test";
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
