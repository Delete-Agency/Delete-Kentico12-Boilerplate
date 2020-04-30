using System.Linq;
using AutoMapper;
using DeleteBoilerplate.Common.Models.Media;
using DeleteBoilerplate.GenericComponents.Models.Widgets.ContactFormWidget;
using DeleteBoilerplate.GenericComponents.Models.Widgets.ContentBlockWidget;
using DeleteBoilerplate.GenericComponents.Models.Widgets.HeroWidget;
using DeleteBoilerplate.GenericComponents.Models.Widgets.ImageWidget;
using DeleteBoilerplate.GenericComponents.Models.Widgets.StaticHtmlWidget;
using DeleteBoilerplate.GenericComponents.Models.Widgets.TwitterWidget;
using DeleteBoilerplate.Infrastructure.Extensions;
using DeleteBoilerplate.Twitter.Extensions;
using LinqToTwitter;

namespace DeleteBoilerplate.GenericComponents
{
    public class GenericComponentsAutoMap : AutoMapper.Profile
    {
        public GenericComponentsAutoMap()
        {
            CreateMap<ContentBlockWidgetProperties, ContentBlockWidgetViewModel>();
            CreateMap<HeroWidgetProperties, HeroWidgetViewModel>();

            CreateMap<ImageWidgetProperties, ImageWidgetViewModel>()
                .ForMember(src => src.Images, opt => opt.Ignore())
                .AfterMap((s, d, context) =>
                {
                    if (s.Images != null)
                    {
                        d.Images = s.Images.Select(x=>
                        {
                            var model = x.GetImageModel();
                            if (model != null)
                            {
                                return context.Mapper.Map<ImageViewModel>(model);
                            }
                            return null;
                        }).Where(x=>x!=null).ToList();
                    }
                });

            CreateMap<StaticHtmlWidgetProperties, StaticHtmlWidgetViewModel>()
                .ForMember(dst => dst.Html, opt => opt.MapFrom(src => src.Html));

            CreateMap<ContactFormProperties, ContactFormWidgetViewModel>(MemberList.None)
                .ForMember(d => d.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(d => d.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(d => d.ButtonText, opt => opt.MapFrom(src => src.ButtonText));

            CreateMap<TwitterWidgetProperties, TwitterWidgetViewModel>(MemberList.None)
                .ForMember(d => d.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(d => d.ScreenName, opt => opt.MapFrom(src => src.ScreenName))
                .ForMember(d => d.TweetCount, opt => opt.MapFrom(src => src.TweetCount))
                .ForMember(d => d.Signet, opt => opt.Ignore())
                .ForMember(d => d.ProfileLink, opt => opt.Ignore())
                .ForMember(d => d.GetTweetsMainPartApi, opt => opt.Ignore())
                .ForMember(d => d.GetTweetsApi, opt => opt.Ignore());

            CreateMap<Status, TweetViewModel>(MemberList.None)
                .ForMember(d => d.Link, opt => opt.MapFrom(src => src.GetTweetLink()))
                .ForMember(d => d.Name, opt => opt.MapFrom(src => $"@{src.ScreenName}"))
                .ForMember(d => d.Text, opt => opt.MapFrom(src => src.FullText))
                .ForMember(d => d.Date, opt => opt.MapFrom(src => src.CreatedAt.ToString("dd MMMM")))
                .ForMember(d => d.ImageUrl, opt => opt.Ignore())
                .ForMember(d => d.HeightImage, opt => opt.Ignore())
                .ForMember(d => d.WidthImage, opt => opt.Ignore())
                .AfterMap((s, d, context) =>
                {
                    var mediaEntity = s.Entities.MediaEntities?.FirstOrDefault();
                    d.ImageUrl = mediaEntity?.MediaUrlHttps;
                    d.HeightImage = mediaEntity?.Sizes.LastOrDefault()?.Height;
                    d.WidthImage = mediaEntity?.Sizes.LastOrDefault()?.Width;
                });
        }
    }
}