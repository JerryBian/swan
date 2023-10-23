using GitStoreDotnet;
using HtmlAgilityPack;
using LibGit2Sharp;
using Swan.Core.Extension;
using Swan.Core.Helper;
using Swan.Core.Model;

namespace Swan.Core.Service
{
    internal class StoreObject
    {
        private readonly IDictionary<Type, object> _typedObjects;

        public StoreObject()
        {
            _typedObjects = new Dictionary<Type, object>
            {
                { typeof(SwanPost), Posts },
                { typeof(SwanTag), Tags },
                { typeof(SwanSeries), Series },
                { typeof(SwanRead), ReadItems }
            };
        }

        public List<SwanPost> Posts { get; } = new();

        public List<SwanTag> Tags { get; } = new();

        public List<SwanSeries> Series { get; } = new();

        public List<SwanRead> ReadItems { get; } = new();

        public List<T> Get<T>() where T : SwanObject
        {
            return _typedObjects.ContainsKey(typeof(T)) ? _typedObjects[typeof(T)] as List<T> : new List<T>();
        }

        public async Task PopulateDataAsync(IGitStore gitStore)
        {
            Reset();

            Posts.AddRange(JsonHelper.Deserialize<List<SwanPost>>(await gitStore.GetTextAsync(new SwanPost().GetGitStorePath())));
            Series.AddRange(JsonHelper.Deserialize<List<SwanSeries>>(await gitStore.GetTextAsync(new SwanSeries().GetGitStorePath())));
            Tags.AddRange(JsonHelper.Deserialize<List<SwanTag>>(await gitStore.GetTextAsync(new SwanTag().GetGitStorePath())));
            ReadItems.AddRange(JsonHelper.Deserialize<List<SwanRead>>(await gitStore.GetTextAsync(new SwanRead().GetGitStorePath())));

            PostExtend();
        }

        private void Reset()
        {
            Posts.Clear();
            Tags.Clear();
            Series.Clear();
            ReadItems.Clear();
        }

        private void PostExtend()
        {
            foreach (var post in Posts)
            {
                var postHtml = MarkdownHelper.ToHtml(post.Content);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(postHtml);
                var imageNodes = htmlDoc.DocumentNode.Descendants("img").ToList();
                foreach (var imageNode in imageNodes)
                {
                    if (imageNode.Attributes.Contains("src"))
                    {
                        imageNode.AddClass("img-thumbnail mx-auto d-block");
                        imageNode.Attributes.Add("loading", "lazy");
                    }
                }

                if (string.IsNullOrEmpty(post.Excerpt))
                {
                    var p = htmlDoc.DocumentNode.Descendants("p").FirstOrDefault(x => !string.IsNullOrEmpty(x.InnerText));
                    if (p != null)
                    {
                        post.HtmlExcerpt = MarkdownHelper.ToHtml(p.InnerText);
                    }
                }
                else
                {
                    post.HtmlExcerpt = MarkdownHelper.ToHtml(post.Excerpt);
                }

                post.HtmlContent = htmlDoc.DocumentNode.OuterHtml;

                var tagSnippets = new List<string>();
                foreach (var item in post.Tags)
                {
                    var tag = Tags.FirstOrDefault(x => StringHelper.EqualsIgoreCase(x.Id, item));
                    if (tag != null)
                    {
                        post.BlogTags.Add(tag);
                        tag.BlogPosts.Add(post);
                        tagSnippets.Add($"<span><a href=\"{tag.GetFullLink()}\">{tag.Name}</a></span>");
                    }
                }

                var seriesSnippets = new List<string>();
                if (post.Series != null)
                {
                    var series = Series.FirstOrDefault(x => StringHelper.EqualsIgoreCase(x.Id, post.Series));
                    if (series != null)
                    {
                        post.BlogSeries = series;
                        series.BlogPosts.Add(post);
                        seriesSnippets.Add($"<span><i class=\"bi bi-bookmark\"></i> <a href=\"{series.GetFullLink()}\">{series.Name}</a></span>");
                    }
                }

                post.PreviousPost = Posts.OrderBy(x => x.PublishDate).LastOrDefault(x => x.PublishDate < post.PublishDate);
                post.NextPost = Posts.OrderBy(x => x.PublishDate).FirstOrDefault(x => x.PublishDate > post.PublishDate);
                post.HtmlMetadata = $"<span><i class=\"bi bi-calendar2-week\"></i> <a href=\"/post/{post.PublishDate.Year}\" class=\"text-reset text-decoration-none\">{post.PublishDate.ToCnDate()}</a></span> " +
                    $"{string.Join(" ", seriesSnippets)} " +
                    $"{string.Join(", ", tagSnippets)}";
            }

            // random posts recommend
            // TODO: use .NET 8 Random functions
            foreach (var post in Posts)
            {
                if (post.Tags.Any())
                {
                    var similarPosts = new List<SwanPost>();
                    foreach (var item in post.BlogTags)
                    {
                        similarPosts.AddRange(item.BlogPosts);
                    }

                    post.RecommendPostsByTag.AddRange(similarPosts.Distinct().Take(8));
                }

                if (post.Series != null)
                {
                    post.RecommendPostsBySeries.AddRange(post.BlogSeries.BlogPosts.Take(8));
                }
            }
        }
    }
}
