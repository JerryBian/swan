using System.IO;

namespace Laobian.Share.Option
{
    public class CommonOption
    {
        public string BlogLocalEndpoint { get; set; }

        public string BlogRemoteEndpoint { get; set; }

        public string ApiLocalEndpoint { get; set; }

        public string FileRemoteEndpoint { get; set; }

        public string AdminUserName { get; set; }

        public string AdminEmail { get; set; }

        public string AdminChineseName { get; set; }

        public string AdminEnglishName { get; set; }

        public string AssetLocation { get; set; }

        public string SendGridApiKey { get; set; }

        public string DataProtectionKeyPath { get; set; }

        public string HttpRequestToken { get; set; }

        public string HomePageEndpoint { get; set; }

        public string GetBlogBaseLocation()
        {
            if (string.IsNullOrEmpty(AssetLocation))
            {
                throw new LaobianOptionException(nameof(AssetLocation));
            }

            return Path.Combine(AssetLocation, Constants.BlogPostAssetFolder);
        }

        public string GetBlogPostLocation()
        {
            return Path.Combine(GetBlogBaseLocation(), Constants.BlogPostPostBaseFolderName);
        }

        public string GetBlogFileLocation()
        {
            if (string.IsNullOrEmpty(AssetLocation))
            {
                throw new LaobianOptionException(nameof(AssetLocation));
            }

            return Path.Combine(AssetLocation, Constants.BlogPostAssetFolder, Constants.BlogPostFileBaseFolderName);
        }

        public string GetBlogMetadataLocation()
        {
            if (string.IsNullOrEmpty(AssetLocation))
            {
                throw new LaobianOptionException(nameof(AssetLocation));
            }

            return Path.Combine(AssetLocation, Constants.DbAssetFolder, "blog");
        }

        public string GetDbLocation()
        {
            if (string.IsNullOrEmpty(AssetLocation))
            {
                throw new LaobianOptionException(nameof(AssetLocation));
            }

            return Path.Combine(AssetLocation, Constants.DbAssetFolder);
        }

        public string GetBlogAccessLocation()
        {
            if (string.IsNullOrEmpty(AssetLocation))
            {
                throw new LaobianOptionException(nameof(AssetLocation));
            }

            return Path.Combine(AssetLocation, Constants.DbAssetFolder, "blog", Constants.BlogAccessBaseFolderName);
        }

        public void Clone(CommonOption option)
        {
            foreach (var propertyInfo in GetType().GetProperties())
            {
                propertyInfo.SetValue(this, propertyInfo.GetValue(option));
            }
        }
    }
}