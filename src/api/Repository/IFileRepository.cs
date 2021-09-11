using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share.Logger;
using Laobian.Share.Site;
using Laobian.Share.Site.Blog;
using Laobian.Share.Site.Jarvis;
using Laobian.Share.Site.Read;

namespace Laobian.Api.Repository
{
    public interface IFileRepository
    {
        Task PrepareAsync(CancellationToken cancellationToken = default);

        Task SaveAsync(string message);

        Task<List<BlogPost>> GetBlogPostsAsync(CancellationToken cancellationToken = default);

        Task<BlogPost> GetBlogPostAsync(string postLink, CancellationToken cancellationToken = default);

        Task AddBlogPostAsync(BlogPost blogPost, CancellationToken cancellationToken = default);

        Task UpdateBlogPostAsync(BlogPost blogPost, CancellationToken cancellationToken = default);


        Task<List<BlogAccess>> GetBlogPostAccessAsync(string postLink,
            CancellationToken cancellationToken = default);

        Task AddBlogPostAccessAsync(BlogPost blogPost, DateTime date, int count,
            CancellationToken cancellationToken = default);

        Task<List<BlogTag>> GetBlogTagsAsync(CancellationToken cancellationToken = default);

        Task<BlogTag> GetBlogTagAsync(string tagLink, CancellationToken cancellationToken = default);

        Task AddBlogTagAsync(BlogTag blogTag, CancellationToken cancellationToken = default);

        Task UpdateBlogTagAsync(BlogTag blogTag, CancellationToken cancellationToken = default);

        Task DeleteBlogTagAsync(string tagLink, CancellationToken cancellationToken = default);

        Task<List<BookItem>> GetBookItemsAsync(CancellationToken cancellationToken = default);

        Task<List<BookItem>> GetBookItemsAsync(int year, CancellationToken cancellationToken = default);

        Task AddBookItemAsync(BookItem bookItem, CancellationToken cancellationToken = default);

        Task UpdateBookItemAsync(BookItem bookItem, CancellationToken cancellationToken = default);

        Task DeleteBookItemAsync(string bookItemId,
            CancellationToken cancellationToken = default);

        Task<List<LaobianLog>> GetLogsAsync(LaobianSite site, DateTime date,
            CancellationToken cancellationToken = default);

        Task AddLogAsync(LaobianLog log, CancellationToken cancellationToken = default);

        Task<string> AddRawFileAsync(string fileName, byte[] content,
            CancellationToken cancellationToken = default);

        Task<Diary> GetDiaryAsync(DateTime date, CancellationToken cancellationToken = default);

        Task<List<DateTime>> ListDiariesAsync(int? year = null, int? month = null,
            CancellationToken cancellationToken = default);

        Task AddDiaryAsync(Diary diary, CancellationToken cancellationToken = default);

        Task UpdateDiaryAsync(Diary diary, CancellationToken cancellationToken = default);
    }
}