using System.IO;
using System.Text.Json;
using System.Xml.Serialization;
using ProtoBuf.Meta;

namespace Laobian.Share.Helper
{
    /// <summary>
    /// Helpers related to serialization
    /// </summary>
    public class SerializeHelper
    {
        private static readonly RuntimeTypeModel Serializer; // custom ProtoBuf instance

        static SerializeHelper()
        {
            Serializer = TypeModel.Create();
            Serializer.UseImplicitZeroDefaults = false;
        }

        /// <summary>
        /// Serialize object to JSON string
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="obj">The given object</param>
        /// <param name="indented">Indicates whether JSON should be intended, true means yes, otherwise no.</param>
        /// <returns>JSON string</returns>
        public static string ToJson<T>(T obj, bool indented = false)
        {
            return JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = indented });
        }

        /// <summary>
        /// Deserialize to object from JSON string
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="json">The given string</param>
        /// <returns>Deserialized object</returns>
        public static T FromJson<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json);
        }

        /// <summary>
        /// Serialize object to Google ProtoBuf binary
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="obj">The given object</param>
        /// <returns>ProtoBuf binary stream</returns>
        public static Stream ToProtoBuf<T>(T obj)
        {
            var ms = new MemoryStream();
            Serializer.Serialize(ms, obj);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        /// <summary>
        /// Deserialize to object from Google ProtoBuf binary
        /// </summary>
        /// <typeparam name="T">Type of object</typeparam>
        /// <param name="stream">The given ProtoBuf binary stream</param>
        /// <returns>Deserialized object</returns>
        public static T FromProtoBuf<T>(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            return (T)Serializer.Deserialize(stream, null, typeof(T));
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
