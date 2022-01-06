using System.Collections.Generic;
using Laobian.Share.Model.Jarvis;

namespace Laobian.Admin.Models;

public class NotePostUpdateViewModel
{
    public Note Post { get; set; }

    public List<NoteTag> Tags { get; } = new();
}