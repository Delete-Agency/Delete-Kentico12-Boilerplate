using CMS.FormEngine.Web.UI;
using System;
using System.Collections.Generic;
using CMS.Base.Web.UI;
using CMS.Helpers;
using DeleteBoilerplate.Domain.Repositories;
using Newtonsoft.Json;
using DeleteBoilerplate.Domain.Services;

public partial class CMSFormControls_TreeTaxonomyMultiSelector : FormEngineUserControl
{
    public string TargetTaxonomyType => ValidationHelper.GetString(GetValue("TargetTaxonomyType"), string.Empty);

    protected string TaxonomyTree { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterScriptFile(Page, "~/CMSScripts/Laureus/jquery/jquery-3.3.1.min.js");
        ScriptHelper.RegisterScriptFile(Page, "~/CMSScripts/Laureus/jsTree/jstree.min.js");
        CssRegistration.RegisterCssLink(Page, "~/CMSScripts/Laureus/jsTree/themes/default/style.min.css");

        var taxonomyService = new TaxonomyService(new TaxonomyRepository());
        TaxonomyTree = JsonConvert.SerializeObject(taxonomyService.GetTaxonomyTree(TargetTaxonomyType));
    }

    public override object Value
    {
        get => !string.IsNullOrWhiteSpace(hfSelectedTaxonomies.Value)
                   ? string.Join(" ", JsonConvert.DeserializeObject<IEnumerable<string>>(hfSelectedTaxonomies.Value))
                   : String.Empty;
        set => hfSelectedTaxonomies.Value = value != null 
                                                ? JsonConvert.SerializeObject(value.ToString().Split(' '))
                                                : String.Empty;
    }

}