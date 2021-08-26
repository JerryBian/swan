namespace Laobian.Share.Extension
{
    public static class IntExtension
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
    }
}