using Swan.Core.Helper;
using System.Runtime.Serialization.Formatters.Binary;

namespace Swan.Core.Extension
{
    public static class ObjectExtension
    {
        public static T DeepClone<T>(this T a)
        {
            var text = JsonHelper.Serialize(a);
            return JsonHelper.Deserialize<T>(text);
        }
    }
}
