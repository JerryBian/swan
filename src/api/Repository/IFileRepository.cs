using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share.Logger;
using Laobian.Share.Site;
using Laobian.Share.Site.Blog;
using Laobian.Share.Site.Jarvis;
using Laobian.Share.Site.Read;

namespace Laobian.Api.Repository;

public interface IFileRepository
{
    Task PrepareAsync(CancellationToken cancellationToken = default);

    Task SaveAsync(string message);

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

    Task<List<Note>> GetNotesAsync(int? year = null, CancellationToken cancellationToken = default);

    Task<Note> GetNoteAsync(string link, CancellationToken cancellationToken = default);

    Task AddNoteAsync(Note note, CancellationToken cancellationToken = default);

    Task UpdateNoteAsync(Note note, CancellationToken cancellationToken = default);
}