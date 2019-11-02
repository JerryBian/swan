using System;
using System.Collections.Generic;
using System.Text;
using Laobian.Share.Blog.Model;

namespace Laobian.Share.Blog
{
    public class BlogAssetManager
    {
        private readonly List<BlogPost> _allPosts;
        private readonly List<BlogCategory> _allCategories;
        private readonly List<BlogTag> _allTags;

        public BlogAssetManager()
        {
            _allTags = new List<BlogTag>();
            _allPosts = new List<BlogPost>();
            _allCategories = new List<BlogCategory>();
        }

        public List<BlogPost> AllPosts => _allPosts;

        public List<BlogCategory> AllCategories => _allCategories;

        public List<BlogTag> AllTagss => _allTags;
    }
}
