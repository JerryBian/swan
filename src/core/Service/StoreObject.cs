using GitStoreDotnet;
using HtmlAgilityPack;
using Swan.Core.Extension;
using Swan.Core.Helper;
using Swan.Core.Model;

namespace Swan.Core.Service
{
    internal class StoreObject
    {
        private readonly bool _adminOnly;
        private readonly IDictionary<Type, object> _typedObjects;

        public StoreObject(bool adminOnly)
        {
            _adminOnly = adminOnly;
            _typedObjects = new Dictionary<Type, object>
            {
                { typeof(SwanPost), Posts },
                { typeof(PostTag), Tags },
                { typeof(PostSeries), Series },
                { typeof(SwanRead), ReadItems },
                { typeof(SwanPage), Pages}
            };
        }

        public List<SwanPost> Posts { get; } = new();

        public List<PostTag> Tags { get; } = new();

        public List<PostSeries> Series { get; } = new();

        public List<SwanRead> ReadItems { get; } = new();

        public List<SwanPage> Pages { get; } = new();

        public List<T> Get<T>() where T : SwanObject
        {
            return _typedObjects.ContainsKey(typeof(T)) ? _typedObjects[typeof(T)] as List<T> : new List<T>();
        }

        public async Task PopulateDataAsync(IGitStore gitStore)
        {
            Reset();

            var posts = JsonHelper.Deserialize<List<SwanPost>>(await gitStore.GetTextAsync(new SwanPost().GetGitStorePath()));
            if (posts != null)
            {
                Posts.AddRange(posts.Where(x => _adminOnly || x.IsPublicToEveryOne()));
            }

            var series = JsonHelper.Deserialize<List<PostSeries>>(await gitStore.GetTextAsync(new PostSeries().GetGitStorePath()));
            if (series != null)
            {
                Series.AddRange(series.Where(x => _adminOnly || x.IsPublicToEveryOne()));
            }

            var tags = JsonHelper.Deserialize<List<PostTag>>(await gitStore.GetTextAsync(new PostTag().GetGitStorePath()));
            if (tags != null)
            {
                Tags.AddRange(tags.Where(x => _adminOnly || x.IsPublicToEveryOne()));
            }

            var readItems = JsonHelper.Deserialize<List<SwanRead>>(await gitStore.GetTextAsync(new SwanRead().GetGitStorePath()));
            if (readItems != null)
            {
                ReadItems.AddRange(readItems.Where(x => _adminOnly || x.IsPublicToEveryOne()));
            }

            var pages = JsonHelper.Deserialize<List<SwanPage>>(await gitStore.GetTextAsync(new SwanPage().GetGitStorePath()));
            if (pages != null)
            {
                Pages.AddRange(pages);
            }

            PostExtend();
        }

        private void Reset()
        {
            Posts.Clear();
            Tags.Clear();
            Series.Clear();
            ReadItems.Clear();
            Pages.Clear();
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
                        post.TextExcerpt = p.InnerText;
                    }
                }
                else
                {
                    post.TextExcerpt = post.Excerpt;
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
                        tagSnippets.Add($"<span><a href=\"{tag.GetFullLink()}\" class=\"btn btn-outline-light btn-sm\"><i class=\"bi bi-tag\"></i> {tag.Name}</a></span>");
                    }
                }

                var seriesSnippet = string.Empty;
                if (post.Series != null)
                {
                    var series = Series.FirstOrDefault(x => StringHelper.EqualsIgoreCase(x.Id, post.Series));
                    if (series != null)
                    {
                        post.BlogSeries = series;
                        series.BlogPosts.Add(post);
                        seriesSnippet = $"<a href=\"{series.GetFullLink()}\" class=\"btn btn-outline-light btn-sm\"><i class=\"bi bi-bookmark\"></i> {series.Name}</a>";
                    }
                }

                post.PreviousPost = Posts.OrderBy(x => x.PublishDate).LastOrDefault(x => x.PublishDate < post.PublishDate);
                post.NextPost = Posts.OrderBy(x => x.PublishDate).FirstOrDefault(x => x.PublishDate > post.PublishDate);

                var page = Pages.FirstOrDefault(x => StringHelper.EqualsIgoreCase(x.Path, post.GetFullLink()));
                if (page != null)
                {
                    post.PageStat = page;
                }

                post.HtmlMetadata1 = $@"<div class=""small text-muted text-truncate mb-1"">
                <a href=""/post/archive#year-{post.PublishDate.Year}"" class=""text-reset text-decoration-none"">
                    {post.PublishDate.ToCnDate()}
                </a>
                <span> &middot; </span>
                <span>
                    {post.PageStat.Hit} 次访问
                </span>
            </div>";
                post.HtmlMetadata2 = $@"<div class=""small text-muted container-fluid"">
                <div class=""row"">
                    <div class=""col-md-7 text-truncate mb-1 px-0"">
                        {string.Join(" ", tagSnippets)}
                    </div>
                    <div class=""col-md-5  text-truncate mb-1 px-0"">
                        {seriesSnippet}
                    </div>
                </div>
            </div>";
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

                if (post.BlogSeries != null)
                {
                    post.RecommendPostsBySeries.AddRange(post.BlogSeries.BlogPosts.Take(8));
                }
            }

            foreach(var read in ReadItems)
            {
                var metadatas = new List<string>();
                if(!string.IsNullOrEmpty(read.Author))
                {
                    if(!string.IsNullOrEmpty(read.AuthorCountry))
                    {
                        metadatas.Add($"({read.AuthorCountry}){read.Author}");
                    }
                    else
                    {
                        metadatas.Add(read.Author);
                    }
                }

                if(!string.IsNullOrEmpty(read.Translator))
                {
                    metadatas.Add($"{read.Translator}(译)");
                }

                read.HtmlMetadata = string.Join("/", metadatas);
                read.HtmlComment = MarkdownHelper.ToHtml(read.Comment);

                foreach(var p in read.Posts)
                {
                    var post = Posts.FirstOrDefault(x => StringHelper.EqualsIgoreCase(x.Id, p));
                    if(post != null)
                    {
                        read.BlogPosts.Add(post);
                    }
                }
            }
        }
    }
}
