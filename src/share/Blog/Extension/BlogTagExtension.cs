using System.Collections.Generic;
using Laobian.Share.Blog.Model;
using Laobian.Share.Helper;

namespace Laobian.Share.Blog.Extension
{
    public static class BlogTagExtension
    {
        public static string GetLink(this BlogTag tag)
        {
            return $"/tag/#{tag.Link}/";
        }

        public static string GetRelativeLink(this BlogTag tag)
        {
            return $"#{tag.Link}/";
        }

        public static void Resolve(this BlogTag tag, List<BlogPost> allPosts)
        {
            tag.Posts.Clear();
            if (allPosts == null)
            {
                return;
            }

            foreach (var post in allPosts)
            {
                foreach (var tagName in post.Raw.Tag)
                {
                    if (CompareHelper.IgnoreCase(tagName, tag.Name))
                    {
                        tag.Posts.Add(post);
                        break;
                    }
                }
            }
        }
    }
}
