using Swan.Core.Model.Object;

namespace Swan.Core.Model
{
    public class SwanLog
    {
        public SwanLog(LogObject obj)
        {
            Raw = obj;
        }

        public LogObject Raw { get; init; }
    }
}
