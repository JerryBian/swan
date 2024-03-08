using System.Text;

namespace Swan.Core.Helper
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

        public static string Underscored(string str)
        {
            var builder = new StringBuilder();

            for (var i = 0; i < str.Length; ++i)
            {
                if (ShouldUnderscore(i, str))
                {
                    builder.Append('_');
                }

                builder.Append(char.ToLowerInvariant(str[i]));
            }

            return builder.ToString();

            static bool ShouldUnderscore(int i, string s)
            {
                if (i == 0 || i >= s.Length || s[i] == '_') return false;

                var curr = s[i];
                var prev = s[i - 1];
                var next = i < s.Length - 2 ? s[i + 1] : '_';

                return prev != '_' && ((char.IsUpper(curr) && (char.IsLower(prev) || char.IsLower(next))) ||
                    (char.IsNumber(curr) && (!char.IsNumber(prev))));
            }
        }
    }
}
