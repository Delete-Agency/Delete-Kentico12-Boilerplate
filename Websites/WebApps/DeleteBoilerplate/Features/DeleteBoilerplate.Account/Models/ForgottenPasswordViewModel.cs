using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DeleteBoilerplate.Account.Models
{
    public class ForgottenPasswordViewModel
    {
        [DisplayName(nameof(Email))]
        [Required(ErrorMessage = "The Email field is required")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        public string Email { get; set; }
    }
}