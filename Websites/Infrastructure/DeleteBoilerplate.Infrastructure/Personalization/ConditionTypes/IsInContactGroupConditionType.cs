using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CMS.ContactManagement;
using DeleteBoilerplate.Infrastructure.Personalization.ConditionTypes;
using Kentico.Forms.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc.Personalization;

[assembly: RegisterPersonalizationConditionType("DeleteBoilerplate.Personalization.IsInContactGroup",
    typeof(IsInContactGroupConditionType),
    "{$DeleteBoilerplate.ConditionType.IsInContactGroup.Name$}",
    Description = "{$DeleteBoilerplate.ConditionType.IsInContactGroup.Description$}",
    IconClass = "icon-app-contact-groups",
    Hint = "{$DeleteBoilerplate.ConditionType.IsInContactGroup.Hint$}")]

namespace DeleteBoilerplate.Infrastructure.Personalization.ConditionTypes
{
    /// <summary>
    /// Personalization condition type based on contact group.
    /// </summary>
    public class IsInContactGroupConditionType : ConditionType
    {
        /// <summary>
        /// List of selected contact group code names.
        /// </summary>
        [EditingComponent("DeleteBoilerplate.ContactGroupSelector", Order = 0, Label = "")]
        public List<string> SelectedContactGroups { get; set; }
        
        /// <summary>
        /// Variant name.
        /// </summary>
        [EditingComponent(TextInputComponent.IDENTIFIER, Order = 1, Label = "")]
        [EditingComponentProperty(nameof(TextInputProperties.Placeholder), "{$kentico.pagebuilder.variant.name$}")]
        [Required(ErrorMessage = "kentico.pagebuilder.variant.name.required")]
        public override string VariantName { get; set; }
        
        /// <summary>
        /// Evaluate condition type.
        /// </summary>
        /// <returns>Returns <c>true</c> if implemented condition is met.</returns>
        public override bool Evaluate()
        {
            var contact = ContactManagementContext.GetCurrentContact();
            if (contact == null)
            {
                return false;
            }

            if (SelectedContactGroups == null || SelectedContactGroups.Count == 0)
            {
                return contact.ContactGroups.Count == 0;
            }

            return contact.IsInAnyContactGroup(SelectedContactGroups.ToArray());
        }
    }
}