using System;
using System.ComponentModel.DataAnnotations;
using DeleteBoilerplate.Common.Models;
using DeleteBoilerplate.Infrastructure.Models.FormComponents.UrlSelector;
using Kentico.Forms.Web.Mvc;

[assembly: RegisterFormComponent(UrlSelectorComponent.Identifier, typeof(UrlSelectorComponent), "URL selector",
        IsAvailableInFormBuilderEditor = false, ViewName = "FormComponents/_UrlSelectorComponent", IconClass = "icon-chain")]

namespace DeleteBoilerplate.Infrastructure.Models.FormComponents.UrlSelector
{
    public class UrlSelectorComponent : FormComponent<UrlSelectorProperties, UrlSelectorItem>
    {
        public const string Identifier = "UrlSelector";

        public override void LoadProperties(FormComponentProperties properties)
        {
            base.LoadProperties(properties);
            RootPath = (properties as UrlSelectorProperties)?.RootPath ?? "/";
        }

        public override UrlSelectorItem GetValue()
        {
            return new UrlSelectorItem
            {
                ExternalUrl = ExternalUrl,
                NodeAliasPath = NodeAliasPath,
                NodeGuid = NodeGuid,
                IsOpenInNewTab = IsOpenInNewTab
            };
        }

        public override void SetValue(UrlSelectorItem value)
        {
            if (value == null) return;

            ExternalUrl = value.ExternalUrl;
            IsOpenInNewTab = value.IsOpenInNewTab;
            UseExternalUrl = !string.IsNullOrWhiteSpace(value.ExternalUrl);
            NodeGuid = value.NodeGuid;
            NodeAliasPath = value.NodeAliasPath;
        }


        [BindableProperty]
        public string ExternalUrl { get; set; }

        [BindableProperty]
        public string NodeAliasPath { get; set; }

        [BindableProperty]
        [Display(Name = "Use external URL")]
        public bool UseExternalUrl { get; set; }

        [BindableProperty]
        public Guid? NodeGuid { get; set; }

        [BindableProperty]
        [Display(Name = "Open in new tab")]
        public bool IsOpenInNewTab { get; set; }

        public string RootPath { get; set; }

        public string GlobalId { get; set; } = Guid.NewGuid().ToString();
    }
}