using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Blog.Models;
using Laobian.Share.BlogEngine;
using Laobian.Share.BlogEngine.Model;
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

        public async Task<IViewComponentResult> InvokeAsync(bool adminView, string excludedLink)
        {
            List<BlogPost> posts;
            if (adminView)
            {
                posts = _blogService.GetPosts()
                    .Where(p => p.IsReallyPublic)
                    .ToList();
            }
            else
            {
                posts = _blogService.GetPosts().ToList();
            }

            var post = posts.FirstOrDefault(p => StringEqualsHelper.IgnoreCase(excludedLink, p.Link));
            var postsWithWeight = new List<PostWithWeight>();
            var latestPost = posts.OrderByDescending(p => p.CreationTimeUtc).FirstOrDefault();
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
                        postWithWeight.TicksDiff = (blogPost.CreationTimeUtc - latestPost.CreationTimeUtc).Ticks;
                    }
                }
                else
                {
                    if (blogPost.CategoryNames.Any(c => post.CategoryNames.Contains(c)))
                    {
                        postWithWeight.Weight += 0.7;
                    }

                    if (blogPost.TagNames.Any(t => post.TagNames.Contains(t)))
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
                totalVisits += postWithWeight.Post.Visits;
                totalTicksDiff += postWithWeight.TicksDiff;
            }

            foreach (var postWithWeight in postsWithWeight)
            {
                postWithWeight.Weight -= postWithWeight.Post.Visits / (double)totalVisits;
                postWithWeight.Weight += postWithWeight.TicksDiff / totalTicksDiff;
            }

            return View(postsWithWeight.OrderByDescending(pw => pw.Weight).Take(8).Select(pw=>pw.Post));
        }
    }
}
