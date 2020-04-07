using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DeleteBoilerplate.Account.Models
{
    public class ResetPasswordViewModel
    {
        [DataType(DataType.Password)]
        [DisplayName(nameof(Password))]
        [Required(ErrorMessage = "The Password field is required")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Confirm password")]
        [Required(ErrorMessage = "The Confirm password field is required")]
        public string PasswordConfirmation { get; set; }
    }
}