using Laobian.Lib.Converter;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Laobian.Lib.Helper
{
    public static class JsonHelper
    {
        public static string Serialize<T>(T obj, bool writeIndented = false)
        {
            JsonSerializerOptions option = new()
            {
                WriteIndented = writeIndented,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            option.Converters.Add(new IsoDateTimeConverter());
            return JsonSerializer.Serialize(obj, option);
        }

        public static T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
