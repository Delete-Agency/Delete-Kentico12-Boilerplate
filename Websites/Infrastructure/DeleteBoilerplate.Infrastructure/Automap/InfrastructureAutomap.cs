using DeleteBoilerplate.Common.Models.Media;

namespace DeleteBoilerplate.Infrastructure
{
    public class InfrastructureAutoMap : AutoMapper.Profile
    {
        public InfrastructureAutoMap()
        {
            CreateMap<ImageModel, ImageViewModel>();
        }
    }
}
