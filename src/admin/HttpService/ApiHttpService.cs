using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Laobian.Share.Blog;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Laobian.Admin.HttpService
{
    public class ApiHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApiHttpService> _logger;

        public ApiHttpService(HttpClient httpClient, ILogger<ApiHttpService> logger, IOptions<AdminConfig> config)
        {
            _logger = logger;
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(config.Value.ApiLocalEndpoint);
        }

        public async Task<bool> ReloadBlogDataAsync(string message)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<BlogPost>> GetPostsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<List<BlogTag>> GetTagsAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<BlogPost> GetPostAsync(string link)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdatePostMetadataAsync(BlogPostMetadata metadata)
        {
            throw new NotImplementedException();
        }

        public async Task<BlogTag> GetTagAsync(string link)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteTagAsync(string link)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> AddTagAsync(BlogTag tag)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateTagAsync(BlogTag tag)
        {
            throw new NotImplementedException();
        }
    }
}
