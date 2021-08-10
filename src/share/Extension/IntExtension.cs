using System.Globalization;

namespace Laobian.Share.Extension
{
    public static class IntExtension
    {
        public static string ToUSThousand(this int number)
        {
            var nfi = new CultureInfo("en-US", false).NumberFormat;
            nfi.NumberDecimalDigits = 0;
            return number.ToString("N", nfi);
        }
    }
}