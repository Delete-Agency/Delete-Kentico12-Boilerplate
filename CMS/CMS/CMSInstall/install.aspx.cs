﻿using System;
using System.Security;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.CMSImportExport;
using CMS.Core;
using CMS.DataEngine;
using CMS.Globalization;
using CMS.Helpers;
using CMS.IO;
using CMS.LicenseProvider;
using CMS.Localization;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.Modules;
using CMS.UIControls;

using MessageTypeEnum = CMS.DataEngine.MessageTypeEnum;
using ProcessStatus = CMS.Base.ProcessStatus;

public partial class CMSInstall_install : CMSPage
{
    #region "InstallInfo"

    /// <summary>
    /// Installation info.
    /// </summary>
    [Serializable]
    private class InstallInfo
    {
        #region "Variables"

        private string mScriptsFullPath;
        private string mConnectionString;

        #endregion


        #region "Properties"

        /// <summary>
        /// Connection string.
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return mConnectionString;
            }

            set
            {
                mConnectionString = value;
            }
        }


        /// <summary>
        /// Scripts full path.
        /// </summary>
        public string ScriptsFullPath
        {
            get
            {
                return mScriptsFullPath;
            }

            set
            {
                mScriptsFullPath = value;
            }
        }


        /// <summary>
        /// Log context
        /// </summary>
        public ILogContext LogContext
        {
            get;
            set;
        }

        #endregion


        #region "Methods"

        /// <summary>
        /// Clears the log
        /// </summary>
        public void ClearLog()
        {
            LogContext.Clear();
        }

        #endregion
    }

    #endregion


    #region "Constants"

    private const string WWAG_KEY = "CMSWWAGInstallation";

    // Short link to help topic page.
    private const string HELP_TOPIC_LINK = "database_installation_additional";

    // Short link to help topic page regarding disk permissions.
    private const string HELP_TOPIC_DISK_PERMISSIONS_LINK = "disk_permission_problems";

    // Short link to help topic page regarding SQL error.
    private const string HELP_TOPIC_SQL_ERROR_LINK = HELP_TOPIC_LINK;
    
    // Index of collation dialog step in wizard
    private const int COLLATION_DIALOG_INDEX = 8;

    private const string INSTALL_CHECK_PAYLOAD = "Install Check Payload";
    private const string INSTALL_CHECK_PURPOSE = "Install POST Check";
    private const string INSTALL_CHECK_COOKIE_NAME = "CMSInstallCheck";
    private const string INSTALL_CHECK_EXCEPTION_MESSAGE = "POST request validation error.";

    #endregion


    #region "Variables"

    private static readonly SafeDictionary<string, InstallInfo> mInstallInfos = new SafeDictionary<string, InstallInfo>();
    private static readonly SafeDictionary<string, ImportManager> mManagers = new SafeDictionary<string, ImportManager>();

    private string hostName = RequestContext.URL.Host.ToLowerCSafe();
    private static bool dbReady;
    private static bool writePermissions = true;

    private UserInfo mImportUser;

    private LocalizedButton mNextButton;
    private LocalizedButton mPreviousButton;
    private LocalizedButton mStartNextButton;

    #endregion


    #region "Properties"

    /// <summary>
    /// User for actions context
    /// </summary>
    private UserInfo ImportUser
    {
        get
        {
            if (mImportUser == null)
            {
                mImportUser = UserInfoProvider.AdministratorUser;
                CMSActionContext.CurrentUser = mImportUser;
            }

            return mImportUser;
        }
    }


    /// <summary>
    /// Database is created.
    /// </summary>
    private bool DBCreated
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["DBCreated"], false);
        }

        set
        {
            ViewState["DBCreated"] = value;
        }
    }


    /// <summary>
    /// Database is installed.
    /// </summary>
    private bool DBInstalled
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["DBInstalled"], false);
        }

        set
        {
            ViewState["DBInstalled"] = value;
        }
    }


    /// <summary>
    /// Process GUID.
    /// </summary>
    public Guid ProcessGUID
    {
        get
        {
            if (ViewState["ProcessGUID"] == null)
            {
                ViewState["ProcessGUID"] = Guid.NewGuid();
            }

            return ValidationHelper.GetGuid(ViewState["ProcessGUID"], Guid.Empty);
        }
    }


    /// <summary>
    /// Install info.
    /// </summary>
    private InstallInfo Info
    {
        get
        {
            string key = "instInfos_" + ProcessGUID;

            return mInstallInfos[key] ?? (mInstallInfos[key] = new InstallInfo());
        }
    }


    /// <summary>
    /// Authentication type.
    /// </summary>
    private SQLServerAuthenticationModeEnum AuthenticationType
    {
        get
        {
            if ((ViewState["authentication"] == null) && RequestHelper.IsPostBack())
            {
                throw new InvalidOperationException("Connection information was lost!");
            }

            return (SQLServerAuthenticationModeEnum)ViewState["authentication"];
        }
        set
        {
            ViewState["authentication"] = value;
        }
    }


    /// <summary>
    /// Database name.
    /// </summary>
    private string Database
    {
        get
        {
            return ValidationHelper.GetString(ViewState["Database"], "");
        }
        set
        {
            ViewState["Database"] = value;
        }
    }


    /// <summary>
    /// Import manager.
    /// </summary>
    private ImportManager ImportManager
    {
        get
        {
            string key = "imManagers_" + ProcessGUID;

            return mManagers[key] ?? (mManagers[key] = CreateManager());
        }
    }


    /// <summary>
    /// New site domain.
    /// </summary>
    public string Domain
    {
        get
        {
            return ValidationHelper.GetString(ViewState["Domain"], "");
        }

        set
        {
            ViewState["Domain"] = value;
        }
    }


    /// <summary>
    /// New site site name.
    /// </summary>
    public string SiteName
    {
        get
        {
            return ValidationHelper.GetString(ViewState["SiteName"], "");
        }

        set
        {
            ViewState["SiteName"] = value;
        }
    }


    /// <summary>
    /// Connection string.
    /// </summary>
    public string ConnectionString
    {
        get
        {
            if (ViewState["connString"] == null)
            {
                ViewState["connString"] = "";
            }
            return (string)ViewState["connString"];
        }

        set
        {
            ViewState["connString"] = value;
        }
    }


    /// <summary>
    /// Step index.
    /// </summary>
    public int StepIndex
    {
        get
        {
            if (ViewState["stepIndex"] == null)
            {
                ViewState["stepIndex"] = 1;
            }
            return (int)ViewState["stepIndex"];
        }

        set
        {
            ViewState["stepIndex"] = value;
        }
    }


    private string mResult
    {
        get
        {
            if (ViewState["result"] == null)
            {
                if (RequestHelper.IsPostBack())
                {
                    throw new Exception("Information was lost!");
                }
            }
            return (string)ViewState["result"];
        }
        set
        {
            ViewState["result"] = value;
        }
    }


    private bool mDisplayLog
    {
        get
        {
            if (ViewState["displLog"] == null)
            {
                if (RequestHelper.IsPostBack())
                {
                    throw new Exception("Information was lost!");
                }
                return false;
            }
            return (bool)ViewState["displLog"];
        }
        set
        {
            ViewState["displLog"] = value;
        }
    }


    /// <summary>
    /// Flag - indicate whether DB objects will be created.
    /// </summary>
    private bool CreateDBObjects
    {
        get
        {
            return ValidationHelper.GetBoolean(ViewState["CreateDBObjects"], true);
        }
        set
        {
            ViewState["CreateDBObjects"] = value;
        }
    }


    /// <summary>
    /// Help control displayed on the navigation for the first step.
    /// </summary>
    protected HelpControl StartHelp
    {
        get
        {
            Control startStepNavigation = wzdInstaller.FindControl("StartNavigationTemplateContainerID$startStepNavigation");
            return (HelpControl)startStepNavigation.FindControl("hlpContext");
        }
    }


    /// <summary>
    /// Help control displayed on the navigation for all remaining steps.
    /// </summary>
    protected HelpControl Help
    {
        get
        {
            Control stepNavigation = wzdInstaller.FindControl("StepNavigationTemplateContainerID$stepNavigation");
            return (HelpControl)stepNavigation.FindControl("hlpContext");
        }
    }


    /// <summary>
    /// Previous step index.
    /// </summary>
    private int PreviousStep
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["PreviousStep"], 0);
        }
        set
        {
            ViewState["PreviousStep"] = value;
        }
    }

    /// <summary>
    /// Current step index.
    /// </summary>
    private int ActualStep
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["ActualStep"], 0);
        }
        set
        {
            ViewState["ActualStep"] = value;
        }
    }


    private int StepOperation
    {
        get
        {
            return ValidationHelper.GetInteger(ViewState["StepOperation"], 0);
        }
        set
        {
            ViewState["StepOperation"] = value;
        }
    }


    /// <summary>
    ///  User password.
    /// </summary>
    private string Password
    {
        get
        {
            return Convert.ToString(ViewState["install.password"]);
        }
        set
        {
            ViewState["install.password"] = value;
        }
    }

    #endregion


    #region "Step wizard buttons"

    /// <summary>
    /// Previous button.
    /// </summary>
    public LocalizedButton PreviousButton
    {
        get
        {
            return mPreviousButton ?? (mPreviousButton = wzdInstaller.FindControl("StepNavigationTemplateContainerID").FindControl("stepNavigation").FindControl("StepPrevButton") as LocalizedButton);
        }
    }


    /// <summary>
    /// Next button.
    /// </summary>
    public LocalizedButton NextButton
    {
        get
        {
            return mNextButton ?? (mNextButton = wzdInstaller.FindControl("StepNavigationTemplateContainerID").FindControl("stepNavigation").FindControl("StepNextButton") as LocalizedButton);
        }
    }


    /// <summary>
    /// Next button.
    /// </summary>
    public LocalizedButton StartNextButton
    {
        get
        {
            return mStartNextButton ?? (mStartNextButton = wzdInstaller.FindControl("StartNavigationTemplateContainerID").FindControl("startStepNavigation").FindControl("StepNextButton") as LocalizedButton);
        }
    }

    #endregion


    #region "Methods"

    private ImportManager CreateManager()
    {
        var settings = new SiteImportSettings(ImportUser);

        settings.IsWebTemplate = true;

        // Import all, but only add new data
        settings.ImportType = ImportTypeEnum.AllNonConflicting;
        settings.ImportOnlyNewObjects = true;
        settings.CopyFiles = false;

        // Allow bulk inserts for faster import, web templates must be consistent enough to allow this without collisions
        settings.AllowBulkInsert = true;

        settings.EnableSearchTasks = false;

        ImportManager im = new ImportManager(settings);
        return im;
    }


    private void ValidatePostRequest()
    {
        if (RequestHelper.IsPostBack())
        {
            var isValidPostRequest = false;
            try
            {
                var cookieValue = CookieHelper.GetValue(INSTALL_CHECK_COOKIE_NAME);
                var value = MachineKey.Unprotect(Convert.FromBase64String(cookieValue), INSTALL_CHECK_PURPOSE);
                isValidPostRequest = INSTALL_CHECK_PAYLOAD.Equals(Encoding.UTF8.GetString(value), StringComparison.InvariantCulture);
            }
            catch (Exception ex)
            {
                throw new SecurityException(INSTALL_CHECK_EXCEPTION_MESSAGE, ex);
            }

            if (!isValidPostRequest)
            {
                throw new SecurityException(INSTALL_CHECK_EXCEPTION_MESSAGE);
            }
        }
    }


    protected void Page_Load(Object sender, EventArgs e)
    {
        ValidatePostRequest();

        // Disable CSS minification
        CssLinkHelper.MinifyCurrentRequest = false;
        ScriptHelper.MinifyCurrentRequestScripts = false;

        SetBrowserClass(false);

        if (!RequestHelper.IsCallback())
        {
            EnsureApplicationConfiguration();

            ctlAsyncImport.OnFinished += worker_OnFinished;
            ctlAsyncDB.OnFinished += workerDB_OnFinished;
            databaseDialog.ServerName = userServer.ServerName;

            // Register script for pendingCallbacks repair
            // Cannot use ScriptHelper.FixPendingCallbacks as during installation the DB is not available
            ScriptManager.RegisterClientScriptInclude(this, GetType(), "cms.js", UrlResolver.ResolveUrl("~/CMSScripts/cms.js"));
            ScriptManager.RegisterClientScriptBlock(this, GetType(), "fixPendingCallbacks", "WebForm_CallbackComplete = WebForm_CallbackComplete_SyncFixed", true);

            // Javascript functions
            string script = String.Format(
@"
function Finished(sender) {{
    var errorElement = document.getElementById('{2}');

    var errorText = sender.getErrors();
    if (errorText != '') {{ 
        errorElement.innerHTML = errorText;
    }}

    var warningElement = document.getElementById('{3}');
    
    var warningText = sender.getWarnings();
    if (warningText != '') {{ 
        warningElement.innerHTML = warningText;
    }}    

    var actDiv = document.getElementById('actDiv');
    if (actDiv != null) {{ 
        actDiv.style.display = 'none'; 
    }}

    BTN_Disable('{0}');
    BTN_Enable('{1}');
}}
",
                PreviousButton.ClientID,
                NextButton.ClientID,
                pnlError.ClientID,
                pnlWarning.ClientID
            );

            // Register the script to perform get flags for showing buttons retrieval callback
            ScriptHelper.RegisterClientScriptBlock(this, GetType(), "InstallFunctions", ScriptHelper.GetScript(script));

            StartHelp.Tooltip = ResHelper.GetFileString("install.tooltip");
            StartHelp.TopicName = HELP_TOPIC_LINK;
            StartHelp.IconCssClass = "cms-icon-80";

            Response.Cache.SetNoStore();

            Help.Tooltip = ResHelper.GetFileString("install.tooltip");
            Help.IconCssClass = "cms-icon-80";


            btnPermissionTest.Click += btnPermissionTest_Click;
            btnPermissionSkip.Click += btnPermissionSkip_Click;
            btnPermissionContinue.Click += btnPermissionContinue_Click;

            // If the connection string is set, redirect
            if (!RequestHelper.IsPostBack())
            {
                if (ConnectionHelper.ConnectionAvailable)
                {
                    URLHelper.Redirect("~/default.aspx");
                }

                var protectedValue = MachineKey.Protect(Encoding.UTF8.GetBytes(INSTALL_CHECK_PAYLOAD), INSTALL_CHECK_PURPOSE);
                CookieHelper.SetValue(INSTALL_CHECK_COOKIE_NAME, Convert.ToBase64String(protectedValue), DateTime.MinValue);

                bool checkPermission = QueryHelper.GetBoolean("checkpermission", true);
                bool testAgain = QueryHelper.GetBoolean("testagain", false);

                string dir = HttpContext.Current.Server.MapPath("~/");

                // Do not test write permissions in WWAG mode
                if (!ValidationHelper.GetBoolean(SettingsHelper.AppSettings[WWAG_KEY], false))
                {
                    if (!DirectoryHelper.CheckPermissions(dir) && checkPermission)
                    {
                        writePermissions = false;
                        pnlWizard.Visible = false;
                        pnlHeaderImages.Visible = false;
                        pnlPermission.Visible = true;
                        pnlButtons.Visible = true;

                        lblPermission.Text = String.Format(ResHelper.GetFileString("Install.lblPermission"), WindowsIdentity.GetCurrent().Name, dir);
                        btnPermissionSkip.Text = ResHelper.GetFileString("Install.btnPermissionSkip");
                        btnPermissionTest.Text = ResHelper.GetFileString("Install.btnPermissionTest");

                        // Show troubleshoot link
                        pnlError.DisplayError("Install.ErrorPermissions", HELP_TOPIC_DISK_PERMISSIONS_LINK);
                        return;
                    }

                    if (testAgain)
                    {
                        pnlWizard.Visible = false;
                        pnlHeaderImages.Visible = false;
                        pnlPermission.Visible = false;
                        pnlButtons.Visible = false;
                        pnlPermissionSuccess.Visible = true;
                        lblPermissionSuccess.Text = ResHelper.GetFileString("Install.lblPermissionSuccess");
                        btnPermissionContinue.Text = ResHelper.GetFileString("Install.btnPermissionContinue");
                        writePermissions = true;
                        return;
                    }
                }
            }

            pnlWizard.Visible = true;
            pnlPermission.Visible = false;
            pnlButtons.Visible = false;

            if (!RequestHelper.IsPostBack())
            {
                if ((HttpContext.Current != null) && !ValidationHelper.GetBoolean(SettingsHelper.AppSettings["CMSWWAGInstallation"], false))
                {
                    userServer.ServerName = SystemContext.MachineName;
                }
                AuthenticationType = SQLServerAuthenticationModeEnum.SQLServerAuthentication;

                wzdInstaller.ActiveStepIndex = 0;
            }
            else
            {
                if (Password == null)
                {
                    Password = userServer.DBPassword;
                }
            }

            // Load the strings
            mDisplayLog = false;

            lblCompleted.Text = ResHelper.GetFileString("Install.DBSetupOK");
            lblMediumTrustInfo.Text = ResHelper.GetFileString("Install.MediumTrustInfo");

            ltlScript.Text = ScriptHelper.GetScript(
                "function NextStep(btnNext,elementDiv)\n" +
                "{\n" +
                "   btnNext.disabled=true;\n" +
                "   try{BTN_Disable('" + PreviousButton.ClientID + "');}catch(err){}\n" +
                ClientScript.GetPostBackEventReference(btnHiddenNext, null) +
                "}\n" +
                "function PrevStep(btnPrev,elementDiv)\n" +
                "{" +
                "   btnPrev.disabled=true;\n" +
                "   try{BTN_Disable('" + NextButton.ClientID + "');}catch(err){}\n" +
                ClientScript.GetPostBackEventReference(btnHiddenBack, null) +
                "}\n"
                );
            mResult = "";

            // Sets connection string panel
            lblConnectionString.Text = ResHelper.GetFileString("Install.lblConnectionString");
            wzdInstaller.StartNextButtonText = ResHelper.GetFileString("general.next") + " >";
            wzdInstaller.FinishCompleteButtonText = ResHelper.GetFileString("Install.Finish");
            wzdInstaller.FinishPreviousButtonText = ResHelper.GetFileString("Install.BackStep");
            wzdInstaller.StepNextButtonText = ResHelper.GetFileString("general.next") + " >";
            wzdInstaller.StepPreviousButtonText = ResHelper.GetFileString("Install.BackStep");

            // Show WWAG dialog instead of license dialog (if running in WWAG mode)
            if (ValidationHelper.GetBoolean(SettingsHelper.AppSettings[WWAG_KEY], false))
            {
                ucLicenseDialog.Visible = false;
                ucWagDialog.Visible = true;
            }
        }

        // Set the active step as 1 if connection string already initialized
        if (!RequestHelper.IsPostBack() && ConnectionHelper.IsConnectionStringInitialized)
        {
            wzdInstaller.ActiveStepIndex = 1;
            databaseDialog.UseExistingChecked = true;
        }

        NextButton.Attributes.Remove("disabled");
        PreviousButton.Attributes.Remove("disabled");

        wzdInstaller.ActiveStepChanged += wzdInstaller_ActiveStepChanged;

        RegisterRequiredMacroFields();
    }


    private static void RegisterRequiredMacroFields()
    {
        // application is not initialized and some macros cannot be resolved
        MacroContext.GlobalResolver.SetNamedSourceData(
            new MacroField("AppPath", x => SystemContext.ApplicationPath.TrimEnd('/')), false);
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (dbReady || ConnectionHelper.ConnectionAvailable)
        {
            ucSiteCreationDialog.StopProcessing = false;
            ucSiteCreationDialog.ReloadData();
        }

        // Display the log if result filled
        if (mDisplayLog)
        {
            logPanel.DisplayLog(mResult);
        }

        InitializeHeader(wzdInstaller.ActiveStepIndex);
        EnsureDefaultButton();

        PreviousButton.Visible = !ConnectionHelper.IsConnectionStringInitialized && (wzdInstaller.ActiveStepIndex != 0) && (wzdInstaller.ActiveStepIndex != 4) ||
            (wzdInstaller.ActiveStepIndex == 6);
    }


    private void wzdInstaller_ActiveStepChanged(object sender, EventArgs e)
    {
        switch (wzdInstaller.ActiveStepIndex)
        {
            case 1:
                break;
            // Finish step
            case 7:
                // Set current user default culture of the site
                LocalizationContext.PreferredCultureCode = SettingsKeyInfoProvider.GetValue(SiteName + ".CMSDefaultCultureCode");

                // Ensure virtual path provider registration if enabled
                VirtualPathHelper.RegisterVirtualPathProvider();

                // Check whether virtual path provider is running
                if (!VirtualPathHelper.UsingVirtualPathProvider)
                {
                    btnWebSite.Text = ResHelper.GetFileString("Install.lnkMediumTrust");
                    lblMediumTrustInfo.Visible = true;
                }
                else
                {
                    btnWebSite.Text = ResHelper.GetFileString("Install.lnkWebsite");
                }
                break;
        }
    }


    private void btnPermissionContinue_Click(object sender, EventArgs e)
    {
        URLHelper.Redirect(RequestContext.URL.GetLeftPart(UriPartial.Path));
    }


    private void btnPermissionSkip_Click(object sender, EventArgs e)
    {
        URLHelper.Redirect(RequestContext.URL.GetLeftPart(UriPartial.Path) + "?checkpermission=0");
    }


    private void btnPermissionTest_Click(object sender, EventArgs e)
    {
        URLHelper.Redirect(RequestContext.URL.GetLeftPart(UriPartial.Path) + "?testagain=1");
    }


    protected void btnWebSite_onClick(object sender, EventArgs e)
    {
        if (!VirtualPathHelper.UsingVirtualPathProvider)
        {
            AuthenticationHelper.AuthenticateUser(UserInfoProvider.AdministratorUserName, false);
            URLHelper.Redirect(ApplicationUrlHelper.GetApplicationUrl("cms", "administration"));
        }
        else
        {
            URLHelper.Redirect(ResolveUrl("~/default.aspx"));
        }
    }


    protected void btnHiddenBack_onClick(object sender, EventArgs e)
    {
        StepOperation = -1;
        if ((wzdInstaller.ActiveStepIndex == COLLATION_DIALOG_INDEX) || (wzdInstaller.ActiveStepIndex == 3))
        {
            StepIndex = 2;
            wzdInstaller.ActiveStepIndex = 1;
        }
        else
        {
            StepIndex--;
            wzdInstaller.ActiveStepIndex--;
        }
    }


    protected void btnHiddenNext_onClick(object sender, EventArgs e)
    {
        StepOperation = 1;
        StepIndex++;

        switch (wzdInstaller.ActiveStepIndex)
        {
            case 0:
                // Set the authentication type
                AuthenticationType = userServer.WindowsAuthenticationChecked ? SQLServerAuthenticationModeEnum.WindowsAuthentication : SQLServerAuthenticationModeEnum.SQLServerAuthentication;

                // Check the server name
                if (String.IsNullOrEmpty(userServer.ServerName))
                {
                    HandleError(ResHelper.GetFileString("Install.ErrorServerEmpty"));
                    return;
                }

                // Do not allow to use empty user name or password
                bool isSQLAuthentication = AuthenticationType == SQLServerAuthenticationModeEnum.SQLServerAuthentication;
                if  (isSQLAuthentication && (String.IsNullOrEmpty(userServer.DBUsername) || String.IsNullOrEmpty(userServer.DBPassword)))
                {
                    HandleError(ResHelper.GetFileString("Install.ErrorUserNamePasswordEmpty"));
                    return;
                }

                Password = userServer.DBPassword;

                // Check if it is possible to connect to the database
                string res = ConnectionHelper.TestConnection(AuthenticationType, userServer.ServerName, String.Empty, userServer.DBUsername, Password);
                if (!string.IsNullOrEmpty(res))
                {
                    HandleError(res, "Install.ErrorSqlTroubleshoot", HELP_TOPIC_SQL_ERROR_LINK);
                    return;
                }

                // Set credentials for the next step
                databaseDialog.AuthenticationType = AuthenticationType;
                databaseDialog.Password = Password;
                databaseDialog.Username = userServer.DBUsername;
                databaseDialog.ServerName = userServer.ServerName;

                // Move to the next step
                wzdInstaller.ActiveStepIndex = 1;
                break;

            case 1:
            case COLLATION_DIALOG_INDEX:
                // Get database name
                Database = TextHelper.LimitLength((databaseDialog.CreateNewChecked ? databaseDialog.NewDatabaseName : databaseDialog.ExistingDatabaseName), 100);

                if (string.IsNullOrEmpty(Database))
                {
                    HandleError(ResHelper.GetFileString("Install.ErrorDBNameEmpty"));
                    return;
                }

                // Set up the connection string
                if (ConnectionHelper.IsConnectionStringInitialized)
                {
                    ConnectionString = ConnectionHelper.ConnectionString;
                }
                else
                {
                    ConnectionString = ConnectionHelper.BuildConnectionString(AuthenticationType, userServer.ServerName, Database, userServer.DBUsername, Password, SqlInstallationHelper.DB_CONNECTION_TIMEOUT);
                }

                // Check if existing DB has the same version as currently installed CMS
                if (databaseDialog.UseExistingChecked && !databaseDialog.CreateDatabaseObjects)
                {
                    string dbVersion = null;
                    try
                    {
                        dbVersion = SqlInstallationHelper.GetDatabaseVersion(ConnectionString);
                    }
                    catch
                    {
                    }

                    if (String.IsNullOrEmpty(dbVersion))
                    {
                        // Unable to get DB version => DB objects missing
                        HandleError(ResHelper.GetFileString("Install.DBObjectsMissing"));
                        return;
                    }

                    if (dbVersion != CMSVersion.MainVersion)
                    {
                        // Get wrong version number
                        HandleError(ResHelper.GetFileString("Install.WrongDBVersion"));
                        return;
                    }
                }

                Info.LogContext = ctlAsyncDB.LogContext;

                // Use existing database
                if (databaseDialog.UseExistingChecked)
                {
                    // Check if DB exists
                    if (!DatabaseHelper.DatabaseExists(ConnectionString))
                    {
                        HandleError(String.Format(ResHelper.GetFileString("Install.ErrorDatabseDoesntExist"), Database));
                        return;
                    }

                    // Get collation of existing DB
                    string collation = DatabaseHelper.GetDatabaseCollation(ConnectionString);
                    DatabaseHelper.DatabaseCollation = collation;

                    if (wzdInstaller.ActiveStepIndex != COLLATION_DIALOG_INDEX)
                    {
                        // Check target database collation and inform the user if it is not fully supported
                        if (!DatabaseHelper.IsSupportedDatabaseCollation(collation))
                        {
                            ucCollationDialog.IsSqlAzure = AzureHelper.IsSQLAzureServer(userServer.ServerName);
                            ucCollationDialog.Collation = collation;
                            ucCollationDialog.InitControls();

                            // Move to "collation dialog" step
                            wzdInstaller.ActiveStepIndex = COLLATION_DIALOG_INDEX;
                            return;
                        }
                    }
                    else
                    {
                        // Change database collation for regular database
                        if (ucCollationDialog.ChangeCollationRequested)
                        {
                            DatabaseHelper.ChangeDatabaseCollation(ConnectionString, Database, DatabaseHelper.DEFAULT_DB_COLLATION);
                        }
                    }
                }
                else
                {
                    // Create a new database
                    if (!CreateDatabase(null))
                    {
                        HandleError(string.Format(ResHelper.GetFileString("Install.ErrorCreateDB"), databaseDialog.NewDatabaseName));
                        return;
                    }

                    databaseDialog.ExistingDatabaseName = databaseDialog.NewDatabaseName;
                    databaseDialog.CreateNewChecked = false;
                    databaseDialog.UseExistingChecked = true;
                }

                if ((!SystemContext.IsRunningOnAzure && writePermissions) || ConnectionHelper.IsConnectionStringInitialized)
                {
                    if (databaseDialog.CreateDatabaseObjects)
                    {
                        if (DBInstalled && DBCreated)
                        {
                            ctlAsyncDB.RaiseFinished(this, EventArgs.Empty);
                        }
                        else
                        {
                            // Run SQL installation
                            RunSQLInstallation();
                        }
                    }
                    else
                    {
                        CreateDBObjects = false;

                        // Set connection string
                        if (SettingsHelper.SetConnectionString(ConnectionHelper.ConnectionStringName, ConnectionString))
                        {
                            // Set the application connection string
                            SetAppConnectionString();

                            // Check if license key for current domain is present
                            LicenseKeyInfo lki = LicenseKeyInfoProvider.GetLicenseKeyInfo(hostName);
                            wzdInstaller.ActiveStepIndex = (lki == null) ? 4 : 5;
                            ucLicenseDialog.SetLicenseExpired();
                        }
                        else
                        {
                            ManualConnectionStringInsertion();
                        }
                    }
                }
                else
                {
                    ManualConnectionStringInsertion();
                }

                break;

            // After connection string save error
            case 2:
                // Check whether connection string is defined
                if (String.IsNullOrWhiteSpace(SettingsHelper.ConnectionStrings[ConnectionHelper.ConnectionStringName]?.ConnectionString))
                {
                    HandleError(ResHelper.GetFileString("Install.ErrorAddConnString"));
                    return;
                }

                ConnectionString = SettingsHelper.ConnectionStrings[ConnectionHelper.ConnectionStringName].ConnectionString;

                if (CreateDBObjects)
                {
                    if (DBInstalled)
                    {
                        SetAppConnectionString();

                        // Continue with next step
                        CheckLicense();
                    }
                    else
                    {
                        // Run SQL installation
                        RunSQLInstallation();
                    }
                }
                else
                {
                    // If this is installation to existing DB and objects are not created
                    if ((hostName != "localhost") && (hostName != "127.0.0.1"))
                    {
                        wzdInstaller.ActiveStepIndex = 4;
                    }
                    else
                    {
                        wzdInstaller.ActiveStepIndex = 5;
                    }
                }
                break;

            // After DB install
            case 3:
                break;

            // After license entering
            case 4:
                try
                {
                    if (ucLicenseDialog.Visible)
                    {
                        ucLicenseDialog.SetLicenseKey();
                        wzdInstaller.ActiveStepIndex = 5;
                    }
                    else if (ucWagDialog.ProcessRegistration(ConnectionString))
                    {
                        wzdInstaller.ActiveStepIndex = 5;
                    }
                }
                catch (Exception ex)
                {
                    HandleError(ex.Message);
                }
                break;

            // Site creation
            case 5:
                switch (ucSiteCreationDialog.CreationType)
                {
                    case CMSInstall_Controls_WizardSteps_SiteCreationDialog.CreationTypeEnum.Template:
                        {
                            // Get web template
                            if (ucSiteCreationDialog.TemplateName == "")
                            {
                                HandleError(ResHelper.GetFileString("install.notemplate"));
                                return;
                            }

                            var ti = WebTemplateInfoProvider.GetWebTemplateInfo(ucSiteCreationDialog.TemplateName);
                            if (ti == null)
                            {
                                HandleError("[Install]: Template not found.");
                                return;
                            }

                            var settings = PrepareSettings(ti);

                            SiteName = settings.SiteName;

                            // Import the site asynchronously
                            ImportManager.Settings = settings;

                            settings.LogContext = ctlAsyncImport.LogContext;

                            ctlAsyncImport.RunAsync(ImportManager.Import, WindowsIdentity.GetCurrent());

                            NextButton.Attributes.Add("disabled", "true");
                            PreviousButton.Attributes.Add("disabled", "true");
                            wzdInstaller.ActiveStepIndex = 6;
                        }
                        break;

                    // Else redirect to the sites application
                    case CMSInstall_Controls_WizardSteps_SiteCreationDialog.CreationTypeEnum.AddOrImportSite:
                        {
                            AuthenticationHelper.AuthenticateUser(UserInfoProvider.AdministratorUserName, false);
                            URLHelper.Redirect(ApplicationUrlHelper.GetApplicationUrl("cms", "sites"));
                        }
                        break;
                }
                break;

            default:
                wzdInstaller.ActiveStepIndex++;
                break;
        }
    }


    private SiteImportSettings PrepareSettings(WebTemplateInfo ti)
    {
        // Settings preparation
        var settings = new SiteImportSettings(ImportUser);

        // Import all, but only add new data
        settings.ImportType = ImportTypeEnum.AllNonConflicting;
        settings.ImportOnlyNewObjects = true;
        settings.CopyFiles = false;

        // Allow bulk inserts for faster import, web templates must be consistent enough to allow this without collisions
        settings.AllowBulkInsert = true;

        settings.IsWebTemplate = true;
        settings.EnableSearchTasks = false;
        settings.CreateVersion = false;
        settings.SiteName = ti.WebTemplateName;
        settings.SiteDisplayName = ti.WebTemplateDisplayName;

        if (HttpContext.Current != null)
        {
            const string www = "www.";
            if (hostName.StartsWithCSafe(www))
            {
                hostName = hostName.Remove(0, www.Length);
            }

            if (!RequestContext.URL.IsDefaultPort)
            {
                hostName += ":" + RequestContext.URL.Port;
            }

            settings.SiteDomain = hostName;
            Domain = hostName;

            string path = HttpContext.Current.Server.MapPath(ti.WebTemplateFileName);
            if (File.Exists(path + "\\template.zip"))
            {
                // Template from zip file
                path += "\\" + ZipStorageProvider.GetZipFileName("template.zip");
                settings.TemporaryFilesPath = path;
                settings.SourceFilePath = path;
                settings.TemporaryFilesCreated = true;
            }
            else
            {
                settings.SourceFilePath = path;
            }

            settings.WebsitePath = HttpContext.Current.Server.MapPath("~/");
        }

        settings.SetSettings(ImportExportHelper.SETTINGS_DELETE_SITE, true);
        settings.SetSettings(ImportExportHelper.SETTINGS_DELETE_TEMPORARY_FILES, false);

        return settings;
    }


    /// <summary>
    /// Runs SQL installation scripts
    /// </summary>
    private void RunSQLInstallation()
    {
        // Setup the installation
        var info = Info;

        info.ScriptsFullPath = SqlInstallationHelper.GetSQLInstallPath();
        info.ConnectionString = ConnectionString;

        info.ClearLog();

        // Start the installation process
        ctlAsyncDB.RunAsync(InstallDatabase, WindowsIdentity.GetCurrent());

        NextButton.Attributes.Add("disabled", "true");
        PreviousButton.Attributes.Add("disabled", "true");
        wzdInstaller.ActiveStepIndex = 3;
    }


    private void worker_OnFinished(object sender, EventArgs e)
    {
        DBCreated = true;

        // If the import finished without error
        if ((ImportManager.ImportStatus != ProcessStatus.Error) && (ImportManager.ImportStatus != ProcessStatus.Restarted))
        {
            wzdInstaller.ActiveStepIndex = 7;
        }
        else
        {
            NextButton.Enabled = false;
        }
    }


    private void workerDB_OnFinished(object sender, EventArgs e)
    {
        CreateDBObjects = databaseDialog.CreateDatabaseObjects;

        DBInstalled = true;

        // Try to set connection string into db only if not running on Azure
        bool setConnectionString = !SystemContext.IsRunningOnAzure && writePermissions;

        // Connection string could not be saved to web.config
        if (!ConnectionHelper.IsConnectionStringInitialized && (!setConnectionString || !SettingsHelper.SetConnectionString(ConnectionHelper.ConnectionStringName, ConnectionString)))
        {
            ManualConnectionStringInsertion();
            return;
        }

        SetAppConnectionString();

        // Recalculate time zone daylight saving start and end.
        TimeZoneInfoProvider.GenerateTimeZoneRules();

        CheckLicense();
    }


    /// <summary>
    /// Check if license for current domain is valid. Try to add trial license if possible.
    /// </summary>
    private void CheckLicense()
    {
        // Add license keys
        bool licensesAdded = true;

        // Try to add trial license
        if (CreateDBObjects && (ucSiteCreationDialog.CreationType == CMSInstall_Controls_WizardSteps_SiteCreationDialog.CreationTypeEnum.Template))
        {
            licensesAdded = AddTrialLicenseKeys();
        }

        if (licensesAdded)
        {
            if ((hostName != "localhost") && (hostName != "127.0.0.1"))
            {
                // Check if license key for current domain is present
                LicenseKeyInfo lki = LicenseKeyInfoProvider.GetLicenseKeyInfo(hostName);
                wzdInstaller.ActiveStepIndex = (lki == null) ? 4 : 5;
            }
            else
            {
                wzdInstaller.ActiveStepIndex = 5;
            }
        }
        else
        {
            wzdInstaller.ActiveStepIndex = 4;
            ucLicenseDialog.SetLicenseExpired();
        }
    }


    /// <summary>
    /// Sets step, that prompts user to enter connection string manually to web.config. ConnectionString is built inside the method.
    /// </summary>
    private void ManualConnectionStringInsertion()
    {
        string encodedPassword = HttpUtility.HtmlEncode(HttpUtility.HtmlEncode(Password));
        string connectionString = ConnectionHelper.BuildConnectionString(AuthenticationType, userServer.ServerName, Database, userServer.DBUsername, encodedPassword, SqlInstallationHelper.DB_CONNECTION_TIMEOUT, isForAzure: SystemContext.IsRunningOnAzure);

        // Set error message
        string connectionStringEntry = "&lt;add name=\"CMSConnectionString\" connectionString=\"" + connectionString + "\"/&gt;";
        string applicationSettingsEntry = "&lt;Setting name=\"CMSConnectionString\" value=\"" + connectionString + "\"/&gt;";

        string errorMessage = SystemContext.IsRunningOnAzure ? string.Format(ResHelper.GetFileString("Install.ConnectionStringAzure"), connectionStringEntry, applicationSettingsEntry) : string.Format(ResHelper.GetFileString("Install.ConnectionStringError"), connectionStringEntry);
        lblErrorConnMessage.Text = errorMessage;

        // Set step that prompts user to enter connection string to web.config
        wzdInstaller.ActiveStepIndex = 2;

        if (!SystemContext.IsRunningOnAzure)
        {
            // Show troubleshoot link
            pnlError.DisplayError("Install.ErrorPermissions", HELP_TOPIC_DISK_PERMISSIONS_LINK);
        }
    }


    /// <summary>
    /// Sets the application connection string and initializes the application.
    /// </summary>
    private void SetAppConnectionString()
    {
        ConnectionHelper.ConnectionString = ConnectionString;
        dbReady = true;

        // Init core
        CMSApplication.Init();
    }


    /// <summary>
    /// Ensures required web.config keys.
    /// </summary>
    private void EnsureApplicationConfiguration()
    {
        // Ensure hash salt in web.config
        if (String.IsNullOrEmpty(ValidationHelper.GetDefaultHashStringSalt()))
        {
            SettingsHelper.SetConfigValue(ValidationHelper.APP_SETTINGS_HASH_STRING_SALT, Guid.NewGuid().ToString());
        }

        // Ensure application GUID in web.config
        if (String.IsNullOrEmpty(CoreServices.AppSettings[SystemHelper.APP_GUID_KEY_NAME]))
        {
            SettingsHelper.SetConfigValue(SystemHelper.APP_GUID_KEY_NAME, Guid.NewGuid().ToString());
        }
    }


    protected void wzdInstaller_PreviousButtonClick(object sender, WizardNavigationEventArgs e)
    {
        --StepIndex;
        wzdInstaller.ActiveStepIndex -= 1;
    }


    /// <summary>
    /// Adds trial license keys to DB. No license is added when running in web application gallery mode.
    /// </summary>
    private bool AddTrialLicenseKeys()
    {
        // Skip creation of trial license keys if running in WWAG mode
        if (ValidationHelper.GetBoolean(SettingsHelper.AppSettings[WWAG_KEY], false))
        {
            return false;
        }

        string licenseKey = ValidationHelper.GetString(SettingsHelper.AppSettings["CMSTrialKey"], String.Empty);
        if (licenseKey != String.Empty)
        {
            return LicenseHelper.AddTrialLicenseKeys(licenseKey, true, false);
        }

        pnlError.ErrorLabelText = ResHelper.GetFileString("Install.ErrorTrialLicense");

        return false;
    }


    /// <summary>
    /// Initialize wizard header
    /// </summary>
    /// <param name="index">Step index</param>
    private void InitializeHeader(int index)
    {
        Help.Visible = true;
        StartHelp.Visible = true;
        StartHelp.TopicName = Help.TopicName = HELP_TOPIC_LINK;

        lblHeader.Text = ResHelper.GetFileString("Install.Step") + " - ";

        string[] stepIcons = 
        {
            " icon-cogwheel",
            " icon-database",
            " icon-layout",
            " icon-check-circle icon-style-allow"
        };

        string[] stepTitles =
        {
            ResHelper.GetFileString("install.sqlsetting"),
            ResHelper.GetFileString("install.lbldatabase"),
            ResHelper.GetFileString("install.step5"),
            ResHelper.GetFileString("install.finishstep")
        };

        // Set common properties to each step icon
        for (var i = 0; i < stepIcons.Length; i++)
        {
            // Step panel
            var pnlStepIcon = new Panel();
            pnlStepIcon.ID = "stepPanel" + i;
            pnlStepIcon.CssClass = "install-step-panel";
            pnlHeaderImages.Controls.Add(pnlStepIcon);

            // Step icon
            var icon = new CMSIcon();
            icon.ID = "stepIcon" + i;
            icon.CssClass = "install-step-icon cms-icon-200" + stepIcons[i];
            icon.Attributes.Add("aria-hidden", "true");
            pnlStepIcon.Controls.Add(icon);

            // Step icon title
            var title = new HtmlGenericControl("title");
            title.ID = "stepTitle" + i;
            title.InnerText = stepTitles[i];
            title.Attributes.Add("class", "install-step-title");
            pnlStepIcon.Controls.Add(title);

            // Render separator only between step icons
            if (i < stepIcons.Length - 1)
            {
                // Separator panel
                var pnlSeparator = new Panel();
                pnlSeparator.ID = "separatorPanel" + i;
                pnlSeparator.CssClass = "install-step-icon-separator";
                pnlHeaderImages.Controls.Add(pnlSeparator);

                // Separator icon
                var separatorIcon = new CMSIcon();
                separatorIcon.CssClass = "icon-arrow-right cms-icon-150";
                separatorIcon.Attributes.Add("aria-hidden", "true");
                pnlSeparator.Controls.Add(separatorIcon);
            }
        }

        switch (index)
        {
            // SQL server and authentication mode
            case 0:
                {
                    lblHeader.Text += ResHelper.GetFileString("Install.Step0");
                    SetSelectedCSSClass("stepPanel0");
                    break;
                }
            // Database
            case 1:
                {
                    lblHeader.Text += ResHelper.GetFileString("Install.Step1");
                    SetSelectedCSSClass("stepPanel1");
                    break;
                }
            // web.config permissions
            case 2:
                {
                    StartHelp.Visible = Help.Visible = false;
                    lblHeader.Text += ResHelper.GetFileString("Install.Step3");
                    SetSelectedCSSClass("stepPanel1");
                    break;
                }

            // Database creation log
            case 3:
                {
                    StartHelp.Visible = Help.Visible = false;
                    lblHeader.Text += ResHelper.GetFileString("Install.Step2");
                    lblDBProgress.Text = ResHelper.GetFileString("Install.lblDBProgress");
                    SetSelectedCSSClass("stepPanel1");
                    break;
                }

            // License import
            case 4:
                {
                    lblHeader.Text += ResHelper.GetFileString("Install.Step4");
                    SetSelectedCSSClass("stepPanel1");
                    break;
                }

            // Starter site selection
            case 5:
                {
                    lblHeader.Text += ResHelper.GetFileString("Install.Step5");
                    SetSelectedCSSClass("stepPanel2");
                    break;
                }

            // Import log
            case 6:
                {
                    StartHelp.Visible = Help.Visible = false;
                    lblHeader.Text += ResHelper.GetFileString("Install.Step6");
                    SetSelectedCSSClass("stepPanel2");
                    break;
                }

            // Finish step
            case 7:
                {
                    lblHeader.Text += ResHelper.GetFileString("Install.Step7");
                    SetSelectedCSSClass("stepPanel3");
                    break;
                }

            case COLLATION_DIALOG_INDEX:
                {
                    lblHeader.Text += ResHelper.GetFileString("Install.Step8");
                    SetSelectedCSSClass("stepPanel1");
                    break;
                }
        }

        // Calculate step number
        if (PreviousStep == index)
        {
            StepOperation = 0;
        }
        ActualStep += StepOperation;
        lblHeader.Text = string.Format(lblHeader.Text, ActualStep + 1);
        PreviousStep = index;
    }


    private void SetSelectedCSSClass(string stepPanel)
    {
        var selectedPanel = pnlHeaderImages.FindControl(stepPanel) as Panel;
        selectedPanel.CssClass += " install-step-icon-selected";
    }


    private void EnsureDefaultButton()
    {
        if (wzdInstaller.ActiveStep != null)
        {
            Page.Form.DefaultButton = (wzdInstaller.ActiveStep.StepType == WizardStepType.Start) ? StartNextButton.UniqueID : NextButton.UniqueID;
        }
    }

    #endregion


    #region "Installation methods"

    public bool CreateDatabase(string collation)
    {
        try
        {
            string message = ResHelper.GetFileString("Installer.LogCreatingDatabase") + " " + databaseDialog.NewDatabaseName;
            AddResult(message);
            LogProgressState(LogStatusEnum.Info, message);

            string connectionString = ConnectionHelper.BuildConnectionString(AuthenticationType, userServer.ServerName, String.Empty, userServer.DBUsername, Password, SqlInstallationHelper.DB_CONNECTION_TIMEOUT);

            // Use default collation, if none specified
            if (String.IsNullOrEmpty(collation))
            {
                collation = DatabaseHelper.DatabaseCollation;
            }

            if (!DBCreated)
            {
                SqlInstallationHelper.CreateDatabase(databaseDialog.NewDatabaseName, connectionString, collation);
            }

            return true;
        }
        catch (Exception ex)
        {
            mDisplayLog = true;
            string message = ResHelper.GetFileString("Intaller.LogErrorCreateDB") + " " + ex.Message;
            AddResult(message);
            LogProgressState(LogStatusEnum.Error, message);
        }

        return false;
    }


    /// <summary>
    /// Logs message to install log.
    /// </summary>
    /// <param name="message">Message</param>
    /// <param name="type">Type of message ("E" - error, "I" - info)</param>
    public void Log(string message, MessageTypeEnum type)
    {
        AddResult(message);
        switch (type)
        {
            case MessageTypeEnum.Error:
                LogProgressState(LogStatusEnum.Error, message);
                break;

            case MessageTypeEnum.Info:
                LogProgressState(LogStatusEnum.Info, message);
                break;
        }
    }


    /// <summary>
    /// Installs database (table structure + default data).
    /// </summary>
    /// <param name="parameter">Async action param</param>
    private void InstallDatabase(object parameter)
    {
        if (!DBInstalled)
        {
            TryResetUninstallationTokens();

            SqlInstallationHelper.AfterDataGet += OnAfterGetDefaultData;

            var settings = new DatabaseInstallationSettings
            {
                ConnectionString = Info.ConnectionString,
                ScriptsFolder = Info.ScriptsFullPath,
                ApplyHotfix = true,
                DatabaseObjectInstallationErrorMessage = ResHelper.GetFileString("Installer.LogErrorCreateDBObjects"),
                DataInstallationErrorMessage = ResHelper.GetFileString("Installer.LogErrorDefaultData"),
                Logger = Log
            };
            bool success = SqlInstallationHelper.InstallDatabase(settings);

            SqlInstallationHelper.AfterDataGet -= OnAfterGetDefaultData;

            if (success)
            {
                LogProgressState(LogStatusEnum.Finish, ResHelper.GetFileString("Installer.DBInstallFinished"));
            }
            else
            {
                throw new Exception("[InstallDatabase]: Error during database creation.");
            }
        }
    }


    /// <summary>
    /// Tries to reset uninstallation tokens of installable modules.
    /// Logs the result into event log.
    /// </summary>
    private static void TryResetUninstallationTokens()
    {
        try
        {
            ModulesModule.ResetUninstallationTokensOfInstallableModules();
            CoreServices.EventLog.LogEvent("I", "Installation", "RESETUNINSTALLATIONTOKENS", "Uninstallation tokens of installable modules have been automatically reset due to database installation.");
        }
        catch (Exception ex)
        {
            CoreServices.EventLog.LogEvent("E", "Installation", "RESETUNINSTALLATIONTOKENS", String.Format("Reset of uninstallation tokens of installable modules has failed. The modules can not be installed again until their tokens are reset. To recover from such state, uninstall the modules and run the instance so it can recover (i.e. remove the uninstallation tokens for modules during application startup). Then install the modules again. {0}", ex));
        }
    }


    private void OnAfterGetDefaultData(object sender, DataSetPostProcessingEventArgs args)
    {
        // We use the default admin user name for installation as the users may not yet be ready and we count with administrator account to be installed
        MacroSecurityProcessor.RefreshSecurityParameters(args.Data, new MacroIdentityOption { IdentityName = MacroIdentityInfoProvider.DEFAULT_GLOBAL_ADMINISTRATOR_IDENTITY_NAME });
    }

    #endregion


    #region "Error handling methods"

    protected void HandleError(string message, WizardNavigationEventArgs e)
    {
        if (StepIndex > 1)
        {
            --StepIndex;
        }
        pnlError.ErrorLabelText = message;
        e.Cancel = true;
    }


    protected void HandleError(string message)
    {
        if (StepIndex > 1)
        {
            --StepIndex;
        }
        pnlError.ErrorLabelText = message;
    }


    protected void HandleError(string message, string resourceString, string topic)
    {
        if (StepIndex > 1)
        {
            --StepIndex;
        }
        pnlError.ErrorLabelText = message;
        pnlError.DisplayError(resourceString, topic);
    }

    #endregion


    #region "Logging methods"

    /// <summary>
    /// Appends the result string to the result message.
    /// </summary>
    /// <param name="result">String to append</param>
    public void AddResult(string result)
    {
        mResult = result + "\n" + mResult;
    }


    /// <summary>
    /// Logs progress state.
    /// </summary>
    /// <param name="type">Type of the message</param>
    /// <param name="message">Message to be logged</param>
    public void LogProgressState(LogStatusEnum type, string message)
    {
        message = HTMLHelper.HTMLEncode(message);

        string logMessage = null;
        string messageType = null;

        switch (type)
        {
            case LogStatusEnum.Info:
            case LogStatusEnum.Start:
                logMessage = message;
                break;

            case LogStatusEnum.Error:
                {
                    messageType = "##ERROR##";
                    logMessage = "<strong>" + ResHelper.GetFileString("Global.ErrorSign", "ERROR:") + "&nbsp;</strong>" + message;
                }
                break;

            case LogStatusEnum.Warning:
                {
                    messageType = "##WARNING##";
                    logMessage = "<strong>" + ResHelper.GetFileString("Global.Warning", "WARNING:") + "&nbsp;</strong>" + message;
                }
                break;

            case LogStatusEnum.Finish:
            case LogStatusEnum.UnexpectedFinish:
                logMessage = "<strong>" + message + "</strong>";
                break;
        }

        logMessage = messageType + logMessage;

        // Log to context
        Info.LogContext.AppendText(logMessage);
    }

    #endregion
}
