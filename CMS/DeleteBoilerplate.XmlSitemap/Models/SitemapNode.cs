using System;
using System.Xml.Serialization;
using DeleteBoilerplate.XmlSitemap.Constants;

namespace DeleteBoilerplate.XmlSitemap.Models
{
    [XmlRoot(ElementName = "url", Namespace = SitemapConstants.XmlNamespaces.Sitemap)]
    public class SitemapNode
    {
        [XmlElement(ElementName = "loc")]
        public string Url { get; set; }

        [XmlIgnore]
        public DateTime ModifyDate { get; set; }

        // Format: 1997-07-16T19:20:30.45Z
        [XmlElement(ElementName = "lastmod")]
        public string ModifyDateString
        {
            get { return this.ModifyDate.ToUniversalTime().ToString("yyyy-MM-dd'T'hh:mm:ss.ff'Z'"); }
            set { this.ModifyDate = DateTime.Parse(value); }
        }
        
        [XmlElement(ElementName = "priority")]
        public decimal? Priority { get; set; }

        public bool ShouldSerializePriority()
        {
            return Priority.HasValue;
        }
    }
}
