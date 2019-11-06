namespace Laobian.Share.Blog.Asset
{
    public class BlogAssetReloadResult<T>
    {
        public BlogAssetReloadResult()
        {
            Success = true;
        }

        public T Result { get; set; }

        public bool Success { get; set; }

        public string Error { get; set; }

        public string Warning { get; set; }
    }
}
