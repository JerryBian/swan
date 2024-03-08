namespace Swan.Core.Model2
{
    public class SwanLog : SwanObject<SwanLog>
    {
        public string Url { get; set; }

        public string IpAddress { get; set; }

        public string UserAgent { get; set; }

        public string Message { get; set; }

        public string Exception { get; set; }

        public string Level { get; set; }
    }
}
