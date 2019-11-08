using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Laobian.Share.Blog.Alert
{
    public interface IBlogAlertService
    {
        Task AlertEventAsync(string message, Exception error = null);

        Task AlertAssetReloadResultAsync(string subject, string warning, string error, List<string> addedPosts = null, List<string> modifiedPosts = null);
    }
}
