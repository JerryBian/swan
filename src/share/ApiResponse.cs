using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Laobian.Share
{
    public class ApiResponse<T>
    {
        [JsonPropertyOrder(1)]
        [JsonPropertyName("isSucceed")]
        public bool IsSucceed { get; set; }

        [JsonPropertyOrder(2)]
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyOrder(3)]
        [JsonPropertyName("content")]
        public T Content { get; set; }
    }
}
