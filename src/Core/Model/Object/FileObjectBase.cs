using System.Text.Json.Serialization;

namespace Swan.Core.Model.Object
{
    public abstract class FileObjectBase
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("createTime")]
        public DateTime CreateTime { get; set; }

        [JsonPropertyName("lastUpdateTime")]
        public DateTime LastUpdateTime { get; set; }

        public abstract string GetFileName();
    }
}
