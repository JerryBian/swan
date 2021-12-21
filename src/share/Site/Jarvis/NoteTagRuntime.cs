using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Laobian.Share.Site.Jarvis;

[DataContract]
public class NoteTagRuntime
{
    public NoteTagRuntime()
    {
    }

    public NoteTagRuntime(NoteTag tag)
    {
        Raw = tag;
    }

    [DataMember(Order = 1)] public NoteTag Raw { get; set; }

    [DataMember(Order = 2)] public List<Note> Notes { get; set; }

    public void ExtractRuntime(List<Note> notes)
    {
        if (notes == null)
        {
            return;
        }

        Notes = notes.Where(x => x.Tags.Contains(Raw.Id, StringComparer.InvariantCultureIgnoreCase)).ToList();
    }
}