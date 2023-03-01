namespace Swan.Core.Model
{
    public class SiteAccess
    {
        public SiteAccess(SiteArea area, string ipAddress)
        {
            Area = area;
            IpAddress = ipAddress;
            Timestamp = DateTime.Now;
        }

        public SiteArea Area { get; init; }

        public string IpAddress { get; init; }

        public DateTime Timestamp { get; init; }
    }
}
