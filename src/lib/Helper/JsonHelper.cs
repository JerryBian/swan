using Laobian.Lib.Converter;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Laobian.Lib.Helper
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
                converters.ForEach(x => option.Converters.Add(x));
            }
            else
            {
                option.Converters.Add(new IsoDateTimeConverter());
            }

            return JsonSerializer.Serialize(obj, option);
        }

        public static T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
