using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Laobian.Share.Converter;

namespace Laobian.Share.Util
{
    public static class JsonUtil
    {
        public static string Serialize<T>(T obj, bool writeIndented = false, List<JsonConverter> converters = null)
        {
            var option = new JsonSerializerOptions
            {
                WriteIndented = writeIndented,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            if (converters == null)
            {
                option.Converters.Add(new IsoDateTimeConverter());
            }
            else
            {
                foreach (var jsonConverter in converters)
                {
                    option.Converters.Add(jsonConverter);
                }
            }

            return JsonSerializer.Serialize(obj, option);
        }

        public static T Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }

        public static async Task<T> DeserializeAsync<T>(Stream stream)
        {
            return await JsonSerializer.DeserializeAsync<T>(stream);
        }
    }
}