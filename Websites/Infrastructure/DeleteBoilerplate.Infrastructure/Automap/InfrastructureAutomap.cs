using DeleteBoilerplate.Common.Models.Media;

namespace DeleteBoilerplate.Infrastructure
{
    public class InfrastructureAutomap : AutoMapper.Profile
    {
        public InfrastructureAutomap()
        {
            CreateMap<ImageModel, ImageViewModel>();
        }
    }
}
