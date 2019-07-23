﻿using System;
using System.Data;
using System.Web.UI;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.SiteProvider;
using CMS.UIControls;


public partial class CMSModules_AdminControls_Controls_Class_TransformationEdit : CMSPreviewControl
{
    #region "Variables"

    protected bool startWithFullScreen = false;
    private TransformationInfo mTransformationInfo;
    HeaderAction generate;
    private DataClassInfo mClassInfo;

    #endregion


    #region "Properties"

    /// <summary>
    /// Edited transformation.
    /// </summary>
    private TransformationInfo TransformationInfo
    {
        get
        {
            if (mTransformationInfo == null)
            {
                mTransformationInfo = (editElem.EditedObject as TransformationInfo) ?? UIContext.EditedObject as TransformationInfo;
            }
            return mTransformationInfo;
        }
    }


    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return pnlMessagePlaceholder;
        }
    }


    /// <summary>
    /// Indicates whether control is opnened in dialog
    /// </summary>
    private bool IsDialog
    {
        get
        {
            return QueryHelper.GetBoolean("editonlycode", false);
        }
    }


    /// <summary>
    /// Parent data class of edited transformation.
    /// </summary>
    private DataClassInfo ClassInfo
    {
        get
        {
            if (mClassInfo == null)
            {
                mClassInfo = DataClassInfoProvider.GetDataClassInfo(TransformationInfo.TransformationClassID);
            }
            return mClassInfo;
        }
    }

    #endregion


    #region "Methods"

    protected void editElem_OnAfterDataLoad(object sender, EventArgs e)
    {
        // Page has been opened in CMSDesk and only transformation code editing is allowed        
        if (IsDialog)
        {
            // Sets dialog mode and validates input (return in case of error)
            if (SetDialogMode())
            {
                return;
            }
        }

        ucTransfCode.ClassID = TransformationInfo.TransformationClassID;
        ucTransfCode.TransformationName = TransformationInfo.TransformationName;
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        MessagesPlaceHolder.WrapperControlClientID = pnlScreen.ClientID;

        editMenuElem.ObjectManager.OnAfterAction += ObjectManager_OnAfterAction;

        // Register action script for dialog purposes
        if (IsDialog)
        {
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(String), "PreviewHierarchyPerformAction", ScriptHelper.GetScript("function actionPerformed(action) { if (action == 'saveandclose') { document.getElementById('" + hdnClose.ClientID + "').value = '1'; }; " + editMenuElem.ObjectManager.GetJSFunction(ComponentEvents.SAVE, null, null) + "; }"));
        }
        
        // For HTML type edit save as full postback (CKEditor issues on partial postback)
        if (ucTransfCode.TransformationType == TransformationTypeEnum.Html)
        {
            PostBackTrigger pt = new PostBackTrigger();
            pt.ControlID = "editMenuElem";
            pnlUpdate.Triggers.Add(pt);
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ucTransfCode.FullscreenMode = IsInPreview;

        // Add preview action
        var preview = new HeaderAction
        {
            Text = GetString("general.preview"),
            OnClientClick = "performToolbarAction('split');return false;",
            Visible = !IsInPreview,
            Tooltip = GetString("preview.tooltip"),
            GenerateSeparatorBeforeAction = true,
            Index = 2
        };

        editMenuElem.ObjectEditMenu.AddExtraAction(preview);
        editMenuElem.ObjectEditMenu.PreviewMode = true;
        editMenuElem.MenuPanel.CssClass = "PreviewMenu";

        // Show generate action if object can be edited
        if (editMenuElem.ObjectManager.IsObjectChecked())
        {
            // Generate default
            generate = new HeaderAction
            {
                Text = GetString("DocumentType_Edit_Transformation_Edit.ButtonDefault"),
                Tooltip = GetString("transformationtypecode.generatetooltip"),
                OnClientClick = "GenerateDefaultCode('default'); return false;",
                GenerateSeparatorBeforeAction = true
            };

            editMenuElem.ObjectEditMenu.AddExtraAction(generate);

            // Add extra actions for ascx type
            if (ucTransfCode.IsAscx)
            {
                if (TransformationInfo != null)
                {
                    // Show RSS and Atom generate button only for custom table or document type class
                    if ((ClassInfo != null) && ((ClassInfo.ClassIsCustomTable) || (ClassInfo.ClassIsDocumentType && ClassInfo.ClassIsCoupledClass)))
                    {
                        generate.AlternativeActions.Add(new HeaderAction
                        {
                            Text = GetString("transformationtypecode.atom"),
                            OnClientClick = "GenerateDefaultCode('atom'); return false;",
                            Tooltip = GetString("transformationtypecode.atomtooltip")
                        });

                        generate.AlternativeActions.Add(new HeaderAction
                        {
                            Text = GetString("transformationtypecode.rss"),
                            OnClientClick = "GenerateDefaultCode('rss'); return false;",
                            Tooltip = GetString("transformationtypecode.rsstooltip")
                        });

                    }
                }

                generate.AlternativeActions.Add(new HeaderAction
                {
                    Text = GetString("transformationtypecode.xml"),
                    OnClientClick = "GenerateDefaultCode('xml'); return false;",
                    Tooltip = GetString("transformationtypecode.xmltooltip")
                });
            }
        }

        startWithFullScreen = (IsInPreview && editMenuElem.ObjectManager.IsObjectChecked());

        if (ucTransfCode.TransformationType == TransformationTypeEnum.Html)
        {
            startWithFullScreen = false;
        }

        // Wrong calculation for these browsers, when div is hidden.
        bool hide = (BrowserHelper.IsSafari() || BrowserHelper.IsChrome());
        pnlContent.Attributes["style"] = (startWithFullScreen && !hide) ? "display:none" : "display:block";

        RegisterInitScripts(pnlContent.ClientID, editMenuElem.MenuPanel.ClientID, startWithFullScreen);
    }


    protected void ObjectManager_OnAfterAction(object sender, SimpleObjectManagerEventArgs e)
    {
        if (e.ActionName == ComponentEvents.SAVE)
        {
            if (IsDialog)
            {
                string script = String.Empty;
                string selector = QueryHelper.GetControlClientId("selectorid", string.Empty);
                if (!string.IsNullOrEmpty(selector))
                {
                    // Selects newly created container in the UniSelector
                    script = string.Format(@"if (wopener && wopener.US_SelectNewValue_{0}) {{wopener.US_SelectNewValue_{0}('{1}'); }}",
                        selector, TransformationInfo.TransformationFullName);
                }

                if (ValidationHelper.GetBoolean(hdnClose.Value, false))
                {
                    script += "; CloseDialog();";
                }

                if (script != String.Empty)
                {
                    ScriptHelper.RegisterStartupScript(Page, typeof(string), "UpdateSelector", ScriptHelper.GetScript(script));
                }
            }

            // Clear warning message
            MessagesPlaceHolder.WarningText = "";

            RegisterRefreshScript();
        }

        if (IsDialog)
        {
            switch (e.ActionName)
            {
                case ComponentEvents.SAVE:
                case ComponentEvents.CHECKOUT:
                case ComponentEvents.UNDO_CHECKOUT:
                case ComponentEvents.CHECKIN:
                    ScriptHelper.RegisterStartupScript(Page, typeof(string), "wopenerRefresh", ScriptHelper.GetScript("if (wopener && wopener.refresh) { wopener.refresh(); }"));
                    break;
            }
        }

        if (e.ActionName == ComponentEvents.CHECKOUT)
        {
            ucTransfCode.ShowMessage();
        }
    }


    private bool SetDialogMode()
    {
        // Error message variable
        string errorMessage = null;

        // Check if document type is registered under current site
        DataSet ds = ClassSiteInfoProvider.GetClassSites().WhereEquals("ClassID", TransformationInfo.TransformationClassID).WhereEquals("SiteID", SiteContext.CurrentSiteID).Column("ClassID");
        if (DataHelper.DataSourceIsEmpty(ds) && !CurrentUser.CheckPrivilegeLevel(UserPrivilegeLevelEnum.GlobalAdmin))
        {
            // Set error message
            errorMessage = GetString("formcontrols_selecttransformation.classnotavailablesite").Replace("%%code%%", HTMLHelper.HTMLEncode(TransformationInfo.ClassName));
        }
        else
        {
            fDisplayName.EditingControl.Enabled = false;
        }

        // Hide panel Menu and write error message
        if (!String.IsNullOrEmpty(errorMessage))
        {
            ShowError(errorMessage);
            return true;
        }

        return false;
    }

    #endregion
}
