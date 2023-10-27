using Swan.Core.Converter;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Swan.Core.Helper
{
    public static class JsonHelper
    {
        public static string Serialize<T>(T obj, bool writeIndented = false, List<JsonConverter> converters = null)
        {
            JsonSerializerOptions option = new()
            {
                WriteIndented = writeIndented,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            if (converters != null)
            {
                converters.ForEach(option.Converters.Add);
            }
            else
            {
                option.Converters.Add(new IsoDateTimeConverter());
            }

            return JsonSerializer.Serialize(obj, option);
        }

        public static T Deserialize<T>(string json)
        {
            return string.IsNullOrEmpty(json) ? default : JsonSerializer.Deserialize<T>(json);
        }
    }
}
