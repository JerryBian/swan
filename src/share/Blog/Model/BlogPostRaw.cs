using System;
using System.Collections.Generic;
using Laobian.Share.Blog.Parser;

namespace Laobian.Share.Blog.Model
{
    internal class BlogPostRaw
    {
        [BlogAssetMeta(BlogAssetMetaReturnType.String, "链接", "Link", "Url")]
        public string Link { get; set; }

        [BlogAssetMeta(BlogAssetMetaReturnType.String, "标题", "Title")]
        public string Title { get; set; }

        [BlogAssetMeta(BlogAssetMetaReturnType.DateTimeOffset, "创建时间", "CreateTime")]
        public DateTime? CreateTime { get; set; }

        [BlogAssetMeta(BlogAssetMetaReturnType.DateTimeOffset, "发表时间", "PublishTime")]
        public DateTime? PublishTime { get; set; }

        [BlogAssetMeta(BlogAssetMetaReturnType.DateTimeOffset, "更新时间", "LastUpdateTime")]
        public DateTime? LastUpdateTime { get; set; }

        [BlogAssetMeta(BlogAssetMetaReturnType.Bool, "草稿", "IsDraft")]
        public bool? IsDraft { get; set; }

        [BlogAssetMeta(BlogAssetMetaReturnType.Bool, "置顶", "IsTopping")]
        public bool? IsTopping { get; set; }

        [BlogAssetMeta(BlogAssetMetaReturnType.Bool, "数学文章", "IncludeMath")]
        public bool? IncludeMath { get; set; }

        [BlogAssetMeta(BlogAssetMetaReturnType.Int32, "访问数量", "AccessCount")]
        public int? AccessCount { get; set; }

        [BlogAssetMeta(BlogAssetMetaReturnType.ListOfString, "文章分类", "Category")]
        public List<string> Category { get; set; }

        [BlogAssetMeta(BlogAssetMetaReturnType.ListOfString, "文章标签", "Tag")]
        public List<string> Tag { get; set; }

        public bool IsDraftDefault => false;

        public bool IsToppingDefault => false;

        public string Markdown { get; set; }
    }
}
