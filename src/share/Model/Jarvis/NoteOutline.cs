using System.Runtime.Serialization;

namespace Laobian.Share.Model.Jarvis;

[DataContract]
public class NoteOutline
{
    [DataMember(Order = 1)] public string Link { get; set; }

    [DataMember(Order = 2)] public string Title { get; set; }
}