using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Api.Source;
using Laobian.Share.Site.Jarvis;
using Laobian.Share.Util;
using Microsoft.Extensions.Logging;

namespace Laobian.Api.Repository;

public class FileRepository : IFileRepository
{
    private readonly IFileSource _fileSource;
    private readonly ILogger<FileRepository> _logger;

    public FileRepository(IFileSource fileSource,
        ILogger<FileRepository> logger)
    {
        _logger = logger;
        _fileSource = fileSource;
    }

    public async Task PrepareAsync(CancellationToken cancellationToken = default)
    {
        await _fileSource.PrepareAsync(cancellationToken);
    }

    public async Task SaveAsync(string message)
    {
        await _fileSource.FlushAsync(message);
    }

    public async Task<List<Note>> GetNotesAsync(int? year = null, CancellationToken cancellationToken = default)
    {
        var notes = await _fileSource.ListNotesAsync(year, cancellationToken);
        var result = new List<Note>();
        foreach (var note in notes)
        {
            result.Add(JsonUtil.Deserialize<Note>(note));
        }

        return result;
    }

    public async Task<Note> GetNoteAsync(string link, CancellationToken cancellationToken = default)
    {
        Note result = null;
        var note = await _fileSource.ReadNoteAsync(link, cancellationToken);
        if (!string.IsNullOrEmpty(note))
        {
            result = JsonUtil.Deserialize<Note>(note);
        }

        return result;
    }

    public async Task AddNoteAsync(Note note, CancellationToken cancellationToken = default)
    {
        var existingNote = await GetNoteAsync(note.Id, cancellationToken);
        if (existingNote != null)
        {
            throw new Exception($"Note with link({note.Id}) already exists.");
        }

        await _fileSource.WriteNoteAsync(note.Id, note.CreateTime.Year,
            JsonUtil.Serialize(note),
            cancellationToken);
    }

    public async Task UpdateNoteAsync(Note note, CancellationToken cancellationToken = default)
    {
        var existingNote = await GetNoteAsync(note.Id, cancellationToken);
        if (existingNote == null)
        {
            throw new Exception($"Note with link({note.Id}) not exists.");
        }

        existingNote.LastUpdateTime = DateTime.Now;
        existingNote.MdContent = note.MdContent;
        existingNote.Title = note.Title;
        existingNote.Tags = note.Tags;
        await _fileSource.WriteNoteAsync(note.Id, note.CreateTime.Year,
            JsonUtil.Serialize(existingNote),
            cancellationToken);
    }
}