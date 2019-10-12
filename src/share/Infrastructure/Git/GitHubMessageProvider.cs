using System;
using Laobian.Share.Extension;

namespace Laobian.Share.Infrastructure.Git
{
    public class GitHubMessageProvider
    {
        public static string GetPostCommitMessage(string message)
        {
            message = string.IsNullOrEmpty(message) ? "Automatically Update" : message;
            return $"{message}, @{DateTime.UtcNow.ToChinaTime().ToTimeZoneString(TimeSpan.FromHours(8))}";
        }
    }
}
