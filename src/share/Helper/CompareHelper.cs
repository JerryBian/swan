using System;

namespace Laobian.Share.Helper
{
    public static class CompareHelper
    {
        public static bool IgnoreCase(string left, string right)
        {
            return string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
        }
    }
}