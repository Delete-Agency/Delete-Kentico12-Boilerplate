using System;
using System.Linq;
using CMS.Base.Web.UI;
using CMS.DocumentEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using DeleteBoilerplate.Domain.Services;


public partial class CMSFormControls_SeoUrlSelector : TextBoxControl
{
    protected ISeoUrlService SeoUrlService = new SeoUrlService();

    protected override CMSTextBox TextBox => txtText;

    public override bool IsValid()
    {
        var seoUrl = ValidationHelper.GetString(Value, String.Empty);

        // If URL is not entered and will be copied from NodeAliasPath automatically in custom module
        if (!(Data is TreeNode currentDocument) || string.IsNullOrWhiteSpace(seoUrl)) return base.IsValid();

        var foundNodes = SeoUrlService.GetAllDocumentsBySeoUrl(seoUrl);

        if (foundNodes.Count == 0 || foundNodes.All(x => x.DocumentID == currentDocument.DocumentID))
            return base.IsValid();

        ValidationError =
            $"URL '{seoUrl}' is in conflict with another URL used for the page '{foundNodes.FirstOrDefault()?.DocumentNamePath ?? String.Empty}' ({foundNodes.FirstOrDefault()?.DocumentCulture ?? String.Empty}) page.";
        return false;
    }
}