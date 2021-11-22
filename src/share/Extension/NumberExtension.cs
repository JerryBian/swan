using System.Globalization;

namespace Laobian.Share.Extension;

public static class NumberExtension
{
    public static string ToHuman(this int number)
    {
        if (number < 1000)
        {
            return number.ToString();
        }

        if (number < 10000)
        {
            return $"{number / 1000:F1}k";
        }

        return $"{number / 10000:F1}w";
    }

    public static string ToThousandHuman(this int number)
    {
        return number.ToString("N0", new CultureInfo("en-US"));
    }
}