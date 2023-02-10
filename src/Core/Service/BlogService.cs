using Swan.Core.Model;
using Swan.Core.Model.Object;
using Swan.Core.Repository;

namespace Swan.Core.Service
{
    public class BlogService : IBlogService
    {
        private readonly IBlogPostObjectRepository _blogPostObjectRepository;

        public BlogService(IBlogPostObjectRepository blogPostObjectRepository)
        {
            _blogPostObjectRepository = blogPostObjectRepository;
        }

        public async Task<List<BlogPost>> GetAllPostsAsync()
        {
            var result = new List<BlogPost>();
            var objs = await _blogPostObjectRepository.GetAllAsync();
            foreach(var obj in objs)
            {
                var post = new BlogPost(obj);
                result.Add(post);
            }

            return result;
        }
    }
}
