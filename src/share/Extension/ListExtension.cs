using System.Collections.Generic;
using System.Linq;

namespace Laobian.Share.Extension
{
    /// <summary>
    ///     Extensions for <see cref="List{T}" />
    /// </summary>
    public static class ListExtension
    {
        /// <summary>
        ///     Split to equal-sized groups, return for current page only
        /// </summary>
        /// <typeparam name="T">Type of instance</typeparam>
        /// <param name="source">The given source</param>
        /// <param name="chunkSize">Size of chunk</param>
        /// <param name="page">Current page. The first page is 1, not 0.</param>
        /// <returns>Collection for current page</returns>
        public static List<T> ToPaged<T>(this List<T> source, int chunkSize, int page)
        {
            if (page <= 0 || page > source.Count) return source; // if page is out of range, simply return whole source

            return source.Skip(chunkSize * (page - 1)).Take(chunkSize).ToList();
        }
    }
}