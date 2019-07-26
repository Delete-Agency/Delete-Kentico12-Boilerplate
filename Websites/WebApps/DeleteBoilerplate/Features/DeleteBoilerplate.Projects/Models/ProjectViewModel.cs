using System;

namespace DeleteBoilerplate.Projects.Models
{
    public class ProjectViewModel
    {
        public Guid Id { get; set; }

        public string Description { get; set; }

        public int Year { get; set; }
    }
}