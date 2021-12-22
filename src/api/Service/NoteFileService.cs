using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Laobian.Api.Repository;
using Laobian.Share;
using Laobian.Share.Site.Jarvis;
using Laobian.Share.Util;
using Microsoft.Extensions.Logging;

namespace Laobian.Api.Service;

public class NoteFileService : INoteFileService
{
    private readonly ILogger<NoteFileService> _logger;
    private readonly INoteFileRepository _noteFileRepository;

    public NoteFileService(ILogger<NoteFileService> logger, INoteFileRepository noteFileRepository)
    {
        _logger = logger;
        _noteFileRepository = noteFileRepository;
    }

    public async Task<List<Note>> GetNotesAsync(int? year = null,
        CancellationToken cancellationToken = default)
    {
        var notes = new List<Note>();
        var searchPath = Constants.AssetDbNotePostFolder;
        if (year != null)
        {
            searchPath = Path.Combine(searchPath, year.Value.ToString("D4"));
        }

        var noteFiles =
            await _noteFileRepository.SearchAsync("*.json", searchPath, cancellationToken);
        foreach (var noteFile in noteFiles)
        {
            var diaryJson = await _noteFileRepository.ReadAsync(noteFile, cancellationToken);
            notes.Add(JsonUtil.Deserialize<Note>(diaryJson));
        }

        return notes;
    }

    public async Task<Note> GetNoteAsync(string id, CancellationToken cancellationToken = default)
    {
        var noteFile =
            (await _noteFileRepository.SearchAsync($"{id.ToLowerInvariant()}.json", Constants.AssetDbNotePostFolder,
                cancellationToken: cancellationToken)).FirstOrDefault();
        if (!string.IsNullOrEmpty(noteFile))
        {
            var noteJson = await _noteFileRepository.ReadAsync(noteFile, cancellationToken);
            if (!string.IsNullOrEmpty(noteJson))
            {
                return JsonUtil.Deserialize<Note>(noteJson);
            }
        }

        return null;
    }

    public async Task AddNoteAsync(Note note, CancellationToken cancellationToken = default)
    {
        if (note == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(note.Id))
        {
            note.Id = StringUtil.GenerateRandom().ToLowerInvariant();
        }

        var existingNote = await GetNoteAsync(note.Id, cancellationToken);
        if (existingNote != null)
        {
            throw new Exception(
                $"Note with id({note.Id}) already exists.");
        }

        note.CreateTime = note.LastUpdateTime = DateTime.Now;
        await _noteFileRepository.WriteAsync(Path.Combine(Constants.AssetDbNotePostFolder, note.CreateTime.Year.ToString("D4"), $"{note.Id}.json"),
            JsonUtil.Serialize(note, true), cancellationToken);
    }

    public async Task UpdateNoteAsync(Note note, CancellationToken cancellationToken = default)
    {
        if (note == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(note.Id))
        {
            throw new Exception($"Note id does not exist.");
        }

        var existingNote = await GetNoteAsync(note.Id, cancellationToken);
        if (existingNote == null)
        {
            throw new Exception(
                $"Note with id({note.Id}) does not exist.");
        }

        note.CreateTime = existingNote.CreateTime;
        note.LastUpdateTime = DateTime.Now;
        await _noteFileRepository.WriteAsync(Path.Combine(Constants.AssetDbNotePostFolder, note.CreateTime.Year.ToString("D4"), $"{note.Id}.json"),
            JsonUtil.Serialize(note, true), cancellationToken);
    }

    public async Task DeleteNoteAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(id))
        {
            return;
        }

        var note = await GetNoteAsync(id, cancellationToken);
        if (note != null)
        {
            await _noteFileRepository.DeleteAsync(Path.Combine(Constants.AssetDbNotePostFolder, note.CreateTime.Year.ToString("D4"), $"{note.Id}.json"),
                cancellationToken);
        }
    }

    public async Task<List<NoteTag>> GetNoteTagsAsync(CancellationToken cancellationToken = default)
    {
        var tags = new List<NoteTag>();
        if (await _noteFileRepository.FileExistsAsync("tag.json", cancellationToken))
        {
            var tagJson = await _noteFileRepository.ReadAsync("tag.json", cancellationToken);
            if (!string.IsNullOrEmpty(tagJson))
            {
                tags.AddRange(JsonUtil.Deserialize<List<NoteTag>>(tagJson));
            }
        }

        return tags;
    }

    public async Task<NoteTag> GetNoteTagByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var tags =  await GetNoteTagsAsync(cancellationToken);
        return tags.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(id, x.Id));
    }

    public async Task<NoteTag> GetNoteTagByLinkAsync(string link, CancellationToken cancellationToken = default)
    {
        var tags = await GetNoteTagsAsync(cancellationToken);
        return tags.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(link, x.Link));
    }

    public async Task AddNoteTagAsync(NoteTag tag, CancellationToken cancellationToken = default)
    {
        if (tag == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(tag.Id))
        {
            tag.Id = StringUtil.GenerateRandom().ToLowerInvariant();
        }

        var tags = await GetNoteTagsAsync(cancellationToken);
        var existingTag = tags.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.Id, tag.Id));
        if (existingTag != null)
        {
            throw new Exception($"Tag with id({tag.Id}) already exists.");
        }

        existingTag = tags.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.Link, tag.Link));
        if (existingTag != null)
        {
            throw new Exception($"Tag with link({tag.Link}) already exists.");
        }

        existingTag = tags.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.DisplayName, tag.DisplayName));
        if (existingTag != null)
        {
            throw new Exception($"Tag with display name({tag.DisplayName}) already exists.");
        }

        tag.LastUpdatedAt = DateTime.Now;
        tags.Add(tag);
        await _noteFileRepository.WriteAsync("tag.json", JsonUtil.Serialize(tags, true), cancellationToken);
    }

    public async Task UpdateNoteTagAsync(NoteTag tag, CancellationToken cancellationToken = default)
    {
        if (tag == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(tag.Id))
        {
            throw new Exception("Empty tag id.");
        }

        var tags = await GetNoteTagsAsync(cancellationToken);
        var existingTag = tags.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.Link, tag.Link) && !StringUtil.EqualsIgnoreCase(x.Id, tag.Id));
        if (existingTag != null)
        {
            throw new Exception($"Tag with link({tag.Link}) already exists.");
        }

        existingTag = tags.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.DisplayName, tag.DisplayName) && !StringUtil.EqualsIgnoreCase(x.Id, tag.Id));
        if (existingTag != null)
        {
            throw new Exception($"Tag with display name({tag.DisplayName}) already exists.");
        }
        
        existingTag = tags.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.Id, tag.Id));
        if (existingTag == null)
        {
            throw new Exception($"Tag with id({tag.Id}) does not exist.");
        }

        existingTag.LastUpdatedAt = DateTime.Now;
        existingTag.DisplayName = tag.DisplayName;
        existingTag.Link = tag.Link;
        existingTag.Description = tag.Description;
        await _noteFileRepository.WriteAsync("tag.json", JsonUtil.Serialize(tags, true), cancellationToken);
    }

    public async Task DeleteNoteTagAsync(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(id))
        {
            return;
        }

        var tags = await GetNoteTagsAsync(cancellationToken);
        var existingTag = tags.FirstOrDefault(x => StringUtil.EqualsIgnoreCase(x.Id, id));
        if (existingTag != null)
        {
            var notes = await GetNotesAsync(cancellationToken: cancellationToken);
            var taggedNotes = notes.Where(x =>
                x.Tags != null && x.Tags.Contains(existingTag.Id, StringComparer.InvariantCultureIgnoreCase));
            foreach (var taggedNote in taggedNotes)
            {
                taggedNote.Tags.Remove(taggedNote.Tags.First(x => StringUtil.EqualsIgnoreCase(x, id)));
                taggedNote.LastUpdateTime = DateTime.Now;
                await _noteFileRepository.WriteAsync(Path.Combine(Constants.AssetDbNotePostFolder, taggedNote.CreateTime.Year.ToString("D4"), $"{taggedNote.Id}.json"),
                    JsonUtil.Serialize(taggedNote, true), cancellationToken);
            }

            tags.Remove(existingTag);
            await _noteFileRepository.WriteAsync("tag.json", JsonUtil.Serialize(tags, true), cancellationToken);
        }
    }
}