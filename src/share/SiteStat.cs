using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Laobian.Share;

[DataContract]
public class SiteStat
{
    [DataMember(Order = 1)]
    [JsonPropertyName("startTime")]
    public DateTime StartTime { get; set; }

    [DataMember(Order = 2)]
    [JsonPropertyName("threads")]
    public int Threads { get; set; }

    [DataMember(Order = 3)]
    [JsonPropertyName("totalProcessorTime")]
    public string TotalProcessorTime { get; set; }

    [DataMember(Order = 4)]
    [JsonPropertyName("allocatedPhysicalMemory")]
    public string AllocatedPhysicalMemory { get; set; }

    [DataMember(Order = 5)]
    [JsonPropertyName("maximumAllocatedPhysicalMemory")]
    public string MaximumAllocatedPhysicalMemory { get; set; }
}