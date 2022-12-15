namespace Swan.Lib.Log
{
    public class SwanLog
    {
        public string Message { get; set; }

        public string Exception { get; set; }

        public DateTime Timestamp { get; set; }

        public LogLevel Level { get; set; }
    }
}
