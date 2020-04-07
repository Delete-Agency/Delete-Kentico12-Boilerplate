using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DeleteBoilerplate.Account.Models
{
    public class UpdateUserDetailsViewModel
    {
        [DisplayName("First Name")]
        [Required(ErrorMessage = "The First Name field is required")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        [Required(ErrorMessage = "The Last Name field is required")]
        public string LastName { get; set; }

        [DisplayName(nameof(Email))]
        [Required(ErrorMessage = "The Email field is required")]
        public string Email { get; set; }
    }
}