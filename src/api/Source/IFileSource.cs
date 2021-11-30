using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share.Site;

namespace Laobian.Api.Source;

public interface IFileSource
{
    Task<List<string>> ReadBlogPostsAsync(CancellationToken cancellationToken = default);

    Task<string> ReadBlogPostAsync(string postLink, CancellationToken cancellationToken = default);

    Task DeleteBlogPostAsync(string postLink, CancellationToken cancellationToken = default);

    Task WriteBlogPostAsync(int year, string postLink, string content,
        CancellationToken cancellationToken = default);

    Task<string> ReadBlogPostAccessAsync(string postLink, CancellationToken cancellationToken = default);

    Task WriteBlogPostAccessAsync(int year, string postLink, string content,
        CancellationToken cancellationToken = default);

    Task RenameBlogPostAccessAsync(int year, string oldPostLink, string newPostLink,
        CancellationToken cancellationToken = default);

    Task<string> ReadBlogTagsAsync(CancellationToken cancellationToken = default);

    Task WriteBlogTagsAsync(string blogTags, CancellationToken cancellationToken = default);

    Task<string> ReadLogsAsync(LaobianSite site, DateTime date, CancellationToken cancellationToken = default);

    Task AppendLogAsync(LaobianSite site, DateTime date, string log, CancellationToken cancellationToken = default);

    Task<IDictionary<string, string>> ReadBookItemsAsync(CancellationToken cancellationToken = default);

    Task<string> ReadBookItemsAsync(int year, CancellationToken cancellationToken = default);

    Task WriteBookItemsAsync(int year, string content, CancellationToken cancellationToken = default);

    Task<string> AddRawFileAsync(string fileName, byte[] content, CancellationToken cancellationToken = default);

    Task<string> ReadDiaryAsync(DateTime date, CancellationToken cancellationToken = default);

    Task WriteDiaryAsync(DateTime date, string diary, CancellationToken cancellationToken = default);

    Task<List<DateTime>> ListDiariesAsync(int? year = null, int? month = null,
        CancellationToken cancellationToken = default);

    Task<string> ReadNoteAsync(string link, CancellationToken cancellationToken = default);

    Task WriteNoteAsync(string link, int year, string note, CancellationToken cancellationToken = default);

    Task<List<string>> ListNotesAsync(int? year = null,
        CancellationToken cancellationToken = default);

    Task FlushAsync(string message);

    Task PrepareAsync(CancellationToken cancellationToken = default);
}