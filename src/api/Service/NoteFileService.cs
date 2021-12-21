using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Share.Site.Jarvis;

namespace Laobian.Api.Service;

public class NoteFileService : INoteFileService
{
    public async Task<List<Note>> GetNotesAsync(int offset = 0, int? count = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Note> GetNoteAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task AddNoteAsync(Note note, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateNoteAsync(Note note, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteNoteAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<List<NoteTag>> GetNoteTagsAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<NoteTag> GetNoteTagAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task AddNoteTagAsync(NoteTag tag, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateNoteTagAsync(NoteTag tag, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteNoteTagAsync(string id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}