﻿using System;

using CMS.Base.Web.UI;
using CMS.Helpers;
using CMS.UIControls;
using CMS.WorkflowEngine;
using CMS.WorkflowEngine.Definitions;
using CMS.WorkflowEngine.Web.UI;


public partial class CMSModules_Workflows_Controls_UI_WorkflowStep_SourcePoint_Edit : CMSAdminEditControl
{
    #region "Variables"

    private SourcePoint mSourcePoint;

    #endregion


    #region "Properties"

    /// <summary>
    /// Redirect URL that is used after new source point is created.
    /// </summary>
    public string AfterCreateRedirectURL
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if simple mode should be displayed
    /// </summary>
    public bool SimpleMode
    {
        get;
        set;
    }


    /// <summary>
    /// Current workflow step info
    /// </summary>
    private WorkflowStepInfo CurrentStepInfo
    {
        get
        {
            return (WorkflowStepInfo)UIContext.EditedObject;
        }
    }


    /// <summary>
    /// Current source point on current step
    /// </summary>
    public SourcePoint CurrentSourcePoint
    {
        get
        {
            if (mSourcePoint == null)
            {
                if ((SourcePointGuid != Guid.Empty) && (CurrentStepInfo != null))
                {
                    mSourcePoint = CurrentStepInfo.StepDefinition.SourcePoints.Find(i => i.Guid == SourcePointGuid);

                    // Check definition point
                    var defPoint = CurrentStepInfo.StepDefinition.DefinitionPoint;
                    if (defPoint != null)
                    {
                        if ((mSourcePoint == null) && (SourcePointGuid == defPoint.Guid))
                        {
                            mSourcePoint = defPoint;
                        }
                    }
                }
            }
            return mSourcePoint;
        }
    }


    /// <summary>
    /// Source point GUID
    /// </summary>
    public Guid SourcePointGuid
    {
        get;
        set;
    }


    /// <summary>
    /// Indicates if condition editing control should be displayed
    /// </summary>
    public bool ShowCondition
    {
        get
        {
            return plcCondition.Visible;
        }
        set
        {
            plcCondition.Visible = value;
        }
    }


    /// <summary>
    /// Gets or sets name(s) of the Macro rule category(ies) which should be displayed in Rule designer. Items should be separated by semicolon.
    /// </summary>
    public string RuleCategoryNames
    {
        get
        {
            return cbCondition.RuleCategoryNames;
        }
        set
        {
            cbCondition.RuleCategoryNames = value;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Register validate event
        ComponentEvents.RequestEvents.RegisterForComponentEvent<SimpleManagerEventArgs>(ComponentName, ComponentEvents.VALIDATE_DATA, null, (s, args) =>
        {
            args.IsValid = ValidateData();
        });

        // Register save event
        ComponentEvents.RequestEvents.RegisterForComponentEvent(ComponentName, ComponentEvents.SAVE_DATA, (s, args) => SaveData(false));
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            // Do nothing!
        }
        else
        {
            plcLabel.Visible = !SimpleMode;
            RequiredFieldValidatorLabel.ErrorMessage = GetString("workflowstep.sourcepoint.requireslabel");

            if (CurrentSourcePoint != null)
            {
                // Switch default doesn't have condition
                if ((CurrentSourcePoint.Type == SourcePointTypeEnum.SwitchDefault) || (CurrentSourcePoint.Type == SourcePointTypeEnum.Timeout))
                {
                    lblCondition.Visible = cbCondition.Visible = false;
                }

                if (!RequestHelper.IsPostBack())
                {
                    txtLabel.Text = CurrentSourcePoint.Label;
                    txtText.Text = CurrentSourcePoint.Text;
                    txtTooltip.Text = CurrentSourcePoint.Tooltip;
                    cbCondition.Text = CurrentSourcePoint.Condition;
                }
            }

            if (cbCondition.Visible)
            {
                WorkflowInfo wi = WorkflowInfoProvider.GetWorkflowInfo(CurrentStepInfo.StepWorkflowID);
                cbCondition.ResolverName = WorkflowHelper.GetResolverName(wi);
                
                // In marketing automation the context is not available,
                // so the context dependent rules should remain hidden
                if (wi.IsAutomation)
                {
                    cbCondition.DisplayRuleType = 1;
                }
            }
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Validates the data, returns true if succeeded.
    /// </summary>
    public bool ValidateData()
    {
        if (Visible && !StopProcessing)
        {
            // Validate control settings
            if (CurrentStepInfo == null)
            {
                ShowError(GetString("editedobject.notexists"));
                return false;
            }

            // Validate user input
            string result = null;

            if (!SimpleMode)
            {
                result = new Validator().NotEmpty(txtLabel.Text.Trim(), GetString("workflowstep.sourcepoint.requireslabel")).Result;
            }

            if (!String.IsNullOrEmpty(result))
            {
                ShowError(result);
                return false;
            }
        }

        // Everything is OK
        return true;
    }


    /// <summary>
    /// Saves data of edited source point from controls to edited object.
    /// </summary>
    /// <param name="validateData">Indicates whether form data should be validated prior to save</param>
    public void SaveData(bool validateData)
    {
        if (Visible && !StopProcessing)
        {
            if (!validateData || ValidateData())
            {
                if (CurrentSourcePoint == null)
                {
                    // Create new source point
                    SourcePoint sp = WorkflowHelper.CreateSourcePoint(CurrentStepInfo.StepType);
                    SetValues(sp);

                    // AddSourcePoint saves the workflow step to database
                    CurrentStepInfo.AddSourcePoint(sp);
                    SourcePointGuid = sp.Guid;
                    if (String.IsNullOrEmpty(AfterCreateRedirectURL))
                    {
                        ShowChangesSaved();
                    }
                    else
                    {
                        URLHelper.Redirect(UrlResolver.ResolveUrl(URLHelper.AppendQuery(AfterCreateRedirectURL, String.Format("?workflowstepid={0}&sourcepointGuid={1}&saved=1", CurrentStepInfo.StepID, SourcePointGuid))));
                    }
                }
                else
                {
                    // Edit existing source point
                    if (CurrentSourcePoint.Label != txtLabel.Text.Trim())
                    {
                        // Refresh header
                        ScriptHelper.RefreshTabHeader(Page);
                    }

                    SetValues(CurrentSourcePoint);
                    WorkflowScriptHelper.RefreshDesignerFromDialog(Page, CurrentStepInfo.StepID, QueryHelper.GetString("graph", String.Empty));
                }               
            }
        }
    }


    /// <summary>
    /// Saves edited object to database.
    /// </summary>
    /// <param name="validateData">Indicates whether form data should be validated prior to save</param>
    public void Save(bool validateData)
    {
        if (!validateData || ValidateData())
        {
            if (validateData)
            {
                SaveData(false);
            }

            WorkflowStepInfoProvider.SetWorkflowStepInfo(CurrentStepInfo);
            ShowChangesSaved();
        }
    }


    /// <summary>
    /// Sets values from UI to provided source point object.
    /// </summary>
    /// <param name="sourcePoint">Source point</param>
    private void SetValues(SourcePoint sourcePoint)
    {
        if (sourcePoint != null)
        {
            sourcePoint.Label = txtLabel.Text;
            sourcePoint.Text = txtText.Text;
            sourcePoint.Tooltip = txtTooltip.Text;
            if (ShowCondition && (sourcePoint.Type != SourcePointTypeEnum.SwitchDefault))
            {
                sourcePoint.Condition = cbCondition.Text;
            }
        }
    }

    #endregion
}
