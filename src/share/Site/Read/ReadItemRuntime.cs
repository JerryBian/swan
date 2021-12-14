using System.Runtime.Serialization;
using Markdig;

namespace Laobian.Share.Site.Read
{
    [DataContract]
    public class ReadItemRuntime
    {
        public ReadItemRuntime(){}

        public ReadItemRuntime(ReadItem raw)
        {
            Raw = raw;
        }

        [DataMember(Order = 1)]
        public ReadItem Raw { get; set; }

        [DataMember(Order = 2)]
        public string ShortCommentHtml { get; set; }

        [IgnoreDataMember]
        public string BlogPostTitle { get; set; }

        public void ExtractRuntimeData()
        {
            if (!string.IsNullOrEmpty(Raw.ShortComment))
            {
                ShortCommentHtml = Markdown.ToHtml(Raw.ShortComment);
            }
        }
    }
}
