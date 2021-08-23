using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share;

namespace Laobian.Api.Source
{
    public interface IFileSource
    {
        Task<List<string>> ReadBlogPostsAsync(CancellationToken cancellationToken = default);

        Task<string> ReadBlogPostAsync(string postLink, CancellationToken cancellationToken = default);

        Task WriteBlogPostAsync(int year, string postLink, string content,
            CancellationToken cancellationToken = default);

        Task<string> ReadBlogPostAccessAsync(string postLink, CancellationToken cancellationToken = default);

        Task WriteBlogPostAccessAsync(int year, string postLink, string content,
            CancellationToken cancellationToken = default);

        Task<string> ReadBlogTagsAsync(CancellationToken cancellationToken = default);

        Task WriteBlogTagsAsync(string blogTags, CancellationToken cancellationToken = default);

        Task<string> ReadLogsAsync(LaobianSite site, DateTime date, CancellationToken cancellationToken = default);

        Task AppendLogAsync(LaobianSite site, DateTime date, string log, CancellationToken cancellationToken = default);

        Task<IDictionary<string, string>> ReadBookItemsAsync(CancellationToken cancellationToken = default);

        Task<string> ReadBookItemsAsync(int year, CancellationToken cancellationToken = default);

        Task WriteBookItemsAsync(int year, string content, CancellationToken cancellationToken = default);

        Task<string> AddRawFileAsync(string fileName, byte[] content, CancellationToken cancellationToken = default);

        Task FlushAsync(string message);

        Task PrepareAsync(CancellationToken cancellationToken = default);
    }
}