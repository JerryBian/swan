using Swan.Core.Helper;

namespace Swan.Core.Extension
{
    public static class ObjectExtension
    {
        public static T DeepClone<T>(this T a)
        {
            string text = JsonHelper.Serialize(a);
            return JsonHelper.Deserialize<T>(text);
        }
    }
}
