using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.DocumentEngine.Types.DeleteBoilerplate
{
    public partial class NavigationLink
    {
        public string AssociatedPagePath { get; set; }

        public IEnumerable<NavigationLink> ChildLinks { get; set; }

        public NavigationLink ParentLink { get; set; }
    }
}
