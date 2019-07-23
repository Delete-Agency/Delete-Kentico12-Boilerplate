﻿using System;

using CMS.Base.Web.UI;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Localization;
using CMS.UIControls;


[Title("Content.NewTitle")]
public partial class CMSModules_Content_CMSDesk_New_New : CMSContentPage
{
    #region "Variables"

    CMSAbstractNewDocumentControl ctrl;

    private const string CONTENT_FOLDER = "~/CMSModules/Content/";
    private const string CONTENT_CMSDESK_FOLDER = CONTENT_FOLDER + "CMSDesk/";

    #endregion


    #region "Page events"

    protected override void CreateChildControls()
    {
        ctrl = LoadUserControl(CONTENT_FOLDER + "Controls/DocTypeSelection.ascx") as CMSAbstractNewDocumentControl;

        if (ctrl == null)
        {
            throw new Exception("Page type selector does not exist.");
        }

        ctrl.ID = "dt";
        ctrl.HeadingLevel = 3;
        ctrl.RedirectWhenNoChoice = true;
        plc.Controls.Add(ctrl);

        base.CreateChildControls();
    }


    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        DocumentManager.RegisterSaveChangesScript = false;
    }


    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        string parentCulture = QueryHelper.GetString("parentculture", LocalizationContext.PreferredCultureCode);
        bool isCultureAllowed = CheckPreferredCulture(parentCulture);
        if (!isCultureAllowed)
        {
            ShowWarning(GetString("licensevalidation.newdocumentcultureversion"));
            plc.Visible = false;
            return;
        }

        EnsureChildControls();

        ctrl.SelectionUrl = URLHelper.AppendQuery(ResolveUrl(CONTENT_CMSDESK_FOLDER + "Edit/Edit.aspx"), RequestContext.CurrentQueryString);

        // Create new document by default
        if (string.IsNullOrEmpty(QueryHelper.GetString("action", null)))
        {
            ctrl.SelectionUrl = URLHelper.AddParameterToUrl(ctrl.SelectionUrl, "action", "new");
        }

        if (ModuleEntryManager.IsModuleLoaded(ModuleName.ECOMMERCE))
        {
            string url = "~/CMSModules/Ecommerce/Pages/Tools/Products/Product_New.aspx";
            url = URLHelper.AddParameterToUrl(url, "content", "1");
            ctrl.ProductSelectionUrl = URLHelper.AppendQuery(ResolveUrl(url), RequestContext.CurrentQueryString);
        }
        ctrl.NewVariantUrl = URLHelper.AppendQuery(ResolveUrl("~/CMSModules/OnlineMarketing/Pages/Content/ABTesting/ABVariant/NewPage.aspx"), RequestContext.CurrentQueryString);

        // Current Node ID
        ctrl.ParentNodeID = QueryHelper.GetInteger("parentnodeid", 0);
        ctrl.ParentCulture = parentCulture;

        if (QueryHelper.GetBoolean("hidecontentonly", false))
        {
            ctrl.Where = new WhereCondition(ctrl.Where)
				.WhereFalse("ClassIsContentOnly")
				.Or()
                .WhereNull("ClassIsContentOnly")
				.ToString(true);
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        // Register script
        ScriptHelper.RegisterScriptFile(this, CONTENT_CMSDESK_FOLDER + "New/New.js");

        EnsureDocumentBreadcrumbs(PageBreadcrumbs, action: PageTitle.TitleText);
    }

    #endregion
}
