﻿using System;

using CMS.Base;

using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;

using CMS.Base.Web.UI;
using CMS.Base.Web.UI.ActionsConfig;
using CMS.DataEngine;
using CMS.EventLog;
using CMS.Globalization;
using CMS.HealthMonitoring;
using CMS.Helpers;
using CMS.IO;
using CMS.LicenseProvider;
using CMS.UIControls;
using CMS.URLRewritingEngine;
using CMS.WebFarmSync;
using CMS.WinServiceEngine;

public partial class CMSModules_System_Controls_System : CMSAdminControl
{
    #region "Request statistics"

    /// <summary>
    /// Container for request statistics
    /// </summary>
    private class RequestStatistics
    {
        private DateTime LastRPS = DateTime.MinValue;

        private long LastPageRequests;
        private long LastNonPageRequests;
        private long LastSystemPageRequests;
        private long LastPageNotFoundRequests;
        private long LastGetFileRequests;

        public double RPSPageRequests = -1;
        public double RPSNonPageRequests = -1;
        public double RPSSystemPageRequests = -1;
        public double RPSPageNotFoundRequests = -1;
        public double RPSGetFileRequests = -1;


        public void EvaluateRequests()
        {
            // Get values from counters
            long totalSystemRequests = RequestHelper.TotalSystemPageRequests.GetValue(null);
            long totalPageRequests = RequestHelper.TotalPageRequests.GetValue(null);
            long totalPageNotFoundRequests = RequestHelper.TotalPageNotFoundRequests.GetValue(null);
            long totalNonPageRequests = RequestHelper.TotalNonPageRequests.GetValue(null);
            long totalGetFileRequests = RequestHelper.TotalGetFileRequests.GetValue(null);

            // Reevaluate RPS
            if (LastRPS != DateTime.MinValue)
            {
                double seconds = DateTime.Now.Subtract(LastRPS).TotalSeconds;
                if ((seconds < 3) && (seconds > 0))
                {
                    RPSSystemPageRequests = (totalSystemRequests - LastSystemPageRequests) / seconds;
                    RPSPageRequests = (totalPageRequests - LastPageRequests) / seconds;
                    RPSPageNotFoundRequests = (totalPageNotFoundRequests - LastPageNotFoundRequests) / seconds;
                    RPSNonPageRequests = (totalNonPageRequests - LastNonPageRequests) / seconds;
                    RPSGetFileRequests = (totalGetFileRequests - LastGetFileRequests) / seconds;
                }
                else
                {
                    RPSGetFileRequests = -1;
                    RPSNonPageRequests = -1;
                    RPSPageNotFoundRequests = -1;
                    RPSPageRequests = -1;
                    RPSSystemPageRequests = -1;
                }
            }

            LastRPS = DateTime.Now;

            // Update last values
            LastGetFileRequests = totalGetFileRequests;
            LastNonPageRequests = totalNonPageRequests;
            LastPageNotFoundRequests = totalPageNotFoundRequests;
            LastPageRequests = totalPageRequests;
            LastSystemPageRequests = totalSystemRequests;
        }
    }

    #endregion


    #region "Variables"

    private static readonly RequestStatistics Statistics = new RequestStatistics();

    private bool isSeparatedDB;

    #endregion


    #region "Properties"

    /// <summary>
    /// Messages placeholder
    /// </summary>
    public override MessagesPlaceHolder MessagesPlaceHolder
    {
        get
        {
            return plcMess;
        }
    }


    /// <summary>
    /// Indicates if control is used on live site.
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
            plcMess.IsLiveSite = value;
        }
    }


    /// <summary>
    /// Gets or sets the refresh interval (in seconds) for the page. If set to 0 then no refresh.
    /// </summary>
    public int RefreshInterval
    {
        get
        {
            return timRefresh.Interval / 1000;
        }
        set
        {
            timRefresh.Interval = value * 1000;
            if (drpRefresh.Items.FindByValue(value.ToString()) != null)
            {
                drpRefresh.SelectedValue = value.ToString();
            }
        }
    }


    public bool WebFarmButtonVisible
    {
        get
        {
            // Hide button if webfarms are not enabled or disallow restarting webfarms for Microsoft Azure
            bool visible = WebFarmContext.WebFarmEnabled && !SystemContext.IsRunningOnAzure;

            // Hide the web farm restart button if this web farm server is not enabled
            if (!RequestHelper.IsPostBack() && !WebFarmContext.EnabledServers.Any(s => s.ServerName.EqualsCSafe(SystemContext.ServerName)))
            {
                visible = false;
            }

            return visible;
        }
    }

    #endregion


    #region "Page Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Init actions
        // Page view
        if (HeaderActions != null)
        {
            HeaderActions.ActionPerformed += HeaderActionsOnActionPerformed;
            InitHeaderActions();
        }
        // Widget view
        else
        {
            InitButtons();
        }

        timRefresh.Interval = 1000 * ValidationHelper.GetInteger(drpRefresh.SelectedValue, 0);
        isSeparatedDB = SqlInstallationHelper.DatabaseIsSeparated();

        // Check permissions
        RaiseOnCheckPermissions("READ", this);

        if (StopProcessing)
        {
            return;
        }

        Statistics.EvaluateRequests();

        // Do not count this page with async postback
        if (RequestHelper.IsAsyncPostback())
        {
            RequestHelper.TotalSystemPageRequests.Decrement(null);
        }

        lblServerName.Text = GetString("Administration-System.ServerName");
        lblServerVersion.Text = GetString("Administration-System.ServerVersion");
        lblDBName.Text = GetString("Administration-System.DatabaseName");
        lblDBSize.Text = GetString("Administration-System.DatabaseSize");
        lblMachineName.Text = GetString("Administration-System.MachineName");
        lblMachineNameValue.Text = HTMLHelper.HTMLEncode(SystemContext.MachineName);
        lblAspAccount.Text = GetString("Administration-System.Account");
        lblValueAspAccount.Text = HTMLHelper.HTMLEncode(WindowsIdentity.GetCurrent().Name);
        plcSeparatedName.Visible = isSeparatedDB;
        plcSeparatedServerName.Visible = isSeparatedDB;
        plcSeparatedSize.Visible = isSeparatedDB;
        plcSeparatedVersion.Visible = isSeparatedDB;
        plcSeparatedHeader.Visible = isSeparatedDB;

        lblAspVersion.Text = GetString("Administration-System.Version");
        lblValueAspVersion.Text = HTMLHelper.HTMLEncode(Environment.Version.ToString());

        lblAlocatedMemory.Text = GetString("Administration-System.Memory");
        lblPeakMemory.Text = GetString("Administration-System.PeakMemory");
        lblVirtualMemory.Text = GetString("Administration-System.VirtualMemory");
        lblPhysicalMemory.Text = GetString("Administration-System.PhysicalMemory");
        lblIP.Text = GetString("Administration-System.IP");

        lblPageViewsValues.Text = GetString("Administration-System.PageViewsValues");

        lblPages.Text = GetString("Administration-System.Pages");
        lblSystemPages.Text = GetString("Administration-System.SystemPages");
        lblNonPages.Text = GetString("Administration-System.NonPages");
        lblGetFilePages.Text = GetString("Administration-System.GetFilePages");
        lblPagesNotFound.Text = GetString("Administration-System.PagesNotFound");
        lblPending.Text = GetString("Administration-System.Pending");

        lblCacheExpired.Text = GetString("Administration-System.CacheExpired");
        lblCacheRemoved.Text = GetString("Administration-System.CacheRemoved");
        lblCacheUnderused.Text = GetString("Administration-System.CacheUnderused");
        lblCacheItems.Text = GetString("Administration-System.CacheItems");
        lblCacheDependency.Text = GetString("Administration-System.CacheDependency");

        LoadData();

        if (!RequestHelper.IsPostBack())
        {
            switch (QueryHelper.GetString("lastaction", String.Empty).ToLowerCSafe())
            {
                case "restarted":
                    ShowConfirmation(GetString("Administration-System.RestartSuccess"));
                    break;

                case "webfarmrestarted":
                    if (ValidationHelper.ValidateHash("WebfarmRestarted", QueryHelper.GetString("restartedhash", String.Empty), new HashSettings("")))
                    {
                        ShowConfirmation(GetString("Administration-System.WebframRestarted"));
                        // Restart other servers - create webfarm task for application restart
                        WebFarmHelper.CreateTask(new RestartApplicationWebFarmTask());
                    }
                    else
                    {
                        ShowError(GetString("general.actiondenied"));
                    }
                    break;

                case "countercleared":
                    ShowConfirmation(GetString("Administration-System.CountersCleared"));
                    break;

                case "winservicesrestarted":
                    ShowConfirmation(GetString("Administration-System.WinServicesRestarted"));
                    break;
            }
        }

        lblRunTime.Text = GetString("Administration.System.RunTime");
        lblServerTime.Text = GetString("Administration.System.ServerTime");

        // Remove milliseconds from the end of the time string
        string timeSpan = (DateTime.Now - CMSApplication.ApplicationStart).ToString();
        int index = timeSpan.LastIndexOfCSafe('.');
        if (index >= 0)
        {
            timeSpan = timeSpan.Remove(index);
        }

        // Display application run time
        lblRunTimeValue.Text = timeSpan;
        lblServerTimeValue.Text = Convert.ToString(DateTime.Now) + " " + TimeZoneHelper.GetUTCStringOffset(TimeZoneHelper.ServerTimeZone);

        lblIPValue.Text = HTMLHelper.HTMLEncode(RequestContext.UserHostAddress);
    }


    /// <summary>
    /// OnPreRender event handler
    /// </summary>
    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        if (timRefresh.Interval > 0)
        {
            timRefresh.Enabled = true;
        }
    }

    #endregion


    #region "Header actions methods"

    /// <summary>
    /// Initializes header actions
    /// </summary>
    private void InitHeaderActions()
    {
        HeaderActions.AddAction(new HeaderAction
        {
            Text = GetString("Administration-System.btnRestart"),
            ButtonStyle = ButtonStyle.Primary,
            CommandName = "Restart",
        });

        HeaderActions.AddAction(new HeaderAction
        {
            Text = GetString("Administration-System.btnRestartWebfarm"),
            ButtonStyle = ButtonStyle.Primary,
            CommandName = "RestartWebFarm",
            Visible = WebFarmButtonVisible,
        });

        HeaderActions.AddAction(new HeaderAction
        {
            Text = GetString("Administration-System.btnClearCache"),
            ButtonStyle = ButtonStyle.Primary,
            CommandName = "ClearCache",
        });

        HeaderActions.AddAction(new HeaderAction
        {
            Text = GetString("Administration-System.btnClearCounters"),
            ButtonStyle = ButtonStyle.Default,
            CommandName = "ClearCounters",
            Visible = LicenseHelper.CheckFeature(RequestContext.CurrentDomain, FeatureEnum.HealthMonitoring),
        });

        HeaderActions.AddAction(new HeaderAction
        {
            Text = GetString("Administration-System.btnClear"),
            ButtonStyle = ButtonStyle.Default,
            CommandName = "ClearMemory",
        });

        HeaderActions.AddAction(new HeaderAction
        {
            Text = GetString("Administration-System.btnRestartServices"),
            ButtonStyle = ButtonStyle.Default,
            CommandName = "RestartServices",
            Visible = WinServiceHelper.ServicesAvailable(),
        });
    }


    /// <summary>
    /// Initializes button actions
    /// </summary>
    private void InitButtons()
    {
        btnClear.Visible = true;
        btnClearCache.Visible = true;
        btnRestart.Visible = true;
        btnRestartWebfarm.Visible = WebFarmButtonVisible;
        btnRestartServices.Visible = WinServiceHelper.ServicesAvailable();
        btnClearCounters.Visible = LicenseHelper.CheckFeature(RequestContext.CurrentDomain, FeatureEnum.HealthMonitoring);
    }


    /// <summary>
    /// Proccess header action click.
    /// </summary>
    private void HeaderActionsOnActionPerformed(object sender, CommandEventArgs commandEventArgs)
    {
        switch (commandEventArgs.CommandName)
        {
            case "Restart":
                Restart();
                break;
            case "RestartWebFarm":
                RestartWebfarm();
                break;
            case "ClearCache":
                ClearCache();
                break;
            case "ClearCounters":
                ClearCounters();
                break;
            case "ClearMemory":
                ClearMemory();
                break;
            case "RestartServices":
                RestartServices();
                break;
        }
    }


    /// <summary>
    /// Clear unused memory.
    /// </summary>
    protected void ClearMemory(object sender = null, EventArgs args = null)
    {
        if (StopProcessing)
        {
            return;
        }

        // Collect the memory
        GC.Collect();
        GC.WaitForPendingFinalizers();

        // Log event
        EventLogProvider.LogEvent(EventType.INFORMATION, "System", "CLEARMEM", GetString("Administration-System.ClearSuccess"));

        ShowConfirmation(GetString("Administration-System.ClearSuccess"));

        LoadData();
    }


    /// <summary>
    /// Restart application.
    /// </summary>
    protected void Restart(object sender = null, EventArgs args = null)
    {
        if (StopProcessing)
        {
            return;
        }

        if (SystemHelper.RestartApplication())
        {
            if (SystemContext.IsRunningOnAzure)
            {
                // Log event
                EventLogProvider.LogEvent(EventType.INFORMATION, "System", "ENDAPP", GetString("admin.system.restartazure"));
                ShowConfirmation(GetString("admin.system.restartazure"));
            }
            else
            {
                // Log event
                EventLogProvider.LogEvent(EventType.INFORMATION, "System", "ENDAPP", GetString("Administration-System.RestartSuccess"));

                string url = URLHelper.UpdateParameterInUrl(RequestContext.CurrentURL, "lastaction", "Restarted");
                URLHelper.Redirect(url);
            }
        }
        else
        {
            ShowError(GetString("System.Restart.Failed"));
        }
    }


    /// <summary>
    /// Restart webfarm.
    /// </summary>
    protected void RestartWebfarm(object sender = null, EventArgs args = null)
    {
        if (StopProcessing)
        {
            return;
        }

        // Restart current server
        SystemHelper.RestartApplication();

        // Log event
        EventLogProvider.LogEvent(EventType.INFORMATION, "System", "RESTARTWEBFARM", GetString("Administration-System.WebframRestarted"));

        string url = URLHelper.UpdateParameterInUrl(RequestContext.CurrentURL, "lastaction", "WebfarmRestarted");
        url = URLHelper.UpdateParameterInUrl(url, "restartedhash", ValidationHelper.GetHashString("WebfarmRestarted", new HashSettings("")));
        URLHelper.Redirect(url);
    }


    /// <summary>
    /// Clear counters.
    /// </summary>
    protected void ClearCounters(object sender = null, EventArgs args = null)
    {
        if (StopProcessing)
        {
            return;
        }

        try
        {
            // Reset values of health monitoring counters
            HealthMonitoringManager.ResetCounters();
            // Clear application counters
            HealthMonitoringLogHelper.ClearApplicationCounters();

            // Log event
            EventLogProvider.LogEvent(EventType.INFORMATION, "System", "CLEARCOUNTERS", GetString("Administration-System.CountersCleared"));

            string url = URLHelper.UpdateParameterInUrl(RequestContext.CurrentURL, "lastaction", "CounterCleared");
            URLHelper.Redirect(url);
        }
        catch (Exception ex)
        {
            // ThreadAbortException is thrown when response is ended (redirect)
            if (!(ex is ThreadAbortException))
            {
                LogAndShowError("System", "CLEARCOUNTERS", ex);
            }
        }
    }


    /// <summary>
    /// Restart windows services.
    /// </summary>
    protected void RestartServices(object sender = null, EventArgs args = null)
    {
        if (StopProcessing)
        {
            return;
        }

        // Resets values of counters
        try
        {
            WinServiceHelper.RestartService(null, true);

            // Log event
            EventLogProvider.LogEvent(EventType.INFORMATION, "System", "RESTARTWINSERVICES", GetString("Administration-System.WinServicesRestarted"));
        }
        catch (Exception ex)
        {
            EventLogProvider.LogException("WinServiceHelper", "RestartService", ex);
        }

        string url = URLHelper.UpdateParameterInUrl(RequestContext.CurrentURL, "lastaction", "WinServicesRestarted");
        URLHelper.Redirect(url);
    }


    protected void ClearCache(object sender = null, EventArgs args = null)
    {
        if (StopProcessing)
        {
            return;
        }

        // Clear the cache
        CacheHelper.ClearCache(null, true);
        Functions.ClearHashtables();

        // Drop the routes
        CMSDocumentRouteHelper.DropAllRoutes();

        // Disposes all zip files
        ZipStorageProvider.DisposeAll();

        // Collect the memory
        GC.Collect();
        GC.WaitForPendingFinalizers();

        // Log event
        EventLogProvider.LogEvent(EventType.INFORMATION, "System", "CLEARCACHE", GetString("Administration-System.ClearCacheSuccess"));

        ShowConfirmation(GetString("Administration-System.ClearCacheSuccess"));

        LoadData();
    }

    #endregion


    #region "Other Methods"

    /// <summary>
    /// Loads the data.
    /// </summary>
    protected void LoadData()
    {
        lblValueAlocatedMemory.Text = DataHelper.GetSizeString(GC.GetTotalMemory(false));

        lblValueVirtualMemory.Text = "N/A";
        lblValuePhysicalMemory.Text = "N/A";
        lblValuePeakMemory.Text = "N/A";

        // Process memory
        try
        {
            lblValueVirtualMemory.Text = DataHelper.GetSizeString(SystemHelper.GetVirtualMemorySize());
            lblValuePhysicalMemory.Text = DataHelper.GetSizeString(SystemHelper.GetWorkingSetSize());
            lblValuePeakMemory.Text = DataHelper.GetSizeString(SystemHelper.GetPeakWorkingSetSize());
        }
        catch
        {
        }

        lblValuePages.Text = GetViewValues(RequestHelper.TotalPageRequests.GetValue(null), Statistics.RPSPageRequests);
        lblValuePagesNotFound.Text = GetViewValues(RequestHelper.TotalPageNotFoundRequests.GetValue(null), Statistics.RPSPageNotFoundRequests);
        lblValueSystemPages.Text = GetViewValues(RequestHelper.TotalSystemPageRequests.GetValue(null), Statistics.RPSSystemPageRequests);
        lblValueNonPages.Text = GetViewValues(RequestHelper.TotalNonPageRequests.GetValue(null), Statistics.RPSNonPageRequests);
        lblValueGetFilePages.Text = GetViewValues(RequestHelper.TotalGetFileRequests.GetValue(null), Statistics.RPSGetFileRequests);

        long pending = RequestHelper.PendingRequests.GetValue(null);
        if (pending > 1)
        {
            // Current request does not count as pending at the time of display
            pending--;
        }
        if (pending < 0)
        {
            pending = 0;
        }

        lblValuePending.Text = pending.ToString();

        lblCacheItemsValue.Text = Cache.Count.ToString();
        lblCacheExpiredValue.Text = CacheHelper.Expired.GetValue(null).ToString();
        lblCacheRemovedValue.Text = CacheHelper.Removed.GetValue(null).ToString();
        lblCacheUnderusedValue.Text = CacheHelper.Underused.GetValue(null).ToString();
        lblCacheDependencyValue.Text = CacheHelper.DependencyChanged.GetValue(null).ToString();

        // GC collections
        try
        {
            plcGC.Controls.Clear();

            int generations = GC.MaxGeneration;
            for (int i = 0; i <= generations; i++)
            {
                int count = GC.CollectionCount(i);
                string genString = "<div class=\"form-group\"><div class=\"editing-form-label-cell\"><span class=\"control-label\">" + GetString("GC.Generation") + " " + i +
                                   ":</span></div><div class=\"editing-form-value-cell\"><span class=\"form-control-text\">" + count + "</span></div></div>";

                plcGC.Controls.Add(new LiteralControl(genString));
            }
        }
        catch
        {
        }

        // DB information
        if (!RequestHelper.IsPostBack())
        {
            TableManager tm = new TableManager(null);

            lblDBNameValue.Text = HTMLHelper.HTMLEncode(tm.DatabaseName);
            lblServerVersionValue.Text = HTMLHelper.HTMLEncode(tm.DatabaseServerVersion);
            lblDBSizeValue.Text = HTMLHelper.HTMLEncode(tm.DatabaseSize);
            lblServerNameValue.Text = HTMLHelper.HTMLEncode(tm.DatabaseServerName);

            // Check if DB is separated
            bool separated = SqlInstallationHelper.DatabaseIsSeparated();
            if (separated)
            {
                tm = new TableManager(DatabaseSeparationHelper.OM_CONNECTION_STRING);

                lblSeparatedServerName.Text = HTMLHelper.HTMLEncode(tm.DatabaseServerName);
                lblSeparatedVersion.Text = HTMLHelper.HTMLEncode(tm.DatabaseServerVersion);
                lblSeparatedName.Text = HTMLHelper.HTMLEncode(tm.DatabaseName);
                lblSeparatedSize.Text = HTMLHelper.HTMLEncode(tm.DatabaseSize);
            }
        }
    }


    /// <summary>
    /// Gets the values string for the page views.
    /// </summary>
    private string GetViewValues(long total, double rps)
    {
        return total + ((rps >= 1) ? " (" + Math.Floor(rps) + " RPS)" : "");
    }

    #endregion
}