using System.Xml.Serialization;
using DeleteBoilerplate.XmlSitemap.Constants;

namespace DeleteBoilerplate.XmlSitemap.Models
{
    [XmlRoot(ElementName = "urlset", Namespace = SitemapConstants.XmlNamespaces.Sitemap)]
    public class Sitemap
    {
        [XmlElement(ElementName = "url")]
        public SitemapNode[] Nodes { get; set; }
    }
}
