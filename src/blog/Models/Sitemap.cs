using System.Collections.Generic;
using System.Xml.Serialization;

namespace Laobian.Blog.Models
{
    [XmlRoot("urlset")]
    public class SiteMapUrlSet
    {
        [XmlElement("url")]
        public List<SiteMapUrl> Urls { get; set; }
    }

    public class SiteMapUrl
    {
        [XmlElement("loc")]
        public string Loc { get; set; }

        [XmlElement("lastmod")]
        public string LastMod { get; set; }

        [XmlElement("changefreq")]
        public string ChangeFreq { get; set; }

        [XmlElement("priority")]
        public double Priority { get; set; }
    }
}
