namespace Laobian.Share.Extension
{
    /// <summary>
    /// Extensions for <see cref="int"/>
    /// </summary>
    public static class IntExtension
    {
        /// <summary>
        /// Display as thousand separated format, e.g 10,000
        /// </summary>
        /// <param name="number">The given number</param>
        /// <returns>Thousand separated format</returns>
        public static string ToThousandsPlace(this int number)
        {
            return number.ToString("N0");
        }
    }
}
