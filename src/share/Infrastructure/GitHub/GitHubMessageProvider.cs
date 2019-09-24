using System;
using Laobian.Share.Extension;

namespace Laobian.Share.Infrastructure.GitHub
{
    public class GitHubMessageProvider
    {
        private const string ServerPrefix = "::SERVER:: ";
        private const string AutomaticallyUpdate = "Automatically Update";

        public static string GetPostCommitMessage(string message = null)
        {
            message = string.IsNullOrEmpty(message) ? "Automatically Update" : message;
            return $"{ServerPrefix}{message}, @{DateTime.UtcNow.ToChinaTime()}";
        }

        public static bool IsServerCommit(string message)
        {
            return message.StartsWith(ServerPrefix, StringComparison.OrdinalIgnoreCase);
        }
    }
}
