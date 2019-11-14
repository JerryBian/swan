using System.Collections.Generic;
using System.Linq;
using Laobian.Blog.Models;
using Laobian.Share.Blog;
using Laobian.Share.Config;
using Laobian.Share.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Laobian.Blog.ViewComponents
{
    public class PostRecommendViewComponent : ViewComponent
    {
        private readonly AppConfig _appConfig;
        private readonly IBlogService _blogService;

        public PostRecommendViewComponent(IBlogService blogService, IOptions<AppConfig> appConfig)
        {
            _appConfig = appConfig.Value;
            _blogService = blogService;
        }

        public IViewComponentResult Invoke(bool adminView, string excludedLink)
        {
            var posts = adminView ? _blogService.GetPosts(false, true, false) : _blogService.GetPosts();
            var post = posts.FirstOrDefault(p => CompareHelper.IgnoreCase(excludedLink, p.Link));
            var postsWithWeight = new List<PostWithWeight>();
            var latestPost = posts.FirstOrDefault();
            foreach (var blogPost in posts)
            {
                if (blogPost == post)
                {
                    continue;
                }

                var postWithWeight = new PostWithWeight { Post = blogPost };
                if (post == null)
                {
                    if (latestPost != null)
                    {
                        postWithWeight.TicksDiff = (blogPost.PublishTime - latestPost.PublishTime).Ticks;
                    }
                }
                else
                {
                    if (blogPost.Categories.Any(c => post.Categories.Contains(c)))
                    {
                        postWithWeight.Weight += 0.7;
                    }

                    if (blogPost.Tags.Any(t => post.Tags.Contains(t)))
                    {
                        postWithWeight.Weight += 0.9;
                    }
                }

                postsWithWeight.Add(postWithWeight);
            }

            double totalVisits = 1.0;
            double totalTicksDiff = 1.0;
            foreach (var postWithWeight in postsWithWeight)
            {
                totalVisits += postWithWeight.Post.AccessCount;
                totalTicksDiff += postWithWeight.TicksDiff;
            }

            foreach (var postWithWeight in postsWithWeight)
            {
                postWithWeight.Weight -= postWithWeight.Post.AccessCount / (double)totalVisits;
                postWithWeight.Weight += postWithWeight.TicksDiff / totalTicksDiff;
            }

            return View(postsWithWeight.OrderByDescending(pw => pw.Weight).Take(8).Select(pw => pw.Post));
        }
    }
}
