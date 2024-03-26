using System.Text.Json.Serialization;
using System.Text.Json;

namespace Swan.Core.Converter
{
    public class IsoDateTimeZoneConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var str = reader.GetString();
            return DateTime.Parse(str ?? string.Empty);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("yyyy-MM-ddTHH:mm:ss+08:00"));
        }
    }
}
