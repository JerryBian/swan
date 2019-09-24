using System;

namespace Laobian.Share.Extension
{
    /// <summary>
    /// Extensions for <see cref="Guid"/>
    /// </summary>
    public static class GuidExtension
    {
        /// <summary>
        /// Display as normal format, this contains only ASCII characters
        /// </summary>
        /// <param name="id">The given <see cref="Guid"/></param>
        /// <returns>Normal format</returns>
        public static string Normal(this Guid id)
        {
            return $"{id:N}";
        }
    }
}
