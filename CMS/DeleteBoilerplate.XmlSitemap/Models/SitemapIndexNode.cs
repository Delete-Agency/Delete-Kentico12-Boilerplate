using System;
using System.Xml.Serialization;
using DeleteBoilerplate.XmlSitemap.Constants;

namespace DeleteBoilerplate.XmlSitemap.Models
{
    [XmlRoot(ElementName = "sitemap", Namespace = SitemapConstants.XmlNamespaces.Sitemap)]
    public class SitemapIndexNode
    {
        [XmlElement(ElementName = "loc")]
        public string Url { get; set; }

        [XmlElement(ElementName = "lastmod")]
        public DateTime ModifyDate { get; set; }
    }
}
