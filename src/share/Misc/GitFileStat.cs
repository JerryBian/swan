using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Laobian.Share.Misc;

[DataContract]
public class GitFileStat
{
    [DataMember(Order = 1)]
    [JsonPropertyName("fn")]
    public string FolderName { get; set; }

    [JsonPropertyName("fs")]
    [DataMember(Order = 2)]
    public string FolderSize { get; set; }

    [JsonPropertyName("sf")]
    [DataMember(Order = 3)]
    public int SubFolderCount { get; set; }

    [JsonPropertyName("fc")]
    [DataMember(Order = 4)]
    public int FileCount { get; set; }
}