using System;
using System.Text;

namespace Laobian.Share.Extension
{
    /// <summary>
    /// Extensions for <see cref="string"/>
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// Compare two strings ignore case
        /// </summary>
        /// <param name="left">The left string</param>
        /// <param name="right">The right string</param>
        /// <returns>True if equals, otherwise false</returns>
        public static bool EqualsIgnoreCase(this string left, string right)
        {
            return left.Equals(right, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Encode string to Base64 format
        /// </summary>
        /// <param name="input">The given string</param>
        /// <returns>Base64 format string</returns>
        public static string EncodeAsBase64(this string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Decode to string from Base64 format
        /// </summary>
        /// <param name="base64">The given Base64 format string</param>
        /// <returns>Decoded string</returns>
        public static string DecodeFromBase64(this string base64)
        {
            var bytes = Convert.FromBase64String(base64);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
