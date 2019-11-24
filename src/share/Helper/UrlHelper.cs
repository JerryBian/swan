namespace Laobian.Share.Helper
{
    public static class UrlHelper
    {
        public static string Combine(string baseAddress, params string[] parts)
        {
            var url = baseAddress.Trim('/', '\\');
            foreach (var part in parts)
            {
                url += "/" + part.TrimStart('/', '\\');
            }

            return url;
        }
    }
}