using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DeleteBoilerplate.Account.Models
{
    public class LoginViewModel
    {
        [DisplayName(nameof(Email))]
        [Required(ErrorMessage = "The Email field is required")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [DisplayName(nameof(Password))]
        [Required(ErrorMessage = "The Password field is required")]
        public string Password { get; set; }
    }
}