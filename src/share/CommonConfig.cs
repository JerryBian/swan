namespace Laobian.Share
{
    public abstract class CommonConfig
    {
        public string CommandLineApp { get; set; }

        public string CommandLineBeginArg { get; set; }

        public string BlogLocalEndpoint { get; set; }

        public string BlogRemoteEndpoint { get; set; }

        public string ApiLocalEndpoint { get; set; }

        public string AdminLocalEndpoint { get; set; }

        public string AdminEmail { get; set; }

        public string BlogPostLocation { get; set; }

        public string FileServerBaseUrl { get; set; }

        public string AppVersion { get; set; }
    }
}