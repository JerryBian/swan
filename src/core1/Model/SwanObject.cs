using Swan.Core.Helper;
using System.Text.Json.Serialization;

namespace Swan.Core.Model
{
    public abstract class SwanObject
    {
        [JsonPropertyName("id")]
        [JsonPropertyOrder(-100)]
        public string Id { get; set; } = StringHelper.Random();

        [JsonPropertyOrder(100)]
        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [JsonPropertyOrder(101)]
        [JsonPropertyName("lastUpdatedAt")]
        public DateTime LastUpdatedAt { get; set; } = DateTime.Now;

        [JsonPropertyOrder(102)]
        [JsonPropertyName("isDeleted")]
        public bool IsDeleted { get; set; } = false;

        [JsonPropertyOrder(103)]
        [JsonPropertyName("isPublic")]
        public bool IsPublic { get; set; } = false;

        public virtual string GetObjectIdentity()
        {
            return null;
        }
    }
}
