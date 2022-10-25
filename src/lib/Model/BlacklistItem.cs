namespace Laobian.Lib.Model
{
    public class BlacklistItem
    {
        public string Ip { get; set; }

        public DateTime CreateAt { get; set; }

        public DateTime LastUpdateAt { get; set; }

        public string Reason { get; set; }

        public DateTime InvalidTo { get; set; }
    }
}
