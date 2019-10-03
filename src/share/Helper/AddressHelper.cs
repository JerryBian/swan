using System.Text;

namespace Laobian.Share.Helper
{
    public class AddressHelper
    {
        public static string GetAddress(string baseAddress, bool endsWithSlash = true, params string[] parts)
        {
            var address = GetAddress(endsWithSlash, parts);
            return $"{baseAddress.Trim('/', ' ')}{address}";
        }

        public static string GetAddress(bool endsWithSlash = true, params string[] parts)
        {
            var sb = new StringBuilder();
            foreach (var part in parts)
            {
                sb.AppendFormat("/{0}", part.Trim('/', ' '));
            }

            if (endsWithSlash)
            {
                sb.Append("/");
            }

            return sb.ToString();
        }
    }
}
