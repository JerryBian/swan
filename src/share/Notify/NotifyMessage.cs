using System;
using System.Collections.Generic;
using System.IO;

namespace Laobian.Share.Notify
{
    public class NotifyMessage
    {
        public string SendGridApiKey { get; set; }

        public string ToEmailAddress { get; set; }

        public string ToName { get; set; }

        public string Subject { get; set; }

        public string Content { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.Now;

        public LaobianSite Site { get; set; }

        public IDictionary<string, Stream> Attachments { get; } = new Dictionary<string, Stream>();
    }
}