using System;
using System.Runtime.Serialization;
using Laobian.Share.Site.Jarvis;

namespace Laobian.Share.Grpc.Request;

[DataContract]
public class DiaryGrpcRequest
{
    [DataMember(Order = 1)] public bool ExtractRuntime { get; set; }

    [DataMember(Order = 2)] public DateTime Date { get; set; }

    [DataMember(Order = 3)] public int? Year { get; set; }

    [DataMember(Order = 4)] public Diary Diary { get; set; }

    [DataMember(Order = 5)] public int Offset { get; set; }

    [DataMember(Order = 6)] public int? Count { get; set; }

    [DataMember(Order = 7)] public int? Month { get; set; }

    [DataMember(Order = 8)] public bool ExtractPrev { get; set; }

    [DataMember(Order = 9)] public bool ExtractNext { get; set; }
}