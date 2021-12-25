using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Laobian.Share
{
    [DataContract]
    public class DbStat
    {
        [DataMember(Order = 1)]
        [JsonPropertyName("fn")]
        public string FolderName { get; set; }

        [JsonPropertyName("fs")]
        [DataMember(Order = 2)]
        public long FolderSize { get; set; }

        [JsonPropertyName("sf")]
        [DataMember(Order = 3)]
        public int SubFolderCount { get; set; }

        [JsonPropertyName("fc")]
        [DataMember(Order = 4)]
        public int FileCount { get; set; }
    }
}
