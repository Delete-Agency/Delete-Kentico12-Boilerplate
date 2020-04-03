using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DeleteBoilerplate.Account.Models
{
    public class RegisterViewModel : ConsentAgreementViewModel
    {
        [DisplayName("First Name")]
        [Required(ErrorMessage = "The First Name field is required")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        [Required(ErrorMessage = "The Last Name field is required")]
        public string LastName { get; set; }

        [DisplayName(nameof(Email))]
        [Required(ErrorMessage = "The Email field is required")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [DisplayName(nameof(Password))]
        [Required(ErrorMessage = "The Password field is required")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Password Again")]
        [Required(ErrorMessage = "The Password Again field is required")]
        [Compare("Password", ErrorMessage = "Your passwords did not match")]
        public string PasswordConfirmation { get; set; }
    }
}