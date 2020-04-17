using System;
using CMS.ContactManagement;
using CMS.Personas;
using DeleteBoilerplate.Infrastructure.Controllers.Personalization.ConditionTypes;
using DeleteBoilerplate.Infrastructure.Personalization.ConditionTypes;
using Kentico.PageBuilder.Web.Mvc.Personalization;
using Newtonsoft.Json;

[assembly: RegisterPersonalizationConditionType("DeleteBoilerplate.Personalization.IsInPersona",
    typeof(IsInPersonaConditionType), "{$DeleteBoilerplate.ConditionType.IsInPersona.Name$}",
    ControllerType = typeof(IsInPersonaController),
    Description = "{$DeleteBoilerplate.ConditionType.IsInPersona.Description$}",
    Hint = "{$DeleteBoilerplate.ConditionType.IsInPersona.Hint$}",
    IconClass = "icon-app-personas")]

namespace DeleteBoilerplate.Infrastructure.Personalization.ConditionTypes
{
    /// <summary>
    /// Personalization condition type based on persona.
    /// </summary>
    public class IsInPersonaConditionType : ConditionType
    {
        /// <summary>
        /// Selected persona code name.
        /// </summary>
        public string PersonaName { get; set; }

        /// <summary>
        /// Automatically populated variant name.
        /// </summary>
        public override string VariantName
        {
            get
            {
                return Persona?.PersonaDisplayName;
            }
            set
            {
                // No need to set automatically generated variant name
            }
        }

        private readonly Lazy<PersonaInfo> _persona;
        
        [JsonIgnore]
        private PersonaInfo Persona => _persona.Value;

        /// <summary>
        /// Creates an empty instance of <see cref="IsInPersonaConditionType"/> class.
        /// </summary>
        public IsInPersonaConditionType()
        {
            _persona = new Lazy<PersonaInfo>(() => PersonaInfoProvider.GetPersonaInfoByCodeName(PersonaName));
        }
        
        /// <summary>
        /// Evaluate condition type.
        /// </summary>
        /// <returns>Returns <c>true</c> if implemented condition is met.</returns>
        public override bool Evaluate()
        {
            if (Persona == null)
            {
                return false;
            }

            var contact = ContactManagementContext.GetCurrentContact(false);

            return contact?.ContactPersonaID == Persona.PersonaID;
        }
    }
}