using System.Runtime.Serialization;
using Laobian.Share.Model.Jarvis;

namespace Laobian.Share.Grpc.Request;

[DataContract]
public class NoteGrpcRequest
{
    [DataMember(Order = 1)] public int? Year { get; set; }

    [DataMember(Order = 2)] public bool ExtractRuntime { get; set; }

    [DataMember(Order = 3)] public Note Note { get; set; }

    [DataMember(Order = 4)] public string Id { get; set; }

    [DataMember(Order = 5)] public NoteTag Tag { get; set; }

    [DataMember(Order = 6)] public string TagLink { get; set; }

    [DataMember(Order = 7)] public int? Offset { get; set; }

    [DataMember(Order = 8)] public int Count { get; set; }
}