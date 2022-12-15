namespace Swan.Lib.Helper
{
    public static class StringHelper
    {
        public static string Random()
        {
            return "b" + Guid.NewGuid().ToString("N");
        }

        public static string Truncate(string val, int maxLength)
        {
            return string.IsNullOrEmpty(val) ? val : val[..Math.Min(val.Length, maxLength)];
        }

        public static bool EqualsIgoreCase(string left, string right)
        {
            return string.Equals(left, right, StringComparison.OrdinalIgnoreCase);
        }
    }
}
