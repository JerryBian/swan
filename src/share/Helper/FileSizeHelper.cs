namespace Laobian.Share.Helper
{
    /// <summary>
    /// Helps for dealing with file size
    /// </summary>
    public class FileSizeHelper
    {
        /// <summary>
        /// Display as friendly format
        /// </summary>
        /// <param name="bytes">The given size in bytes</param>
        /// <returns>Friendly display</returns>
        public static string Format(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            var order = 0;
            while (bytes >= 1024 && order < sizes.Length - 1)
            {
                order++;
                bytes = bytes / 1024;
            }

            var result = $"{bytes:0.##} {sizes[order]}";
            return result;
        }
    }
}
