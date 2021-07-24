using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Share.Blog;
using Laobian.Share.Helper;

namespace Laobian.Api.Store
{
    public class BlogCommentStore
    {
        private readonly ConcurrentDictionary<string, List<BlogCommentItem>> _comments;

        public BlogCommentStore(IDictionary<string, string> val)
        {
            _comments = new ConcurrentDictionary<string, List<BlogCommentItem>>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var item in val)
            {
                _comments.TryAdd(item.Key, JsonHelper.Deserialize<List<BlogCommentItem>>(item.Value));
            }
        }

        public IDictionary<string, List<BlogCommentItem>> GetAll()
        {
            return _comments;
        }

        public void Add(string postLink, BlogCommentItem comment)
        {
            comment.LastUpdatedAt = comment.Timestamp = DateTime.Now;
            _comments.AddOrUpdate(postLink, x => new List<BlogCommentItem> {comment}, (x, val) =>
            {
                val.Add(comment);
                return val;
            });
        }

        public List<BlogCommentItem> GetByLink(string postLink)
        {
            if (_comments.TryGetValue(postLink, out var c))
            {
                return c;
            }

            return null;
        }

        public void Update(string postLink, BlogCommentItem comment)
        {
            if (!_comments.TryGetValue(postLink, out var c))
            {
                throw new InvalidOperationException($"Comment link not exist: {postLink}");
            }

            var item = c.FirstOrDefault(x => x.Id == comment.Id);
            if (item == null)
            {
                throw new InvalidOperationException($"Comment not exist.");
            }

            item.MdContent = comment.MdContent;
            item.Email = comment.Email;
            item.LastUpdatedAt = DateTime.Now;
            item.IpAddress = comment.IpAddress;
            item.IsAdmin = comment.IsAdmin;
            item.IsPublished = comment.IsPublished;
            item.IsReviewed = comment.IsReviewed;
            item.UserName = comment.UserName;
        }
    }
}
