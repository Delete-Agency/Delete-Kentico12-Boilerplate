using System;

namespace DeleteBoilerplate.Common.Models
{
    public class UrlSelectorItem
    {
        public string ExternalUrl { get; set; }

        public Guid? NodeGuid { get; set; }

        public string NodeAliasPath { get; set; }

        public bool IsOpenInNewTab { get; set; }
    }
}