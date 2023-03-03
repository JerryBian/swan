namespace Swan.Core
{
    public class Constants
    {
        public class CacheKey
        {
            public const string MemoryObjects = "_memory_objects_";

            public const string MemoryObjectsAdmin = "_memory_objects_admin";
        }

        public class Asset
        {
            public const string BaseDir = "asset";

            public const string BlogPostDir = "blog/post";

            public const string ReadDir = "read";

            public const string BlogTagDir = "blog";

            public const string BlogTagFile = "tag.json";

            public const string BlogSeriesDir = "blog";

            public const string BlogSeriesFile = "series.json";

            public const string LogDir = "";

            public const string LogFile = "log.json";

            public const string FileDir = "file";

            public const string BlogPostAccessDir = "blog";

            public const string BlogPostAccessFile = "post_access.json";
        }

        public class Misc
        {
            public const string RouterFile = "file";

            public const string CacheProfileClientShort = "cache0";

            public const string CacheProfileServerShort = "cache1";

            public const string CacheProfileServerLong = "cache2";

            public const string JsonFileFilter = "*.json";

            public const string JsonFileExt = ".json";
        }
    }
}
