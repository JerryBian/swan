using Swan.Core.Model.Object;
using System.Text.Json.Serialization;

namespace Swan.Core.Model
{
    public class BlogPost
    {
        public BlogPost()
        {
        }

        public BlogPost(BlogPostObject obj)
        {
            Object = obj;
            BlogTags = new List<BlogTag>();
        }

        [JsonPropertyName("o")]
        public BlogPostObject Object { get; init; }

        [JsonPropertyName("t")]
        public List<BlogTag> BlogTags { get; init; }

        [JsonPropertyName("s")]
        public BlogSeries BlogSeries { get; set; }

        [JsonPropertyName("h")]
        public string HtmlContent { get; set; }

        public string GetUrl()
        {
            return $"/blog/post/{Object.Link}.html";
        }

        public bool IsPublished()
        {
            return Object.PublishTime <= DateTime.Now;
        }

        public List<BlogTag> GetTags(bool isAdmin)
        {
            if(isAdmin)
            {
                return BlogTags;
            }

            var tags = BlogTags.Where(x => x.Posts.Any(x => x.IsPublished())).ToList();
            return tags;
        }
    }
}
