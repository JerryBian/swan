using System.Collections.Generic;
using System.Runtime.Serialization;
using Laobian.Share.Site.Jarvis;

namespace Laobian.Share.Grpc.Response;

[DataContract]
public class NoteGrpcResponse : GrpcResponseBase
{
    [DataMember(Order = 1)]
    public Note Note { get; set; }

    [DataMember(Order = 2)]
    public NoteRuntime NoteRuntime { get; set; }

    [DataMember(Order = 3)]
    public List<NoteRuntime> Notes { get; set; }

    [DataMember(Order = 4)]
    public Dictionary<string, int> Data { get; set; }
}