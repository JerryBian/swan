using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Laobian.Share.Grpc.Response;
using Laobian.Share.Model.Jarvis;

namespace Laobian.Share.Grpc.Request;

[DataContract]
public class DiaryGrpcResponse : GrpcResponseBase
{
    [DataMember(Order = 1)] public Diary Diary { get; set; }

    [DataMember(Order = 2)] public DiaryRuntime DiaryRuntime { get; set; }

    [DataMember(Order = 3)] public List<DiaryRuntime> DiaryRuntimeList { get; set; }

    [DataMember(Order = 4)] public List<DateTime> DiaryDates { get; set; }

    [DataMember(Order = 5)] public bool NotFound { get; set; }
}