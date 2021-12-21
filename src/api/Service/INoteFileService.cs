using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share.Site.Jarvis;

namespace Laobian.Api.Service;

public interface INoteFileService
{
    Task<List<Note>> GetNotesAsync(int offset = 0, int? count = null,
        CancellationToken cancellationToken = default);

    Task<Note> GetNoteAsync(string id, CancellationToken cancellationToken = default);

    Task AddNoteAsync(Note note, CancellationToken cancellationToken = default);

    Task UpdateNoteAsync(Note note, CancellationToken cancellationToken = default);

    Task DeleteNoteAsync(string id, CancellationToken cancellationToken = default);

    Task<List<NoteTag>> GetNoteTagsAsync(CancellationToken cancellationToken = default);

    Task<NoteTag> GetNoteTagAsync(string id, CancellationToken cancellationToken = default);

    Task AddNoteTagAsync(NoteTag tag, CancellationToken cancellationToken = default);

    Task UpdateNoteTagAsync(NoteTag tag, CancellationToken cancellationToken = default);

    Task DeleteNoteTagAsync(string id, CancellationToken cancellationToken = default);
}