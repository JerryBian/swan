using Laobian.Share.Helper;

namespace Laobian.Share.BlogEngine.Model
{
    public class BlogTag : BlogAsset
    {
        public string Name { get; set; }

        public string Link { get; set; }

        public string GetLink()
        {
            return AddressHelper.GetAddress(false, "tag", $"#{Link}");
        }
    }
}
