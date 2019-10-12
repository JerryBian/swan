using System;

namespace Laobian.Share.Helper
{
    public class StringEqualsHelper
    {
        public static bool IgnoreCase(string left, string right)
        {
            return string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
        }
    }
}
