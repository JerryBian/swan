using Swan.Core.Helper;
using Swan.Core.Model;
using System.Text;
using System.Text.Json.Serialization;

namespace Swan.Web
{
    public static class DataHelper
    {
        public static async Task RunAsync()
        {
            const string srcDir = @"C:\Source\swan-data";
            const string destDir = @"C:\Users\jerry\AppData\Local\Temp\swan\data";

            var readItems = new List<SwanPost>();
            foreach (var item in Directory.EnumerateFiles(Path.Combine(srcDir, "blog", "post")))
            {
                var json = await File.ReadAllTextAsync(item);
                var obj = JsonHelper.Deserialize<BlogPostObject>(json);

                readItems.Add(new SwanPost
                {
                    CreatedAt = obj.CreateTime,
                    IsPublic = true,
                    LastUpdatedAt = obj.LastUpdateTime,
                    Content = obj.MdContent,
                    Link = obj.Link,
                    PublishDate = obj.PublishTime,
                    Title = obj.Title
                });
            }

            var j = JsonHelper.Serialize(readItems);
            await File.WriteAllTextAsync(Path.Combine(destDir, "obj", "post.json"), j, new UTF8Encoding(false));
        }
        public abstract class FileObjectBase
        {
            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("createTime")]
            public DateTime CreateTime { get; set; }

            [JsonPropertyName("lastUpdateTime")]
            public DateTime LastUpdateTime { get; set; }

        }
        public class BlogPostObject : FileObjectBase
        {
            [JsonPropertyName("link")]
            public string Link { get; set; }

            [JsonPropertyName("title")]
            public string Title { get; set; }

            [JsonPropertyName("isPublic")]
            public bool IsPublic { get; set; }

            [JsonPropertyName("isTopping")]
            public bool IsTopping { get; set; }

            [JsonPropertyName("containsMath")]
            public bool ContainsMath { get; set; }

            [JsonPropertyName("publishTime")]
            public DateTime PublishTime { get; set; }

            [JsonPropertyName("mdContent")]
            public string MdContent { get; set; }

            [JsonPropertyName("accessCount")]
            public int AccessCount { get; set; }

            [JsonPropertyName("tags")]
            public List<string> Tags { get; init; } = new();

            [JsonPropertyName("series")]
            public string Series { get; set; }
        }
    }
}
