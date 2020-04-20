using System.Collections.Generic;
using CMS.DataEngine;
using Kentico.Forms.Web.Mvc;

namespace DeleteBoilerplate.Infrastructure.Models.FormComponents.ContactGroupSelector
{
    public class ContactGroupSelectorProperties : FormComponentProperties<List<string>>
    {
        public ContactGroupSelectorProperties() : base(FieldDataType.Unknown)
        {
        }

        public override List<string> DefaultValue { get; set; }
    }
}