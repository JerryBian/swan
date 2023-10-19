using Swan.Core.Model;

namespace Swan.Core.Store
{
    internal class StoreObject
    {
        public List<BlogPost> BlogPosts { get; set; }

        public List<BlogTag> BlogTags { get; set; }

        public List<BlogSeries> BlogSeries { get; set; }

        public List<ReadItem> ReadItems { get; set; }
    }
}
