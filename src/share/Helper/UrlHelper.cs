namespace Laobian.Share.Helper
{
    public static class UrlHelper
    {
        public static string Combine(params string[] parts)
        {
            var url = string.Empty;
            foreach (var part in parts)
            {
                url += "/" + part.TrimStart('/', '\\');
            }

            return url;
        }
    }
}