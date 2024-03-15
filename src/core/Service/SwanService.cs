using Swan.Core.Model;
using Swan.Core.Store;

namespace Swan.Core.Service
{
    public class SwanService
    {
        private readonly ILogger<SwanService> _logger;
        private readonly ISwanDatabase _swanDatabase;

        public SwanService(
            ISwanDatabase swanDatabase,
            ILogger<SwanService> logger)
        {
            _swanDatabase = swanDatabase;
            _logger = logger;
        }

        #region SwanTag

        public async Task<List<SwanTag>> GetTagsAsync(bool publicOnly = true)
        {
            var tagQuery = new DatabaseQuery();
            if(publicOnly)
            {
                tagQuery.Add(nameof(SwanTag.IsPublic), 1);
            }

            var tags = await _swanDatabase.QueryAsync<SwanTag>(tagQuery);
            foreach (var tag in tags)
            {
                var postQuery = new DatabaseQuery();
                if(publicOnly)
                {
                    postQuery.Add(nameof(SwanPost.IsPublic), 1);
                }

                var taggedPosts = await _swanDatabase.QueryAsync<SwanPost>(postQuery);
                tag.Posts.AddRange(taggedPosts);
            }

            return tags;
        }

        public async Task<SwanTag> GetTagAsync(string url, bool publicOnly = true)
        {
            var tagQuery = new DatabaseQuery();
            tagQuery.Add(nameof(SwanTag.Url), url);

            if (publicOnly)
            {
                tagQuery.Add(nameof(SwanTag.IsPublic), 1);
            }

            var tag = await _swanDatabase.QueryFirstOrDefaultAsync<SwanTag>(tagQuery);
            if(tag == null)
            {
                return null;
            }

            var postQuery = new DatabaseQuery();
            postQuery.Add(nameof(SwanPost.TagId), tag.Id);

            if (publicOnly)
            {
                postQuery.Add(nameof(SwanPost.IsPublic), 1);
            }

            var taggedPosts = await _swanDatabase.QueryAsync<SwanPost>(postQuery);
            tag.Posts.AddRange(taggedPosts);

            return tag;
        }

        public async Task<SwanTag> GetTagAsync(int id)
        {
            var tagQuery = new DatabaseQuery();
            tagQuery.Add(nameof(SwanTag.Id), id);

            var tag = await _swanDatabase.QueryFirstOrDefaultAsync<SwanTag>(tagQuery);
            if (tag == null)
            {
                return null;
            }

            var postQuery = new DatabaseQuery();
            postQuery.Add(nameof(SwanPost.TagId), tag.Id);

            var taggedPosts = await _swanDatabase.QueryAsync<SwanPost>(postQuery);
            tag.Posts.AddRange(taggedPosts);

            return tag;
        }

        public async Task UpdateTagAsync(SwanTag tag)
        {
            var query = new DatabaseQuery();
            query.Add(nameof(SwanTag.Id), tag.Id);

            var oldTag = await _swanDatabase.QueryFirstOrDefaultAsync<SwanTag>(query);
            if(oldTag == null)
            {
                throw new Exception($"Failed to update tag, id {tag.Id} not exists.");
            }

            tag.CreatedAt = oldTag.CreatedAt;
            tag.LastModifiedAt = DateTime.Now;
            await _swanDatabase.UpdateAsync(tag);
        }

        #endregion

        #region SwanPost

        public async Task<List<SwanPost>> GetPostsAsync(bool publicOnly = true)
        {
            var postQuery = new DatabaseQuery();
            if (publicOnly)
            {
                postQuery.Add(nameof(SwanPost.IsPublic), 1);
            }

            var posts = await _swanDatabase.QueryAsync<SwanPost>(postQuery);
            foreach (var post in posts)
            {
                var tagQuery = new DatabaseQuery();
                if (publicOnly)
                {
                    tagQuery.Add(nameof(SwanTag.IsPublic), 1);
                }

                var taggedPosts = await _swanDatabase.QueryAsync<SwanPost>(tagQuery);
                post.Posts.AddRange(taggedPosts);
            }

            return posts;
        }

        public async Task<SwanTag> GetTagAsync(string url, bool publicOnly = true)
        {
            var tagQuery = new DatabaseQuery();
            tagQuery.Add(nameof(SwanTag.Url), url);

            if (publicOnly)
            {
                tagQuery.Add(nameof(SwanTag.IsPublic), 1);
            }

            var tag = await _swanDatabase.QueryFirstOrDefaultAsync<SwanTag>(tagQuery);
            if (tag == null)
            {
                return null;
            }

            var postQuery = new DatabaseQuery();
            postQuery.Add(nameof(SwanPost.TagId), tag.Id);

            if (publicOnly)
            {
                postQuery.Add(nameof(SwanPost.IsPublic), 1);
            }

            var taggedPosts = await _swanDatabase.QueryAsync<SwanPost>(postQuery);
            tag.Posts.AddRange(taggedPosts);

            return tag;
        }

        public async Task<SwanTag> GetTagAsync(int id)
        {
            var tagQuery = new DatabaseQuery();
            tagQuery.Add(nameof(SwanTag.Id), id);

            var tag = await _swanDatabase.QueryFirstOrDefaultAsync<SwanTag>(tagQuery);
            if (tag == null)
            {
                return null;
            }

            var postQuery = new DatabaseQuery();
            postQuery.Add(nameof(SwanPost.TagId), tag.Id);

            var taggedPosts = await _swanDatabase.QueryAsync<SwanPost>(postQuery);
            tag.Posts.AddRange(taggedPosts);

            return tag;
        }

        public async Task UpdateTagAsync(SwanTag tag)
        {
            var query = new DatabaseQuery();
            query.Add(nameof(SwanTag.Id), tag.Id);

            var oldTag = await _swanDatabase.QueryFirstOrDefaultAsync<SwanTag>(query);
            if (oldTag == null)
            {
                throw new Exception($"Failed to update tag, id {tag.Id} not exists.");
            }

            tag.CreatedAt = oldTag.CreatedAt;
            tag.LastModifiedAt = DateTime.Now;
            await _swanDatabase.UpdateAsync(tag);
        }

        #endregion
    }
}
