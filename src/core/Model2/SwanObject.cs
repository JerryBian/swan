using System.Reflection;
using System.Text.Json.Serialization;

namespace Swan.Core.Model2
{
    public abstract class SwanObject<T> : ISwanObject
    {
        private static readonly Lazy<string> _objectName = new Lazy<string>(() =>
        {
            return typeof(T).Name.Replace("swan", string.Empty, StringComparison.OrdinalIgnoreCase).ToLowerInvariant();
        }, true);

        private static readonly Lazy<List<string>> _properties = new Lazy<List<string>>(() =>
        {
            var props = new List<string>();
            foreach(var property in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var attr = property.GetCustomAttribute<JsonPropertyNameAttribute>();
                if(attr != null)
                {
                    props.Add(property.Name);
                }
            }

            return props;
        }, true);

        public static string ObjectName => _objectName.Value;

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("last_modified_at")]
        public DateTime LastModifiedAt { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }

        public static List<string> GetObjectProperties()
        {
            return _properties.Value;
        }
    }
}
