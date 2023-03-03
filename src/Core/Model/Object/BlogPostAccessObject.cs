using System.Text.Json.Serialization;

namespace Swan.Core.Model.Object
{
    public class BlogPostAccessObject : FileObjectBase
    {
        [JsonPropertyName("p")]
        public string PostId { get; set; }

        [JsonPropertyName("t")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("ip")]
        public string IpAddress { get; set; }

        public override string GetFileName()
        {
            return Constants.Asset.BlogPostAccessFile;
        }
    }
}
