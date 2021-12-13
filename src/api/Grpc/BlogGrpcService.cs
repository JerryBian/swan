using System;
using System.Collections.Generic;
using Laobian.Share.Grpc.Request;
using Laobian.Share.Grpc.Response;
using Laobian.Share.Grpc.Service;
using ProtoBuf.Grpc;
using System.Threading.Tasks;
using Laobian.Api.Controllers;
using Laobian.Api.HttpClients;
using Laobian.Api.Repository;
using Laobian.Share.Site.Blog;
using Laobian.Share.Util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Laobian.Api.Grpc
{
    public class BlogGrpcService : IBlogGrpcService
    {
        private readonly BlogSiteHttpClient _blogSiteHttpClient;
        private readonly IFileRepository _fileRepository;
        private readonly ILogger<BlogGrpcService> _logger;

        public BlogGrpcService(IFileRepository fileRepository,
            ILogger<BlogGrpcService> logger, BlogSiteHttpClient blogSiteHttpClient)
        {
            _logger = logger;
            _fileRepository = fileRepository;
            _blogSiteHttpClient = blogSiteHttpClient;
        }

        public async Task<BlogResponse> AddPostAccessAsync(BlogRequest request, CallContext context = default)
        {
            var response = new BlogResponse();
            try
            {
                var post = await _fileRepository.GetBlogPostAsync(request.Link);
                if (post == null)
                {
                    _logger.LogWarning($"No post found with link {request.Link}, new access will be discarded.");
                }
                else
                {
                    await _fileRepository.AddBlogPostAccessAsync(post, DateTime.Now, 1);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(AddPostAccessAsync)}({request.Link}) failed.");
                response.IsOk = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<BlogResponse> AddPostAsync(BlogRequest request, CallContext context = default)
        {
            var response = new BlogResponse();
            try
            {
                await _fileRepository.AddBlogPostAsync(request.Post);
                await _blogSiteHttpClient.ReloadBlogDataAsync();
                response.Post = request.Post;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(AddPostAsync)}({JsonUtil.Serialize(request.Post)}) failed.");
                response.IsOk = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<BlogResponse> AddTagAsync(BlogRequest request, CallContext context = default)
        {
            var response = new BlogResponse();
            try
            {
                await _fileRepository.AddBlogTagAsync(request.Tag);
                await _blogSiteHttpClient.ReloadBlogDataAsync();
                response.Tag = request.Tag;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(AddTagAsync)} failed.");
                response.IsOk = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<BlogResponse> DeleteTagAsync(BlogRequest request, CallContext context = default)
        {
            var response = new BlogResponse();
            try
            {
                await _fileRepository.DeleteBlogTagAsync(request.TagId);
                var posts = await _fileRepository.GetBlogPostsAsync();
                foreach (var blogPost in posts)
                {
                    if (blogPost.Tag.Contains(request.TagId))
                    {
                        blogPost.Tag.Remove(request.TagId);
                        await _fileRepository.UpdateBlogPostAsync(blogPost, blogPost.Link);
                    }
                }

                await _blogSiteHttpClient.ReloadBlogDataAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(DeleteTagAsync)} failed.");
                response.IsOk = false;
                response.Message = ex.Message;
            }

            return response;
        }

        private async Task<BlogPostRuntime> GetBlogPostRuntimeAsync(BlogPost post, bool extractRuntime = true)
        {
            var blogPostRuntime = new BlogPostRuntime(post);
            if (extractRuntime)
            {
                var blogPostAccess = await _fileRepository.GetBlogPostAccessAsync(post.Link);
                var blogTags = new List<BlogTag>();
                foreach (var blogPostTag in post.Tag)
                {
                    var tag = await _fileRepository.GetBlogTagAsync(blogPostTag);
                    if (tag != null)
                    {
                        blogTags.Add(tag);
                    }
                }

                blogPostRuntime.ExtractRuntimeData(blogPostAccess, blogTags);
            }

            return blogPostRuntime;
        }

        public async Task<BlogResponse> GetPostAsync(BlogRequest request, CallContext context = default)
        {
            var response = new BlogResponse();
            try
            {
                var post = await _fileRepository.GetBlogPostAsync(request.Link);
                if (post == null)
                {
                    response.IsOk = false;
                    response.Message = $"Post with link not found: {request.Link}";
                }
                else
                {
                    var blogPostRuntime = await GetBlogPostRuntimeAsync(post, request.ExtractRuntime);
                    response.PostRuntime = blogPostRuntime;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(GetPostAsync)}({request.Link}) failed.");
                response.IsOk = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<BlogResponse> GetPostsAsync(BlogRequest request, CallContext context = default)
        {
            var response = new BlogResponse();
            try
            {
                var posts = await _fileRepository.GetBlogPostsAsync();
                var result = new List<BlogPostRuntime>();
                foreach (var blogPost in posts)
                {
                    var blogPostRuntime = await GetBlogPostRuntimeAsync(blogPost, request.ExtractRuntime);
                    result.Add(blogPostRuntime);
                }

                response.Posts = result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(GetPostsAsync)} failed.");
                response.IsOk = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<BlogResponse> GetTagAsync(BlogRequest request, CallContext context = default)
        {
            var response = new BlogResponse();
            try
            {
                var tag = await _fileRepository.GetBlogTagAsync(request.TagId);
                if (tag == null)
                {
                    response.IsOk = false;
                    response.Message = $"Tag id = {request.TagId} not found.";
                }
                else
                {
                    response.Tag = tag;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(GetTagAsync)} failed.");
                response.IsOk = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<BlogResponse> GetTagsAsync(CallContext context = default)
        {
            var response = new BlogResponse();
            try
            {
                var tags = await _fileRepository.GetBlogTagsAsync();
                response.Tags = tags;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(GetTagsAsync)} failed.");
                response.IsOk = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<BlogResponse> UpdatePostAsync(BlogRequest request, CallContext context = default)
        {
            var response = new BlogResponse();
            try
            {
                await _fileRepository.UpdateBlogPostAsync(request.Post, request.ReplacedPostLink);
                await _blogSiteHttpClient.ReloadBlogDataAsync();
                response.Post = request.Post;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(UpdatePostAsync)}({JsonUtil.Serialize(request.Post)}) failed.");
                response.IsOk = false;
                response.Message = ex.Message;
            }

            return response;
        }

        public async Task<BlogResponse> UpdateTagAsync(BlogRequest request, CallContext context = default)
        {
            var response = new BlogResponse();
            try
            {
                await _fileRepository.UpdateBlogTagAsync(request.Tag);
                await _blogSiteHttpClient.ReloadBlogDataAsync();
                response.Tag = request.Tag;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(UpdateTagAsync)} failed.");
                response.IsOk = false;
                response.Message = ex.Message;
            }

            return response;
        }
    }
}
