using DeleteBoilerplate.Common.Models;
using DeleteBoilerplate.Domain.Repositories;
using LightInject;

namespace DeleteBoilerplate.Domain.Services
{
    public class UrlSelectorService : IUrlSelectorService
    {
        [Inject]
        protected ITreeNodeRepository TreeNodeRepository { get; set; }

        public LinkViewModel GetLink(UrlSelectorItem item)
        {
            if (item == null)
            {
                return new LinkViewModel();
            }
            
            var linkViewModel = new LinkViewModel { Target = item.IsOpenInNewTab ? "_blank" : "_self" };
            
            if (item.NodeGuid != null)
            {
                var node = this.TreeNodeRepository.GetByNodeGuid(item.NodeGuid.Value);
                linkViewModel.Url = node.RelativeURL;
            }
            else
            {
                linkViewModel.Url = item.ExternalUrl;
            }
            
            return linkViewModel;
        }
    }

    public interface IUrlSelectorService
    {
        LinkViewModel GetLink(UrlSelectorItem item);
    }
}