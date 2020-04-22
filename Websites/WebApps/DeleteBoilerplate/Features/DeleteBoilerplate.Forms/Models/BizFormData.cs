using System.ComponentModel.DataAnnotations;

namespace DeleteBoilerplate.Forms.Models
{
    public class BizFormData : IFormData
    {
        public string FormName { get; set; }

        public string ElementId { get; set; }
    }
}