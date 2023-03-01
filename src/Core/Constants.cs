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
            public const string BasePath = "asset";

            public const string BlogPostPath = "blog/post";

            public const string ReadPath = "read";

            public const string BlogTagPath = "blog";

            public const string BlogSeriesPath = "series";

            public const string LogPath = "temp/log";

            public const string FilePath = "file";
        }

        public class Misc
        {
            public const string RouterFile = "file";

            public const string CacheProfileClientShort = "cache0";

            public const string CacheProfileServerShort = "cache1";

            public const string CacheProfileServerLong = "cache2";

            public const string JsonFileFilter = "*.json";

            public const string JsonFileExt = ".json";

            public const string LogFileFilter = "*.log";

            public const string LogFileExt = ".log";
        }
    }
}
