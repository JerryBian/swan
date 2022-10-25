namespace Laobian.Lib.Model
{
    public class PostAccessItem
    {
        public PostAccessItem(string id, string ip)
        {
            Ip = ip;
            Timestamp = DateTime.Now;
        }

        public string Ip { get; init; }

        public DateTime Timestamp { get; init; }

        public string Id { get; init; }
    }
}
