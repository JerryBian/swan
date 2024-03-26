using Swan.Core.Helper;
using System.Text.Json.Serialization;

namespace Swan.Core.Model;

public class SwanRead
{
    #region Raw

    [JsonPropertyName("bookName")]
    public string BookName { get; set; }

    [JsonPropertyName("author")]
    public string Author { get; set; }

    [JsonPropertyName("authorCountry")]
    public string AuthorCountry { get; set; }

    [JsonPropertyName("translator")]
    public string Translator { get; set; }

    [JsonPropertyName("grade")]
    public short Grade { get; set; }

    [JsonPropertyName("comment")]
    public string Comment { get; set; }

    [JsonPropertyName("posts")]
    public List<string> Posts { get; set; } = [];

    [JsonPropertyName("id")]
    [JsonPropertyOrder(-100)]
    public string Id { get; set; }

    [JsonPropertyOrder(100)]
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [JsonPropertyOrder(101)]
    [JsonPropertyName("lastUpdatedAt")]
    public DateTime LastUpdatedAt { get; set; } = DateTime.Now;

    [JsonPropertyOrder(103)]
    [JsonPropertyName("isPublic")]
    public bool IsPublic { get; set; } = false;

    #endregion

    #region Extension

    [JsonIgnore]
    public List<SwanPost> BlogPosts { get; } = [];

    [JsonIgnore]
    public string HtmlMetadata { get; set; }

    [JsonIgnore]
    public string HtmlComment { get; set; }

    #endregion
}
