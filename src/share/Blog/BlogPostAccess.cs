using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Laobian.Share.Blog
{
    public class BlogPostAccess
    {
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        [JsonIgnore]
        public string DateString => Date.ToString("yyyy-MM");

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}
