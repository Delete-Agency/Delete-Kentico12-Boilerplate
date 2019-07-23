﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using CMS.Activities;
using CMS.Base.Web.UI;
using CMS.Base.Web.UI.Internal;
using CMS.Core;
using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.Globalization;
using CMS.Helpers;
using CMS.MacroEngine;
using CMS.Membership;
using CMS.Modules;
using CMS.SiteProvider;
using CMS.UIControls;
using CMS.WebAnalytics;
using CMS.WebAnalytics.Web.UI;


[EditedObject(CampaignInfo.OBJECT_TYPE, "objectid")]
[UIElement(ModuleName.WEBANALYTICS, "Campaign.General", false, true)]
public partial class CMSModules_WebAnalytics_Pages_Tools_Campaign_Tab_General : CMSCampaignPage
{
    private const string HELP_TOPIC_TRACKING_CAMPAIGNS_LINK = "campaigns_managing";

    private const string SMART_TIP_PROMOTION_IDENTIFIER = "campaigns|general|promotion";
    private const string SMART_TIP_LAUNCH_IDENTIFIER = "campaigns|general|launch";

    private readonly string[] mAllowedActivities =
    {
        PredefinedActivityType.PAGE_VISIT,
        PredefinedActivityType.BIZFORM_SUBMIT,
        PredefinedActivityType.PURCHASE,
        PredefinedActivityType.PURCHASEDPRODUCT,
        PredefinedActivityType.REGISTRATION,
        PredefinedActivityType.EVENT_BOOKING,
        PredefinedActivityType.INTERNAL_SEARCH,
        PredefinedActivityType.PRODUCT_ADDED_TO_SHOPPINGCART,
        PredefinedActivityType.NEWSLETTER_SUBSCRIBING
    };

    private UserSmartTipDismissalManager mSmartTipManager = new UserSmartTipDismissalManager(MembershipContext.AuthenticatedUser);

    private Uri mPresentationUri;


    /// <summary>
    /// Current site presentation URL processed to URI.
    /// </summary>
    private Uri SitePresentationUri
    {
        get
        {
            if (mPresentationUri == null && SiteContext.CurrentSite.SiteIsContentOnly)
            {
                Uri.TryCreate(SiteContext.CurrentSite.SitePresentationURL, UriKind.Absolute, out mPresentationUri);
            }
            return mPresentationUri;
        }
    }


    /// <summary>
    /// Prepares regex that will validate user input for <see cref="CampaignAssetUrlInfo.CampaignAssetUrlTarget"/>.
    /// Checks that inserted URL belongs to content only site.
    /// </summary>
    private string PrepareUrlAssetTargetRegex()
    {
        if (SitePresentationUri == null)
        {
            return null;
        }

        var siteDomain = SitePresentationUri.Host.StartsWith("www.", StringComparison.InvariantCultureIgnoreCase) ? SitePresentationUri.Host.Remove(0, 4) : SitePresentationUri.Host;

        var campaignUrlAssetTargetInputRegex = @"^(http(s?)://)([-.\w]*" + siteDomain + @")(:[0-9]*)*";
        campaignUrlAssetTargetInputRegex += SitePresentationUri.AbsolutePath.TrimEnd('/');
        campaignUrlAssetTargetInputRegex += @"(|\/[^\?\&\#]*)$";

        return campaignUrlAssetTargetInputRegex;
    }


    /// <summary>
    /// Initializes selection modal dialog.
    /// </summary>
    /// <param name="objectType">Object type</param>
    /// <param name="cliendId">Client ID</param>
    private string GetModalDialogUrl(string objectType, string cliendId)
    {
        var guid = Guid.NewGuid().ToString();
        WindowHelper.Add(guid, new Hashtable
        {
            {"AllowAll", false},
            {"AllowEmpty", false},
            {"AllowDefault", true},
            {"LocalizeItems", true},
            {"ObjectType", objectType},
            {"SelectionMode", SelectionModeEnum.SingleButton},
            {"ResourcePrefix", objectType},
            {"FilterControl", "~/CMSFormControls/Filters/ObjectFilter.ascx"},
            {"CurrentSiteOnly", true}
        });

        var url = URLHelper.GetAbsoluteUrl("~/CMSAdminControls/UI/UniSelector/SelectionDialog.aspx");
        url = URLHelper.AddParameterToUrl(url, "clientId", cliendId);
        url = URLHelper.AddParameterToUrl(url, "params", guid);
        return URLHelper.AddParameterToUrl(url, "hash", QueryHelper.GetHash(url));
    }


    /// <summary>
    /// Prepares JSON object to be inserted to the breadcrumbs. This object will be used when updating breadcrumbs after changing display name of the campaign.
    /// </summary>
    /// <returns>List of objects containing breadcrumb for root element and single campaign.</returns>
    private object GetBreadcrumbsData()
    {
        var breadcrumbsList = new List<object>();
        var application = UIContext.UIElement.Application;

        // Root application
        string rootRedirectUrl = UrlResolver.ResolveUrl(ApplicationUrlHelper.GetApplicationUrl(application));
        breadcrumbsList.Add(new
        {
            text = MacroResolver.Resolve(application.ElementDisplayName),
            redirectUrl = rootRedirectUrl,
            isRoot = true
        });

        // (Campaign)
        breadcrumbsList.Add(new
        {
            suffix = GetString("analytics.campaign")
        });


        return new
        {
            data = breadcrumbsList,
            pin = new
            {
                elementGuid = UIElementInfoProvider.GetUIElementInfo(UIContext.UIElement.ElementParentID).ElementGUID,
                applicationGuid = application.ElementGUID,
                objectType = CampaignInfo.OBJECT_TYPE
            }
        };
    }


    protected void Page_Init(object sender, EventArgs e)
    {
        ScriptHelper.RegisterTooltip(this);

        RegisterAngularModule();

        ScriptHelper.RegisterDialogScript(this);
        CssRegistration.RegisterCssLink(this, "~/CMSScripts/jquery/jqueryui/jquery-ui.css");

        if (SiteContext.CurrentSite.SiteIsContentOnly && SitePresentationUri == null)
        {
            ShowError(GetString("campaign.incorrectPresentationUrl"));
            MessagesPlaceHolder.ContainerCssClass = CssHelper.EnsureClass(MessagesPlaceHolder.ContainerCssClass, "cms-campaigns-edit");
        }
    }

    private void RegisterAngularModule()
    {
        var moduleId = "CMS.WebAnalytics/Campaign/build";
        var localizationProvider = Service.Resolve<IClientLocalizationProvider>();
        var resources = localizationProvider.GetClientLocalization(moduleId);

        var campaign = new CampaignEditViewModel(GetCampaignInfo(), DateTime.Now);

        ScriptHelper.RegisterAngularModule(moduleId, new {
            Resources = resources,
            campaign = campaign,
            assets = GetAssets(campaign.CampaignID),
            conversions = GetConversions(campaign.CampaignID),
            objective = GetObjective(campaign.CampaignID),
            activityTypes = GetActivityTypes(),
            Breadcrumbs = GetBreadcrumbsData(),
            EmailRegexp = ValidationHelper.EmailRegExp.ToString(),
            IsNewsletterModuleLoaded = ModuleEntryManager.IsModuleLoaded(ModuleName.NEWSLETTER),
            SiteIsContentOnly = SiteContext.CurrentSite.SiteIsContentOnly,
            UrlAssetTargetRegex = PrepareUrlAssetTargetRegex(),
            SmartTips = PrepareSmartTips(),
            CreatePageDialogUrl = GetCreateNewPageDialogUrl(),
            SelectEmailDialogUrl = GetModalDialogUrl("newsletter.issue", "campaignAssetEmail")
        });
    }

    private object GetObjective(int campaignID)
    {
        var service = Service.Resolve<ICampaignObjectiveService>();
        return CampaignObjectiveInfoProvider.GetCampaignObjectives()
            .WhereEquals("CampaignObjectiveCampaignID", campaignID)
            .ToList()
            .Select(service.GetObjectiveViewModel)
            .FirstOrDefault();
    }


    /// <summary>
    /// Returns URL of modal dialog for creating new page.
    /// </summary>
    private string GetCreateNewPageDialogUrl()
    {
        var path = SettingsKeyInfoProvider.GetValue("CMSCampaignNewPageLocation", SiteContext.CurrentSiteName);
        var tree = new TreeProvider(MembershipContext.AuthenticatedUser);
        var node = tree.SelectSingleNode(SiteContext.CurrentSiteName, path, SiteContext.CurrentSite.DefaultVisitorCulture, true);

        if (node == null)
        {
            node = tree.SelectSingleNode(SiteContext.CurrentSiteName, "/", SiteContext.CurrentSite.DefaultVisitorCulture, true);
        }

        // General url settings
        var settings = new UIPageURLSettings
        {
            AllowSplitview = false,
            NodeID = node.NodeID,
            Culture = node.DocumentCulture,
            Action = "new",
            AdditionalQuery = "dialog=1&action=new&hidecontentonly=true"
        };

        return DocumentUIHelper.GetDocumentPageUrl(settings);
    }


    /// <summary>
    /// Gets the campaign info we are working with. Either edited or being newly created.
    /// </summary>
    private CampaignInfo GetCampaignInfo()
    {
        var campaignInfo = (EditedObject as CampaignInfo) ?? new CampaignInfo { CampaignSiteID = SiteContext.CurrentSiteID };

        // Add default display name for new campaign
        if (String.IsNullOrEmpty(campaignInfo.CampaignDisplayName))
        {
            var currentCulture = CultureHelper.GetCultureInfo(CultureCode);
            var currentSite = SiteContext.CurrentSite;
            var currentDateTimeFormatted = TimeZoneHelper.GetSiteDateTime(currentSite).ToString(currentCulture.DateTimeFormat.ShortDatePattern.Replace("'", ""));

            campaignInfo.CampaignDisplayName = String.Format("{0} – {1}", GetString("campaign.defaultname"), currentDateTimeFormatted);
        }

        // Add default UTM code for new campaign
        if (String.IsNullOrEmpty(campaignInfo.CampaignUTMCode))
        {
            campaignInfo.CampaignUTMCode = campaignInfo.Generalized.GetUniqueName(GetString("campaign.defaultutmcode"), campaignInfo.CampaignID, "CampaignUTMCode", "_{0}", "[_](\\d+)$", false);
        }

        return campaignInfo;
    }


    private Dictionary<string, object> PrepareSmartTips()
    {
        var smartTips = new Dictionary<string, object>
        {
            {"promotionSmartTip", PreparePromotingCampaignsSmartTip() },
            {"launchSmartTip", PrepareLaunchSectionSmartTip() }
        };

        return smartTips;
    }


    private object PreparePromotingCampaignsSmartTip()
    {
        var documentationLink = DocumentationHelper.GetDocumentationTopicUrl(HELP_TOPIC_TRACKING_CAMPAIGNS_LINK);
        var smartTipContent = String.Format(GetString("campaigns.promotionsmarttip"), documentationLink);

        return new
        {
            Identifier = SMART_TIP_PROMOTION_IDENTIFIER,
            Content = smartTipContent,
            ExpandedHeader = GetString("campaigns.promotionsmarttip.header"),
            CollapsedHeader = GetString("campaigns.promotionsmarttip.header"),
            IsCollapsed = mSmartTipManager.IsSmartTipDismissed(SMART_TIP_PROMOTION_IDENTIFIER)
        };
    }


    private object PrepareLaunchSectionSmartTip()
    {
        return new
        {
            Identifier = SMART_TIP_LAUNCH_IDENTIFIER,
            Content = GetString("campaigns.launchsmarttip"),
            ExpandedHeader = GetString("campaigns.launchsmarttip.header"),
            CollapsedHeader = GetString("campaigns.launchsmarttip.header"),
            IsCollapsed = mSmartTipManager.IsSmartTipDismissed(SMART_TIP_LAUNCH_IDENTIFIER)
        };
    }


    private Dictionary<int, CampaignAssetViewModel> GetAssets(int campaignId)
    {
        var service = Service.Resolve<ICampaignAssetModelService>();
        return CampaignAssetInfoProvider.GetCampaignAssets()
            .WhereEquals("CampaignAssetCampaignID", campaignId)
            .ToList()
            .Select(x => service.GetStrategy(x.CampaignAssetType).GetAssetViewModel(x))
            .ToDictionary(x => x.AssetID);
    }


    private IEnumerable<CampaignConversionViewModel> GetConversions(int campaignId)
    {
        var service = Service.Resolve<ICampaignConversionService>();
        return CampaignConversionInfoProvider.GetCampaignConversions()
            .WhereEquals("CampaignConversionCampaignID", campaignId)
            .OrderBy("CampaignConversionOrder")
            .ToList()
            .Select(service.GetConversionViewModel);
    }


    private IEnumerable<ActivityTypeViewModel> GetActivityTypes()
    {
        var service = Service.Resolve<IActivityTypeService>();
        return service.GetActivityTypeViewModels(mAllowedActivities, SiteContext.CurrentSite.SiteIsContentOnly);
    }
}
