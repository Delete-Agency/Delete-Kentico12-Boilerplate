using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeleteBoilerplate.Common.Models;
using DeleteBoilerplate.Infrastructure.Models;

namespace DeleteBoilerplate.GenericComponents.Models.InlineEditors
{
    public class SelectEditorViewModel : BaseInlineEditorViewModel
    {
        public string SelectedValue { get; set; }

        public IList<OptionModel> Options { get; set; }
    }
}