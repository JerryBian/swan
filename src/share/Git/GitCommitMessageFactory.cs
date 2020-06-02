using System;
using Laobian.Share.Extension;

namespace Laobian.Share.Git
{
    public class GitCommitMessageFactory
    {
        public static string ServerStarted()
        {
            return Format(":horse: Web server started.");
        }

        public static string ServerStopped()
        {
            return Format(":frog: Web server stopped.");
        }

        public static string ScheduleUpdated()
        {
            return Format(":panda_face: Update scheduled.");
        }

        public static string GitHubHook()
        {
            return Format(":monkey_face: GitHub Hook event.");
        }

        private static string Format(string message)
        {
            return $"[{DateTime.Now.ToDateAndTime()}] {message}";
        }
    }
}