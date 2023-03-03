namespace Swan.Core.Model.Object
{
    public class BlogPostAccessObject : FileObjectBase
    {
        public string PostId { get; set; }

        public DateTime Timestamp { get; set; }

        public string IpAddress { get; set; }

        public override string GetFileName()
        {
            return Constants.Asset.BlogPostAccessFile;
        }
    }
}
