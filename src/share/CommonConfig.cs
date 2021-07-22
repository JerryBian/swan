using System.IO;

namespace Laobian.Share
{
    public abstract class CommonConfig
    {
        public string BlogLocalEndpoint { get; set; }

        public string BlogRemoteEndpoint { get; set; }

        public string ApiLocalEndpoint { get; set; }

        public string AdminLocalEndpoint { get; set; }

        public string AdminEmail { get; set; }

        public string AppVersion { get; set; }

        public string AssetLocation { get; set; }

        public string GetBlogPostLocation()
        {
            if (string.IsNullOrEmpty(AssetLocation))
            {
                throw new LaobianConfigException(nameof(AssetLocation));
            }

            return Path.Combine(AssetLocation, Constants.BlogPostAssetFolder);
        }

        public string GetBlogFileLocation()
        {
            if (string.IsNullOrEmpty(AssetLocation))
            {
                throw new LaobianConfigException(nameof(AssetLocation));
            }

            return Path.Combine(AssetLocation, Constants.BlogFileAssetFolder);
        }
    }
}