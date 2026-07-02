namespace Swan.Web.Models
{
    public class StaticFileItem
    {
        public string PhysicalPath { get; set; }
        public string RelativePath { get; set; }
        public long Size { get; set; }
        public DateTime LastModified { get; set; }

        /// <summary>First path segment under /static/, e.g. "file" or "img".</summary>
        public string PathType { get; set; }

        /// <summary>Year parsed from the URL path, e.g. "2026".</summary>
        public string PathYear { get; set; }

        public string SizeFormatted => Size switch
        {
            >= 1024 * 1024 => $"{Size / (1024.0 * 1024.0):F1} MB",
            >= 1024 => $"{Size / 1024.0:F1} KB",
            _ => $"{Size} B"
        };

        public string Extension => Path.GetExtension(RelativePath).ToLowerInvariant();
    }
}
