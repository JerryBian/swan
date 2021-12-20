using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Api.Source;
using Laobian.Share.Extension;
using Laobian.Share.Logger;
using Laobian.Share.Site;
using Laobian.Share.Site.Blog;
using Laobian.Share.Site.Jarvis;
using Laobian.Share.Site.Read;
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

    public async Task<Diary> GetDiaryAsync(DateTime date, CancellationToken cancellationToken = default)
    {
        Diary result = null;
        var diary = await _fileSource.ReadDiaryAsync(date, cancellationToken);
        if (!string.IsNullOrEmpty(diary))
        {
            result = JsonUtil.Deserialize<Diary>(diary);
        }

        return result;
    }

    public async Task<List<DateTime>> ListDiariesAsync(int? year = null, int? month = null,
        CancellationToken cancellationToken = default)
    {
        var dates = await _fileSource.ListDiariesAsync(year, month, cancellationToken);
        return dates;
    }

    public async Task AddDiaryAsync(Diary diary, CancellationToken cancellationToken = default)
    {
        var existingDiary = await GetDiaryAsync(diary.Date, cancellationToken);
        if (existingDiary != null)
        {
            throw new Exception($"Diary with date({diary.Date.ToDate()}) already exists.");
        }

        diary.CreateTime = diary.LastUpdateTime = DateTime.Now;
        await _fileSource.WriteDiaryAsync(diary.Date,
            JsonUtil.Serialize(diary),
            cancellationToken);
    }

    public async Task UpdateDiaryAsync(Diary diary, CancellationToken cancellationToken = default)
    {
        var existingDiary = await GetDiaryAsync(diary.Date, cancellationToken);
        if (existingDiary == null)
        {
            throw new Exception($"Diary with date({diary.Date.ToDate()}) not exists.");
        }

        diary.CreateTime = existingDiary.CreateTime;
        diary.LastUpdateTime = DateTime.Now;
        await _fileSource.WriteDiaryAsync(diary.Date,
            JsonUtil.Serialize(diary),
            cancellationToken);
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
        var existingNote = await GetNoteAsync(note.Link, cancellationToken);
        if (existingNote != null)
        {
            throw new Exception($"Note with link({note.Link}) already exists.");
        }

        await _fileSource.WriteNoteAsync(note.Link, note.CreateTime.Year,
            JsonUtil.Serialize(note),
            cancellationToken);
    }

    public async Task UpdateNoteAsync(Note note, CancellationToken cancellationToken = default)
    {
        var existingNote = await GetNoteAsync(note.Link, cancellationToken);
        if (existingNote == null)
        {
            throw new Exception($"Note with link({note.Link}) not exists.");
        }

        existingNote.LastUpdateTime = DateTime.Now;
        existingNote.MdContent = note.MdContent;
        existingNote.Title = note.Title;
        existingNote.Tag = note.Tag;
        await _fileSource.WriteNoteAsync(note.Link, note.CreateTime.Year,
            JsonUtil.Serialize(existingNote),
            cancellationToken);
    }
}