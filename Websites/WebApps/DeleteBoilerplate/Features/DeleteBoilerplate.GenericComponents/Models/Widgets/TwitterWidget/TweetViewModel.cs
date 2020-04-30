using System;

namespace DeleteBoilerplate.GenericComponents.Models.Widgets.TwitterWidget
{
    public class TweetViewModel
    {
        public string Link { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public string Date { get; set; }

        public string ImageUrl { get; set; }

        public int? HeightImage { get; set; }

        public int? WidthImage { get; set; }
    }
}