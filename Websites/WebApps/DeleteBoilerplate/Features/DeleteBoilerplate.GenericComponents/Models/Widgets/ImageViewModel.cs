using System;

namespace DeleteBoilerplate.GenericComponents.Models.Widgets
{
    public class ImageViewModel
    {
        public string Url { get; set; }
        public string Title { get; set; }

        public string FileName { get; set; }
        public string FileExtension { get; set; }

        public Guid Id { get; set; }

        public DateTime UploadDate { get; set; }
    }
}