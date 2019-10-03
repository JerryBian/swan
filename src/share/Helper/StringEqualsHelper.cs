using System;

namespace Laobian.Share.Helper
{
    public class StringEqualsHelper
    {
        public static bool EqualsIgnoreCase(string left, string right)
        {
            return string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
        }
    }
}
