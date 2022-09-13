namespace Laobian.Lib.Helper
{
    public static class StringHelper
    {
        public static string Random()
        {
            return "b" + Guid.NewGuid().ToString("N");
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
}
