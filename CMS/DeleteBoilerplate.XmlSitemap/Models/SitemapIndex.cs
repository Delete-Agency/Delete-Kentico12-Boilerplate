using System.Xml.Serialization;
using DeleteBoilerplate.XmlSitemap.Constants;

namespace DeleteBoilerplate.XmlSitemap.Models
{
    [XmlRoot(ElementName = "sitemapindex", Namespace = SitemapConstants.XmlNamespaces.Sitemap)]
    public class SitemapIndex
    {
        [XmlElement(ElementName = "sitemap")]
        public SitemapIndexNode[] Nodes { get; set; }
    }
}
