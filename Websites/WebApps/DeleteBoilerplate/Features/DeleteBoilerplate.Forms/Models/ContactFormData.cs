using System.ComponentModel.DataAnnotations;

namespace DeleteBoilerplate.Forms.Models
{
    public class ContactFormData : IFormData
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Telephone { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public bool? IsConsented { get; set; }
    }
}