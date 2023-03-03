using System.Text.Json.Serialization;

namespace Swan.Core.Model.Object
{
    public class LogObject : FileObjectBase
    {
        [JsonPropertyName("m")]
        public string Message { get; set; }

        [JsonPropertyName("e")]
        public string Exception { get; set; }

        [JsonPropertyName("t")]
        public DateTime Timestamp { get; set; }

        [JsonPropertyName("l")]
        public LogLevel Level { get; set; }

        public override string GetFileName()
        {
            return Constants.Asset.LogFile;
        }
    }
}
