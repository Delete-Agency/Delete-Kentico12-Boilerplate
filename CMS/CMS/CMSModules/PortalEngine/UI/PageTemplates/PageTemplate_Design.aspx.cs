﻿using System;

using CMS.Base.Web.UI;
using CMS.DocumentEngine;
using CMS.Helpers;
using CMS.PortalEngine;
using CMS.UIControls;


public partial class CMSModules_PortalEngine_UI_PageTemplates_PageTemplate_Design : PortalPage
{
    #region "Public properties"

    /// <summary>
    /// Document manager
    /// </summary>
    public override ICMSDocumentManager DocumentManager
    {
        get
        {
            // Enable document manager
            docMan.Visible = true;
            docMan.StopProcessing = false;
            docMan.RegisterSaveChangesScript = (PortalContext.ViewMode.IsEdit());
            docMan.MessagesPlaceHolder.UseRelativePlaceHolder = false;
            return docMan;
        }
    }


    /// <summary>
    /// Local page messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return DocumentManager.MessagesPlaceHolder;
        }
    }

    #endregion


    #region "Page methods"

    /// <summary>
    /// PreInit event handler.
    /// </summary>
    protected override void OnPreInit(EventArgs e)
    {
        base.OnPreInit(e);

        // Init the page components
        PageManager = manPortal;
        manPortal.SetMainPagePlaceholder(plc);

        int pageTemplateId = QueryHelper.GetInteger("templateid", 0);
        UIContext.EditedObject = PageTemplateInfoProvider.GetPageTemplateInfo(pageTemplateId);

        // Prepare the page info
        PageInfo pi = PageInfoProvider.GetVirtualPageInfo(pageTemplateId);
        pi.DocumentNamePath = "/" + GetString("edittabs.design");
        
        DocumentContext.CurrentPageInfo = pi;

        // Set the design mode
        PortalContext.SetRequestViewMode(ViewModeEnum.Design);
        ContextHelper.Add(CookieName.DisplayContentInDesignMode, "0", true, false, false, DateTime.MinValue);

        ManagersContainer = plcManagers;
        ScriptManagerControl = manScript;
    }


    /// <summary>
    /// PreRender event handler.
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (QueryHelper.Contains("deviceprofileid"))
        {
            ScriptHelper.RegisterGetTopScript(this);
            ShowWarning(GetString("devicelayout.designmode.warning"), null, null);
        }

        // Init the header tags
        ltlTags.Text = HeaderTags;
    }

    #endregion
}
