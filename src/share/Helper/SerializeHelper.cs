using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Xml.Serialization;

namespace Laobian.Share.Helper
{
    /// <summary>
    ///     Helpers related to serialization
    /// </summary>
    public class SerializeHelper
    {
        /// <summary>
        ///     Serialize object to JSON string
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="obj">The given object</param>
        /// <param name="indented">Indicates whether JSON should be intended, true means yes, otherwise no.</param>
        /// <returns>JSON string</returns>
        public static string ToJson<T>(T obj, bool indented = false)
        {
            return JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = indented, Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) });
        }

        /// <summary>
        ///     Deserialize to object from JSON string
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="json">The given string</param>
        /// <returns>Deserialized object</returns>
        public static T FromJson<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions { Encoder = JavaScriptEncoder.Create(UnicodeRanges.All) });
        }

        public static string ToXml<T>(T obj, string nsPrefix = "", string ns = "")
        {
            using (var sw = new Utf8StringWriter())
            {
                var xmlNamespaces = new XmlSerializerNamespaces();
                xmlNamespaces.Add(nsPrefix, ns);
                var serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(sw, obj, xmlNamespaces);
                return sw.ToString();
            }
        }
    }
}