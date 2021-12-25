using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Laobian.Share
{
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
        public TimeSpan TotalProcessorTime { get; set; }

        [DataMember(Order = 4)]
        [JsonPropertyName("allocatedPhysicalMemory")]
        public long AllocatedPhysicalMemory { get; set; }

        [DataMember(Order = 5)]
        [JsonPropertyName("allocatedVirtualMemory")]
        public long AllocatedVirtualMemory { get; set; }

        [DataMember(Order = 6)]
        [JsonPropertyName("maximumAllocatedPhysicalMemory")]
        public long MaximumAllocatedPhysicalMemory { get; set; }

        [DataMember(Order = 7)]
        [JsonPropertyName("maximumAllocatedVirtualMemory")]
        public long MaximumAllocatedVirtualMemory { get; set; }
    }
}
