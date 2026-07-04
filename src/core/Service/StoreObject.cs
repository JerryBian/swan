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

        public List<SwanPost> Posts { get; } = [];

        public List<PostTag> Tags { get; } = [];

        public List<PostSeries> Series { get; } = [];

        public List<SwanRead> ReadItems { get; } = [];

        public List<SwanPage> Pages { get; } = [];

        public List<T> Get<T>() where T : SwanObject
        {
            return _typedObjects.ContainsKey(typeof(T)) ? _typedObjects[typeof(T)] as List<T> : [];
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
            // Pre-sort posts by date once for O(n) prev/next linking
            var postsByDate = Posts.OrderBy(x => x.PublishDate).ToList();
            for (int i = 0; i < postsByDate.Count; i++)
            {
                postsByDate[i].PreviousPost = i > 0 ? postsByDate[i - 1] : null;
                postsByDate[i].NextPost = i < postsByDate.Count - 1 ? postsByDate[i + 1] : null;
            }

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
                        tagSnippets.Add($"<a href=\"{tag.GetFullLink()}\" class=\"post-tag-pill\">{tag.Name}</a>");
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
                        seriesSnippet = $"<a href=\"{series.GetFullLink()}\" class=\"post-tag-pill\">{series.Name}</a>";
                    }
                }

                var page = Pages.FirstOrDefault(x => StringHelper.EqualsIgoreCase(x.Path, post.GetFullLink()));
                if (page != null)
                {
                    post.PageStat = page;
                }

                post.HtmlMetadata1 = $@"<div class=""post-meta-row"">
                <a href=""/post/archive#year-{post.PublishDate.Year}"">
                    {post.PublishDate.ToCnDate()}
                </a>
                <span> &middot; </span>
                <span>
                    {post.PageStat.Hit} 次访问
                </span>
            </div>";
                post.HtmlMetadata2 = $@"<div class=""post-tags-row"">
                    {string.Join(" ", tagSnippets)}
                    {seriesSnippet}
            </div>";
            }

            if (!_adminOnly)
            {
                Tags.RemoveAll(x => !x.BlogPosts.Any());
                Series.RemoveAll(x => !x.BlogPosts.Any());
            }

            // Build reverse index: tag → posts for O(1) lookup
            var tagToPosts = new Dictionary<PostTag, List<SwanPost>>();
            foreach (var tag in Tags)
            {
                if (tag.BlogPosts.Count > 0)
                {
                    tagToPosts[tag] = tag.BlogPosts;
                }
            }

            foreach (var post in Posts)
            {
                const int maxTagItems = 8;
                const int maxSeriesItems = 6;

                if (post.BlogTags.Any())
                {
                    var seen = new HashSet<SwanPost> { post };
                    foreach (var tag in post.BlogTags)
                    {
                        if (tagToPosts.TryGetValue(tag, out var tagPosts))
                        {
                            foreach (var tp in tagPosts)
                            {
                                seen.Add(tp);
                            }
                        }
                    }
                    seen.Remove(post);
                    if (seen.Count > 0)
                    {
                        var similarPosts = Random.Shared.GetItems(seen.ToArray(), Math.Min(maxTagItems, seen.Count));
                        post.RecommendPostsByTag.AddRange(similarPosts.DistinctBy(x => x.Id));
                    }
                }

                if (post.BlogSeries != null)
                {
                    // Older posts from same series (up to half budget)
                    post.RecommendPostsBySeries.AddRange(post.BlogSeries.BlogPosts
                        .Where(x => x.PublishDate < post.PublishDate && !post.RecommendPostsByTag.Contains(x))
                        .OrderByDescending(x => x.PublishDate).Take(maxSeriesItems / 2)
                        .DistinctBy(x => x.Id));
                    // Fill remaining budget with newer posts
                    var taken = post.RecommendPostsBySeries.Count();
                    post.RecommendPostsBySeries.AddRange(post.BlogSeries.BlogPosts
                        .Where(x => x.PublishDate > post.PublishDate && !post.RecommendPostsByTag.Contains(x))
                        .OrderBy(x => x.PublishDate).Take(maxSeriesItems - taken)
                        .DistinctBy(x => x.Id));
                    post.RecommendPostsBySeries.Add(post);
                }
            }

            foreach (var read in ReadItems)
            {
                var metadatas = new List<string>();
                if (!string.IsNullOrEmpty(read.Author))
                {
                    if (!string.IsNullOrEmpty(read.AuthorCountry))
                    {
                        metadatas.Add($"({read.AuthorCountry}){read.Author}");
                    }
                    else
                    {
                        metadatas.Add(read.Author);
                    }
                }

                if (!string.IsNullOrEmpty(read.Translator))
                {
                    metadatas.Add($"{read.Translator}(译)");
                }

                read.HtmlMetadata = string.Join("/", metadatas);
                read.HtmlComment = MarkdownHelper.ToHtml(read.Comment);

                foreach (var p in read.Posts)
                {
                    var post = Posts.FirstOrDefault(x => StringHelper.EqualsIgoreCase(x.Id, p));
                    if (post != null)
                    {
                        read.BlogPosts.Add(post);
                    }
                }
            }
        }
    }
}
