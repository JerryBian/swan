using System;

namespace Laobian.Share.Util;

public static class StringUtil
{
    public static bool EqualsIgnoreCase(string left, string right)
    {
        return string.Equals(left, right, StringComparison.InvariantCultureIgnoreCase);
    }

    public static string GenerateRandom()
    {
        return Guid.NewGuid().ToString("N");
    }

    public static string Truncate(string val, int maxLength)
    {
        if (string.IsNullOrEmpty(val))
        {
            return val;
        }

        return val[..Math.Min(val.Length, maxLength)];
    }
}