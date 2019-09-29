using System.Runtime.InteropServices;

namespace Laobian.Blog
{
    public class BlogState
    {
        public static string NetCoreVersion => RuntimeInformation.FrameworkDescription;
    }
}
