using System;
using DeleteBoilerplate.Common.Models.Media;

namespace DeleteBoilerplate.Projects.Models
{
    public class ProjectViewModel
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public int Year { get; set; }

        public string Url { get; set; }
        
        public ImageViewModel Image { get; set; }
    }
}