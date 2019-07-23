﻿using System;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Automation;
using CMS.Base;
using CMS.Base.Web.UI;
using CMS.ContactManagement;
using CMS.Core;
using CMS.DataEngine;
using CMS.Helpers;
using CMS.Membership;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WorkflowEngine;


public partial class CMSModules_ContactManagement_Controls_UI_Automation_Contacts : CMSAdminEditControl
{
    #region "Public properties"

    /// <summary>
    /// Gets or sets current identifier.
    /// </summary>
    public int ProcessID
    {
        get;
        set;
    }

    #endregion


    #region "Page events"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!StopProcessing)
        {
            SetupControl();
        }
        else
        {
            listElem.StopProcessing = true;
        }
    }


    /// <summary>
    /// Setup control.
    /// </summary>
    private void SetupControl()
    {
        if (ProcessID > 0)
        {
            listElem.WhereCondition = "(StateWorkflowID = " + ProcessID + ") ";
        }

        listElem.OnExternalDataBound += listElem_OnExternalDataBound;
        listElem.RememberStateByParam = String.Empty;

        // Register scripts for contact details dialog
        ScriptHelper.RegisterDialogScript(Page);
        ScriptHelper.RegisterClientScriptBlock(this, typeof(string), "ViewContactDetails", ScriptHelper.GetScript(
            "function Refresh() {" +
            "__doPostBack('" + this.ClientID + @"', '');" +
            "}"));

        // Hide filtered fields that are on separated database, since the query that the filter
        // returns couldn't be executed anyways
        if (SqlInstallationHelper.DatabaseIsSeparated())
        {
            listElem.FilterForm.FieldsToHide.Add("ContactLastName");
            listElem.FilterForm.FieldsToHide.Add("ContactFirstName");
            listElem.FilterForm.FieldsToHide.Add("ContactEmail");
        }
    }


    protected object listElem_OnExternalDataBound(object sender, string sourceName, object parameter)
    {
        CMSGridActionButton btn;
        switch (sourceName.ToLowerCSafe())
        {
            // Delete action
            case "delete":
                btn = (CMSGridActionButton)sender;
                btn.OnClientClick = "if(!confirm(" + ScriptHelper.GetString(String.Format(ResHelper.GetString("autoMenu.RemoveStateConfirmation"), HTMLHelper.HTMLEncode(TypeHelper.GetNiceObjectTypeName(ContactInfo.OBJECT_TYPE).ToLowerCSafe()))) + ")) { return false; }" + btn.OnClientClick;
                if (!WorkflowStepInfoProvider.CanUserRemoveAutomationProcess(CurrentUser, SiteContext.CurrentSiteName))
                {
                    if (btn != null)
                    {
                        btn.Enabled = false;
                    }
                }
                break;

            case "view":
                btn = (CMSGridActionButton)sender;
                // Ensure accountID parameter value;
                var objectID = ValidationHelper.GetInteger(btn.CommandArgument, 0);
                // Contact detail URL
                string contactURL = ApplicationUrlHelper.GetElementDialogUrl(ModuleName.CONTACTMANAGEMENT, "EditContact", objectID);
                // Add modal dialog script to onClick action
                btn.OnClientClick = ScriptHelper.GetModalDialogScript(contactURL, "ContactDetail");
                break;

            // Process status column
            case "statestatus":
                return AutomationHelper.GetProcessStatus((ProcessStatusEnum)ValidationHelper.GetInteger(parameter, 0));             
        }

        return null;
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Reloads data in listing.
    /// </summary>
    /// <param name="forceReload">Whether to force complete reload</param>
    public override void ReloadData(bool forceReload)
    {
        listElem.ReloadData();
    }

    #endregion
}
