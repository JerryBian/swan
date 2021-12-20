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

    Task<List<Note>> GetNotesAsync(int? year = null, CancellationToken cancellationToken = default);

    Task<Note> GetNoteAsync(string link, CancellationToken cancellationToken = default);

    Task AddNoteAsync(Note note, CancellationToken cancellationToken = default);

    Task UpdateNoteAsync(Note note, CancellationToken cancellationToken = default);
}