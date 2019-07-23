﻿using System;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.DeviceProfiles;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.Membership;
using CMS.PortalEngine;
using CMS.PortalEngine.Web.UI;
using CMS.Synchronization;
using CMS.UIControls;


public partial class CMSModules_PortalEngine_Controls_Layout_TemplateLayoutEdit : CMSPreviewControl
{
    #region "Variables"

    // Type of the edited object
    private enum EditedObjectTypeEnum
    {
        Layout = 0,
        Template = 1,
        DeviceLayout = 2
    }

    private HeaderAction createDeviceLayout;
    private HeaderAction removeDeviceLayout;

    protected bool startWithFullScreen = false;
    private int previewState;
    private int mTemplateId;
    private int? mDeviceProfileID;
    private EditedObjectTypeEnum? mEditedObjectType;
    private PageTemplateInfo mPageTemplateInfo;
    private DeviceProfileInfo mDeviceProfileInfo;
    private bool enablePreview = true;

    private bool? mShowSharedLayoutWarnings;
    private bool requiresDialog;
    private bool dialog;
    private int sharedLayoutId;

    private LayoutInfo actualLayoutInfo;
    private bool allowDeviceButtons = true;

    private DeviceProfileInfo mDeviceProfile;
    private bool deviceChecked;

    #endregion


    #region "Private properties"

    /// <summary>
    /// Gets or sets the edited object.
    /// </summary>
    private new BaseInfo EditedObject
    {
        get
        {
            return base.EditedObject as BaseInfo;
        }
    }


    /// <summary>
    /// Gets the type of the edited object.
    /// </summary>
    private EditedObjectTypeEnum EditedObjectType
    {
        get
        {
            if (mEditedObjectType == null)
            {
                mEditedObjectType = EditedObjectTypeEnum.Layout;

                if (EditedObject != null)
                {
                    switch (EditedObject.TypeInfo.ObjectType)
                    {
                        case PageTemplateInfo.OBJECT_TYPE:
                            mEditedObjectType = EditedObjectTypeEnum.Template;
                            break;

                        case PageTemplateDeviceLayoutInfo.OBJECT_TYPE:
                            mEditedObjectType = EditedObjectTypeEnum.DeviceLayout;
                            break;
                    }
                }
            }

            return mEditedObjectType.Value;
        }
    }


    /// <summary>
    /// Gets a value that indicates if warnings about the shared layouts should be visible.
    /// </summary>
    private bool ShowSharedLayoutWarnings
    {
        get
        {
            if (!mShowSharedLayoutWarnings.HasValue)
            {
                mShowSharedLayoutWarnings = QueryHelper.GetBoolean("sharedlayoutwarnings", true);
            }
            return mShowSharedLayoutWarnings.Value;
        }
    }


    /// <summary>
    /// Gets the template ID.
    /// </summary>
    private int TemplateID
    {
        get
        {
            if (mTemplateId == 0)
            {
                mTemplateId = QueryHelper.GetInteger("templateid", 0);
            }

            return mTemplateId;
        }
    }


    /// <summary>
    /// Gets the device profile ID.
    /// </summary>
    private int DeviceProfileID
    {
        get
        {
            if (mDeviceProfileID == null)
            {
                mDeviceProfileID = 0;

                DeviceProfileInfo deviceProfile = DeviceContext.CurrentDeviceProfile;
                if (deviceProfile != null)
                {
                    mDeviceProfileID = deviceProfile.ProfileID;
                }
            }

            return mDeviceProfileID.Value;
        }
    }


    /// <summary>
    /// Gets the current page template info object.
    /// </summary>
    private PageTemplateInfo PageTemplateInfo
    {
        get
        {
            return mPageTemplateInfo ?? (mPageTemplateInfo = PageTemplateInfoProvider.GetPageTemplateInfo(TemplateID));
        }
    }


    /// <summary>
    /// Gets the current device profile info object.
    /// </summary>
    private DeviceProfileInfo DeviceProfileInfo
    {
        get
        {
            return mDeviceProfileInfo ?? (mDeviceProfileInfo = DeviceProfileInfoProvider.GetDeviceProfileInfo(DeviceProfileID));
        }
    }

    #endregion


    #region "Properties"

    /// <summary>
    /// Ensures correct displaying of info messages.
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return pnlMessagePlaceholder;
        }
    }


    /// <summary>
    /// Indicates if the control should perform the operations.
    /// </summary>
    public override bool StopProcessing
    {
        get
        {
            return base.StopProcessing;
        }
        set
        {
            base.StopProcessing = value;
            EditForm.StopProcessing = value;
        }
    }


    /// <summary>
    /// Indicates if the control is used on the live site.
    /// </summary>
    public override bool IsLiveSite
    {
        get
        {
            return base.IsLiveSite;
        }
        set
        {
            base.IsLiveSite = value;
            EditForm.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Actual form according to correct object type.
    /// </summary>
    public UIForm EditForm
    {
        get
        {
            switch (EditedObjectType)
            {
                case EditedObjectTypeEnum.Template:
                    return EditFormTemplate;

                case EditedObjectTypeEnum.DeviceLayout:
                    return EditFormDeviceLayout;

                default:
                    return EditFormLayout;
            }
        }
    }


    /// <summary>
    /// If true, for PageTemplates it is possible to switch between shared and custom layout.
    /// </summary>
    public bool AllowTypeSwitching
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowTypeSwitching"), false);
        }
        set
        {
            SetValue("AllowTypeSwitching", value);
        }
    }


    /// <summary>
    /// Gets the current device profile
    /// </summary>
    private DeviceProfileInfo CurrentDeviceProfile
    {
        get
        {
            if (!deviceChecked)
            {
                mDeviceProfile = DeviceContext.CurrentDeviceProfile;
                deviceChecked = true;
            }
            return mDeviceProfile;
        }
    }

    #endregion


    #region "Page events"

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        // Hide all UIForms
        EditFormTemplate.Visible = false;
        EditFormTemplate.StopProcessing = true;
        EditFormDeviceLayout.Visible = false;
        EditFormDeviceLayout.StopProcessing = true;
        EditFormLayout.Visible = false;
        EditFormLayout.StopProcessing = true;

        sharedLayoutId = QueryHelper.GetInteger("newshared", 0);

        PageTemplateDeviceLayoutInfo deviceLayout = PageTemplateDeviceLayoutInfoProvider.GetTemplateDeviceLayoutInfo(TemplateID, DeviceProfileID);

        // Show UIForm for the current edited object type
        switch (EditedObjectType)
        {
            case EditedObjectTypeEnum.Template:
                EditFormTemplate.Visible = true;
                EditFormTemplate.StopProcessing = false;
                break;

            case EditedObjectTypeEnum.DeviceLayout:
                EditFormDeviceLayout.Visible = true;
                EditFormDeviceLayout.StopProcessing = false;
                break;

            case EditedObjectTypeEnum.Layout:
                EditFormLayout.Visible = true;
                EditFormLayout.StopProcessing = false;
                break;
        }

        if (AllowTypeSwitching && (EditedObjectType == EditedObjectTypeEnum.Layout))
        {
            if (deviceLayout != null)
            {
                // Force ObjectManager to work with PageTemplateDeviceLayout even though the EditedObject is layout
                editMenuElem.ObjectManager.ObjectType = PageTemplateDeviceLayoutInfo.OBJECT_TYPE;
                editMenuElem.ObjectManager.ObjectID = deviceLayout.TemplateDeviceLayoutID;
            }
            else
            {
                // Force ObjectManager to work with PageTemplate even though the EditedObject is layout
                editMenuElem.ObjectManager.ObjectType = PageTemplateInfo.OBJECT_TYPE;
                editMenuElem.ObjectManager.ObjectID = TemplateID;
            }
        }

        pnlType.Visible = AllowTypeSwitching;
        requiresDialog = ((CMSDeskPage)Page).RequiresDialog;
        dialog = (QueryHelper.GetBoolean("dialog", false) || QueryHelper.GetBoolean("isindialog", false));
    }


    protected void EditForm_Create(object sender, EventArgs e)
    {
        EditForm.OnAfterDataLoad += EditForm_OnAfterDataLoad;
    }


    /// <summary>
    /// UIForm's after data load event handler.
    /// </summary>
    protected void EditForm_OnAfterDataLoad(object sender, EventArgs e)
    {
        // Setup Preview mode state
        previewState = GetPreviewState();

        bool fullScreen = ((previewState != 0) && enablePreview);
        switch (EditedObjectType)
        {
            case EditedObjectTypeEnum.Template:
                codeElem.Editor.AutoSize = true;
                codeElem.Editor.ParentElementID = ParentClientID;
                codeElem.FullscreenMode = fullScreen;
                break;

            case EditedObjectTypeEnum.Layout:
                iconSelector.Value = EditedObject.Generalized.ObjectThumbnailGUID;
                codeLayoutElem.Editor.AutoSize = true;
                codeLayoutElem.Editor.ParentElementID = ParentClientID;
                codeLayoutElem.FullscreenMode = fullScreen;
                break;

            case EditedObjectTypeEnum.DeviceLayout:
                deviceCode.Editor.AutoSize = true;
                deviceCode.Editor.ParentElementID = ParentClientID;
                deviceCode.FullscreenMode = fullScreen;
                break;
        }
    }


    /// <summary>
    /// Display info messages
    /// </summary>
    /// <param name="forceDisplay">If true, message is displayed even on postback</param>
    private void DisplayMessage(bool forceDisplay)
    {
        bool showMessage = editMenuElem.ObjectManager.IsObjectChecked() && (!RequestHelper.IsPostBack() || forceDisplay);

        // Display shared template warning
        switch (EditedObjectType)
        {
            case EditedObjectTypeEnum.Template:
                {
                    if (showMessage)
                    {
                        PageTemplateInfo pti = EditFormTemplate.EditedObject as PageTemplateInfo;
                        ShowSharedTemplateWarningMessage(pti);
                        codeElem.ShowMessage();
                    }
                }
                break;

            case EditedObjectTypeEnum.Layout:
                {
                    LayoutInfo layoutInfo = EditFormLayout.EditedObject as LayoutInfo;
                    if (showMessage)
                    {
                        if (DialogMode && ShowSharedLayoutWarnings && (layoutInfo != null))
                        {
                            ShowInformation(string.Format(GetString("layout.sharedwarning"), layoutInfo.LayoutDisplayName));
                        }
                        codeLayoutElem.ShowMessage();
                    }
                }
                break;

            case EditedObjectTypeEnum.DeviceLayout:
                {
                    PageTemplateDeviceLayoutInfo deviceLayout = EditFormDeviceLayout.EditedObject as PageTemplateDeviceLayoutInfo;
                    if (showMessage)
                    {
                        if (deviceLayout != null)
                        {
                            if (DialogMode && (deviceLayout.LayoutID > 0))
                            {
                                ShowInformation(GetString("layout.sharedwarning"));
                            }
                            else
                            {
                                PageTemplateInfo pti = PageTemplateInfoProvider.GetPageTemplateInfo(deviceLayout.PageTemplateID);
                                ShowSharedTemplateWarningMessage(pti);
                            }
                        }
                        deviceCode.ShowMessage();
                    }
                }
                break;
        }

        if (!RequestHelper.IsPostBack() && DialogMode)
        {
            SetDeviceWarning();
        }
    }


    /// <summary>
    // Shows warning for not mapped device profiles
    /// </summary>
    private void SetDeviceWarning()
    {
        if (CurrentDeviceProfile != null)
        {
            PageTemplateDeviceLayoutInfo deviceLayout = PageTemplateDeviceLayoutInfoProvider.GetTemplateDeviceLayoutInfo(TemplateID, CurrentDeviceProfile.ProfileID);
            bool layoutIsMapped = (deviceLayout != null);

            // Check layout auto mapping
            if (!layoutIsMapped && (TemplateID > 0) && (PageTemplateInfo != null))
            {
                layoutIsMapped |= (DeviceProfileLayoutInfoProvider.GetTargetLayoutInfo(CurrentDeviceProfile.ProfileID, PageTemplateInfo.LayoutID) != null);
            }

            // Device layout not defined
            if (!layoutIsMapped)
            {
                ShowWarning(GetString("devicelayout.notdefined"));
            }
        }
    }


    /// <summary>
    /// Returns preview state based on edited object type.
    /// </summary>
    private int GetPreviewState()
    {
        int state = 0;

        switch (EditedObjectType)
        {
            case EditedObjectTypeEnum.Template:
                state = GetPreviewStateFromCookies(PAGETEMPLATELAYOUT);
                break;

            case EditedObjectTypeEnum.Layout:
                state = GetPreviewStateFromCookies(PAGELAYOUT);
                break;

            case EditedObjectTypeEnum.DeviceLayout:
                state = GetPreviewStateFromCookies(DEVICELAYUOT);
                break;
        }

        // No preview for UI templates
        if ((PageTemplateInfo != null) && (PageTemplateInfo.PageTemplateType == PageTemplateTypeEnum.UI))
        {
            enablePreview = false;
            state = 0;
        }

        return state;
    }


    protected override void OnLoad(EventArgs e)
    {
        DisplayMessage(false);

        // Display the Create/Remove device layout buttons when the device is other than the default device
        allowDeviceButtons = string.IsNullOrEmpty(QueryHelper.GetString("devicename", null));

        base.OnLoad(e);

        // Setup Preview mode state
        previewState = GetPreviewState();

        if (AllowTypeSwitching && (EditedObject != null))
        {
            if (EditedObjectType == EditedObjectTypeEnum.DeviceLayout)
            {
                // Custom device layout
                radCustom.Checked = ((((PageTemplateDeviceLayoutInfo)EditedObject).LayoutID == 0) && (sharedLayoutId == 0));
                radShared.Checked = !radCustom.Checked;
            }
            else
            {
                // Standard layout
                radCustom.Checked = (EditedObjectType == EditedObjectTypeEnum.Template);
                radShared.Checked = !radCustom.Checked;
            }
        }

        InitializeHeaderActions();
        RegisterScripts();

        editMenuElem.ObjectEditMenu.PreviewMode = true;
        editMenuElem.MenuPanel.CssClass = "PreviewMenu";
        editMenuElem.ObjectManager.OnAfterAction += ObjectManager_OnAfterAction;
        EditForm.OnBeforeSave += EditForm_OnBeforeSave;
    }


    private void EditForm_OnBeforeSave(object sender, EventArgs e)
    {
        // Don't save shared layout object, update only necessary objects
        if ((EditedObjectType == EditedObjectTypeEnum.Layout) && radShared.Checked)
        {
            LayoutInfo li = EditedObject as LayoutInfo;

            // Get the current layout type
            PageTemplateLayoutTypeEnum layoutType = PageTemplateLayoutTypeEnum.PageTemplateLayout;
            PageTemplateDeviceLayoutInfoProvider.GetLayoutObject(PageTemplateInfo, DeviceProfileInfo, out layoutType);

            switch (layoutType)
            {
                case PageTemplateLayoutTypeEnum.PageTemplateLayout:
                case PageTemplateLayoutTypeEnum.SharedLayout:
                    {
                        int newLayoutId = ValidationHelper.GetInteger(drpLayout.Value, 0);

                        // We need to save also page template if shared template is used
                        if ((PageTemplateInfo != null) && (PageTemplateInfo.LayoutID != li.LayoutId))
                        {
                            PageTemplateInfo.LayoutID = newLayoutId;
                            PageTemplateInfo.Update();
                        }
                    }
                    break;

                case PageTemplateLayoutTypeEnum.DeviceSharedLayout:
                case PageTemplateLayoutTypeEnum.DeviceLayout:
                    {
                        // We need to save also template device layout if shared template is used
                        PageTemplateDeviceLayoutInfo deviceLayout = PageTemplateDeviceLayoutInfoProvider.GetTemplateDeviceLayoutInfo(TemplateID, DeviceProfileID);
                        if (deviceLayout != null)
                        {
                            deviceLayout.LayoutID = ValidationHelper.GetInteger(drpLayout.Value, 0);
                            deviceLayout.LayoutCode = null;
                            deviceLayout.LayoutCSS = null;
                            deviceLayout.Update();
                        }
                    }
                    break;
            }

            ShowChangesSaved();

            // Prevent from saving object
            EditForm.StopProcessing = true;
        }
    }


    protected override void OnPreRender(EventArgs e)
    {
        HandleFullScreen();

        base.OnPreRender(e);

        if (AllowTypeSwitching && (EditedObject != null))
        {
            if (sharedLayoutId > 0)
            {
                drpLayout.Value = sharedLayoutId;
            }
            else if (EditedObjectType == EditedObjectTypeEnum.Layout)
            {
                drpLayout.Value = EditedObject.Generalized.ObjectID;
            }

            radCustom.Text = GetString("TemplateLayout.Custom");

            var sanitizedCurrentUrl = ScriptHelper.GetString(RequestContext.CurrentURL, encapsulate: false);
            var customLayoutUrl = URLHelper.AddParameterToUrl(sanitizedCurrentUrl, "newshared", "0");
            customLayoutUrl = URLHelper.AddParameterToUrl(customLayoutUrl, "oldshared", drpLayout.Value.ToString());

            radCustom.Attributes.Add("onclick", "window.location = '" + customLayoutUrl + "'");

            radShared.Text = GetString("TemplateLayout.Shared");
            if (drpLayout.UniSelector.HasData)
            {
                var sharedLayoutUrl = URLHelper.AddParameterToUrl(sanitizedCurrentUrl, "newshared", drpLayout.Value.ToString());
                radShared.Attributes.Add("onclick", "window.location = '" + sharedLayoutUrl + "'");
            }

            // Get the current layout type
            bool radioButtonsEnabled = !SynchronizationHelper.UseCheckinCheckout;
            if (DeviceProfileID > 0)
            {
                // Device profile layout
                PageTemplateDeviceLayoutInfo dli = PageTemplateDeviceLayoutInfoProvider.GetTemplateDeviceLayoutInfo(TemplateID, DeviceProfileID);
                if (dli != null)
                {
                    radioButtonsEnabled |= dli.Generalized.IsCheckedOutByUser(MembershipContext.AuthenticatedUser);
                }
            }
            else
            {
                // Page template layout
                radioButtonsEnabled |= PageTemplateInfo.Generalized.IsCheckedOutByUser(MembershipContext.AuthenticatedUser);
            }

            // Disable also radio buttons when the object is not checked out
            radShared.Enabled = radCustom.Enabled = drpLayout.Enabled = radioButtonsEnabled;
        }

        if ((EditedObjectType == EditedObjectTypeEnum.Layout) && (AllowTypeSwitching || DialogMode))
        {
            pnlServer.Visible = false;
            pnlLayout.CategoryTitleResourceString = null;
        }

        SetActionVisiblity();

        RegisterInitScripts(pnlBody.ClientID, editMenuElem.MenuPanel.ClientID, startWithFullScreen);

        if (QueryHelper.GetBoolean("refreshParent", false))
        {
            ScriptHelper.RegisterStartupScript(this, typeof(string), "refreshParent", "window.refreshPageOnClose = true;", true);
        }

        HandleWebpartZonesCountWarning();

        if (DialogMode && QueryHelper.GetBoolean("wopenerrefresh", false) && !ValidationHelper.GetBoolean(hdnWOpenerRefreshed.Value, false))
        {
            RegisterWOpenerRefreshScript();
            hdnWOpenerRefreshed.Value = "1";
        }

        var deviceActions = QueryHelper.GetBoolean("deviceactions", true);
        if (!deviceActions)
        {
            if (createDeviceLayout != null)
            {
                createDeviceLayout.Visible = false;
            }
            if (removeDeviceLayout != null)
            {
                removeDeviceLayout.Visible = false;
            }
        }

        // Try to get page template
        EditForm.EnableByLockState();

        // Enable DDL and disable UIForm for shared layouts
        if (radShared.Checked)
        {
            DisableEditableFields();
        }
        else
        {
            drpLayout.Enabled = false;
        }

        // Check whether virtual objects are allowed
        if (!SettingsKeyInfoProvider.VirtualObjectsAllowed)
        {
            ShowWarning(GetString("VirtualPathProvider.NotRunning"));
        }
    }


    /// <summary>
    /// Handles the OnAfterAction event of the ObjectManager control.
    /// </summary>
    protected void ObjectManager_OnAfterAction(object sender, SimpleObjectManagerEventArgs e)
    {
        if (!e.IsValid)
        {
            MessagesPlaceHolder.WarningText = "";
            return;
        }

        if ((e.ActionName == ComponentEvents.SAVE) || (e.ActionName == ComponentEvents.CHECKIN))
        {
            // Register refresh script
            string refreshScript = ScriptHelper.GetScript("if ((wopener != null) && (wopener.RefreshPage != null)) {wopener.RefreshPage();}");
            ScriptHelper.RegisterClientScriptBlock(Page, typeof(String), "pageTemplateRefreshScript", refreshScript);

            // Register preview refresh
            RegisterRefreshScript();

            // Close if required
            if (ValidationHelper.GetBoolean(hdnClose.Value, false))
            {
                ScriptHelper.RegisterStartupScript(Page, typeof(string), "CloseDialogPreviewScript", ScriptHelper.GetScript("CloseDialog();"));
            }

            // Hide warning after save
            MessagesPlaceHolder.WarningText = "";
        }
        else if (e.ActionName == ComponentEvents.UNDO_CHECKOUT)
        {
            if (AllowTypeSwitching)
            {
                var url = RequestContext.CurrentURL;
                url = URLHelper.RemoveParameterFromUrl(url, "newshared");
                url = URLHelper.RemoveParameterFromUrl(url, "oldshared");
                url = URLHelper.AddParameterToUrl(url, "wopenerrefresh", "1");
                URLHelper.ResponseRedirect(url);
            }
        }
        else if (e.ActionName == ComponentEvents.CHECKOUT)
        {
            DisplayMessage(true);
        }

        switch (e.ActionName)
        {
            case ComponentEvents.SAVE:
            case ComponentEvents.CHECKOUT:
            case ComponentEvents.CHECKIN:
            case ComponentEvents.UNDO_CHECKOUT:
                if (DialogMode)
                {
                    RegisterWOpenerRefreshScript();
                }
                else if (dialog)
                {
                    ScriptHelper.RegisterClientScriptBlock(Page, typeof(string), "parentWOpenerRefresh", ScriptHelper.GetScript("if (parent && parent.wopener && parent.wopener.refresh) { parent.wopener.refresh(); }"));
                }
                break;
        }

        if (!AllowTypeSwitching && (EditedObjectType == EditedObjectTypeEnum.Layout) && (e.ActionName != ComponentEvents.CHECKOUT) && !DialogMode)
        {
            ScriptHelper.RefreshTabHeader(Page, EditForm.EditedObject.Generalized.ObjectDisplayName);
        }

        // No save for checkout
        if (e.ActionName != ComponentEvents.CHECKOUT)
        {
            ShowChangesSaved();
        }
    }


    /// <summary>
    /// Layout DropDownlist change.
    /// </summary>
    protected void selectShared_Changed(object sender, EventArgs ea)
    {
        if (EditedObject != null)
        {
            URLHelper.ResponseRedirect(URLHelper.AddParameterToUrl(URLHelper.AddParameterToUrl(RequestContext.CurrentURL, "newshared", drpLayout.Value + ""), "oldshared", EditedObject.Generalized.ObjectID.ToString()));
        }
    }


    /// <summary>
    /// Ensures full screen mode for preview.
    /// </summary>
    private void HandleFullScreen()
    {
        startWithFullScreen = ((previewState != 0) && editMenuElem.ObjectManager.IsObjectChecked());

        // Wrong calculation for these browsers, when div is hidden.
        bool hide = (BrowserHelper.IsSafari() || BrowserHelper.IsChrome());
        pnlBody.Attributes["style"] = (startWithFullScreen && !hide) ? "display: none;" : "display: block;";
    }


    /// <summary>
    /// Shows warning if there is wrong web part zone count set.
    /// </summary>
    private void HandleWebpartZonesCountWarning()
    {
        if (EditedObjectType == EditedObjectTypeEnum.Layout)
        {
            if (actualLayoutInfo == null)
            {
                actualLayoutInfo = EditedObject as LayoutInfo;
            }

            FormEngineUserControl zonesCountControl = EditFormLayout.FieldControls["layoutzonecount"];
            if ((actualLayoutInfo == null) || (zonesCountControl == null) || (!zonesCountControl.Visible))
            {
                return;
            }

            // Handle difference between counted and entered number of web part zones
            if (actualLayoutInfo.LayoutZoneCountAutomatic != actualLayoutInfo.LayoutZoneCount)
            {
                string msg = (actualLayoutInfo.LayoutZoneCount >= 0) ? 
                    String.Format(ResHelper.GetString("pagelayout.webpartzonescountnotmatch"), actualLayoutInfo.LayoutZoneCount, actualLayoutInfo.LayoutZoneCountAutomatic) : 
                    ResHelper.GetString("pagelayout.webpartzonescountmissing");
                ShowWarning(msg);
            }
        }
    }


    /// <summary>
    /// Initializes header action control.
    /// </summary>
    private void InitializeHeaderActions()
    {
        // Add preview action
        HeaderAction preview = new HeaderAction
        {
            Text = GetString("general.preview"),
            OnClientClick = "performToolbarAction('split');return false;",
            Visible = ((previewState == 0) && enablePreview),
            Index = 1,
            Tooltip = GetString("preview.tooltip")
        };
        editMenuElem.ObjectEditMenu.AddExtraAction(preview);

        // Custom device layout
        if (allowDeviceButtons && (DialogMode || requiresDialog) && (DeviceProfileID > 0))
        {
            // "Create device layout" button
            string dialogUrl = UrlResolver.ResolveUrl("~/CMSModules/PortalEngine/UI/PageLayouts/PageLayout_CustomDeviceLayout.aspx?dialog=1");
            dialogUrl += "&templateid=" + TemplateID + "&deviceprofileid=" + DeviceProfileID;

            // Create header button
            createDeviceLayout = new HeaderAction
            {
                Text = GetString("devicelayout.create"),
                Tooltip = GetString("devicelayout.create.tooltip"),
                OnClientClick = "modalDialog('" + dialogUrl + "', 'createCustomDeviceLayout', 450, 420); return false;",
                Visible = false
            };
            editMenuElem.ObjectEditMenu.AddExtraAction(createDeviceLayout);

            // "Remove device layout" button
            removeDeviceLayout = new HeaderAction
            {
                OnClientClick = "javascript:if (confirm(" + ScriptHelper.GetLocalizedString("devicelayout.remove.confirmation") + ")) { window.refreshPageOnClose = true; return true; } return false;",
                Text = GetString("devicelayout.remove"),
                Tooltip = GetString("devicelayout.remove.tooltip"),
                CommandName = "removelayout",
                Visible = false
            };
            editMenuElem.ObjectEditMenu.AddExtraAction(removeDeviceLayout);

            editMenuElem.ObjectEditMenu.HeaderActions.ActionPerformed += HeaderActions_ActionPerformed;
        }
    }


    /// <summary>
    /// Handles the ActionPerformed event of the HeaderActions control.
    /// </summary>
    protected void HeaderActions_ActionPerformed(object sender, CommandEventArgs e)
    {
        switch (e.CommandName.ToLowerCSafe())
        {
            case "removelayout":
                // Remove custom device layout
                PageTemplateDeviceLayoutInfo deviceLayout = PageTemplateDeviceLayoutInfoProvider.GetTemplateDeviceLayoutInfo(TemplateID, DeviceProfileID);
                PageTemplateDeviceLayoutInfoProvider.DeleteTemplateDeviceLayoutInfo(deviceLayout);

                ShowConfirmation(GetString("devicelayout.removed"));

                // Refresh the current page
                string refreshUrl = URLHelper.AddParameterToUrl(RequestContext.RawURL, "refreshParent", "1");
                URLHelper.Redirect(refreshUrl);
                break;
        }
    }


    /// <summary>
    /// Sets the visibility of actions according to checkout info
    /// </summary>
    private void SetActionVisiblity()
    {
        // Show "Create/Remove device layout" buttons
        if (allowDeviceButtons && (DialogMode || requiresDialog) && (DeviceProfileID > 0))
        {
            PageTemplateDeviceLayoutInfo deviceLayout = PageTemplateDeviceLayoutInfoProvider.GetTemplateDeviceLayoutInfo(TemplateID, DeviceProfileID);
            if (deviceLayout != null)
            {
                removeDeviceLayout.Visible = true;
            }
            else
            {
                createDeviceLayout.Visible = true;
            }
        }
    }


    /// <summary>
    /// Registers script for header resize
    /// </summary>
    private void RegisterScripts()
    {
        // Register action script for dialog purposes
        ScriptHelper.RegisterClientScriptBlock(Page, typeof(String), "PreviewHierarchyPerformAction", ScriptHelper.GetScript("function actionPerformed(action) { if (action == 'saveandclose') { document.getElementById('" + hdnClose.ClientID + "').value = '1'; }; " + editMenuElem.ObjectManager.GetJSFunction(ComponentEvents.SAVE, null, null) + "; }"));
    }


    private void RegisterWOpenerRefreshScript()
    {
        ScriptHelper.RegisterStartupScript(Page, typeof(string), "wOpenerRefresh", ScriptHelper.GetScript("if (wopener && wopener.refresh) { wopener.refresh(); }"));
    }


    /// <summary>
    /// Shows the shared template warning message for re-usable templates.
    /// </summary>
    /// <param name="pti">The page template info object</param>
    private void ShowSharedTemplateWarningMessage(PageTemplateInfo pti)
    {
        if ((pti != null) && pti.IsReusable && ShowSharedLayoutWarnings)
        {
            ShowInformation(GetString("template.shared.warning"));
        }
    }


    /// <summary>
	/// Disables editable form fields according to appropriate object type.
	/// </summary>
    private void DisableEditableFields()
    {
        switch (EditedObjectType)
        {
            case EditedObjectTypeEnum.Layout:
                codeLayoutElem.Enabled = false;
                cssLayoutEditor.Editor.Enabled = false;
                break;
            case EditedObjectTypeEnum.Template:
                codeElem.Editor.Enabled = false;
                cssEditor.Editor.Enabled = false;
                break;
            case EditedObjectTypeEnum.DeviceLayout:
                deviceCode.Enabled = false;
                cssDeviceEditor.Editor.Enabled = false;
                break;
        }
    }

    #endregion
}