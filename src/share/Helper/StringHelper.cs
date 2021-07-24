using System;

namespace Laobian.Share.Helper
{
    public static class StringHelper
    {
        public static bool EqualIgnoreCase(string left, string right)
        {
            return string.Equals(left, right, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}