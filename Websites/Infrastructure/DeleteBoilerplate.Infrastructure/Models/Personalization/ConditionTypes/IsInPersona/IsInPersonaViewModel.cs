using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeleteBoilerplate.Infrastructure.Models.Personalization.ConditionTypes.IsInPersona
{
    public class IsInPersonaViewModel
    {
        [Required]
        [Display(Name = "Persona Code Name")]
        public string PersonaCodeName { get; set; }
        
        public List<IsInPersonaListItemViewModel> AllPersonas { get; set; }
    }
}