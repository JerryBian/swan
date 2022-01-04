using System.Collections.Generic;
using System.Runtime.Serialization;
using Laobian.Share.Model.Jarvis;

namespace Laobian.Share.Grpc.Response;

[DataContract]
public class NoteGrpcResponse : GrpcResponseBase
{
    [DataMember(Order = 1)] public Note Note { get; set; }

    [DataMember(Order = 2)] public NoteRuntime NoteRuntime { get; set; }

    [DataMember(Order = 3)] public List<NoteRuntime> Notes { get; set; } = new();

    [DataMember(Order = 4)] public Dictionary<string, int> Data { get; set; } = new();

    [DataMember(Order = 5)] public NoteTag Tag { get; set; }

    [DataMember(Order = 6)] public NoteTagRuntime TagRuntime { get; set; }

    [DataMember(Order = 7)] public List<NoteTagRuntime> Tags { get; set; } = new();

    [DataMember(Order = 8)] public int Count { get; set; }
}