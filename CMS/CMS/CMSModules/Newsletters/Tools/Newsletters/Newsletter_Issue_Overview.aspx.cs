﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Web.UI.WebControls;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.ContactManagement.Web.UI.Internal;
using CMS.Core;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.LicenseProvider;
using CMS.Modules;
using CMS.Newsletters;
using CMS.PortalEngine;
using CMS.PortalEngine.Internal;
using CMS.UIControls;
using CMS.WebAnalytics;


[UIElement(ModuleName.NEWSLETTER, "Newsletter.Issue.Reports.Overview")]
[Title("general.overview")]
[EditedObject(IssueInfo.OBJECT_TYPE, "objectid")]
public partial class CMSModules_Newsletters_Tools_Newsletters_Newsletter_Issue_Overview : CMSDeskPage
{
    #region "Constants"

    private const string CAMPAIGN_ELEMENT_CODENAME = "CampaignProperties";
    private const string PERCENT_FORMAT = "{0:f2}%";
    private const string RATE_TEXT_FORMAT = "{0}: {1:f2}%";

    #endregion


    #region "Variables"

    private IssueInfo mIssue;
    private bool mMonitorBouncedEmails;
    private IList<int> mParentAndVariantsIDs;
    private int? mTotalClicks;
    private int? mUniqueClicks;
    private int? mOpenedEmails;

    #endregion


    #region "Properties"

    private int TotalClicks
    {
        get
        {
            if (mTotalClicks == null)
            {
                mTotalClicks = CalculateTotalClicks();
            }

            return mTotalClicks.Value;
        }
    }


    private int UniqueClicks
    {
        get
        {
            if (mUniqueClicks == null)
            {
                mUniqueClicks = CalculateUniqueClicks();
            }

            return mUniqueClicks.Value;
        }
    }


    private int OpenedEmails
    {
        get
        {
            if (mOpenedEmails == null)
            {
                mOpenedEmails = CalculateOpenedEmails();
            }

            return mOpenedEmails.Value;
        }
    }

    #endregion


    #region "Page Events"

    protected void Page_Load(object sender, EventArgs e)
    {
        // Prepare object data
        mIssue = (IssueInfo)EditedObject;
        mMonitorBouncedEmails = NewsletterHelper.MonitorBouncedEmails(CurrentSiteName);
        issueLinks.IssueID = mIssue.IssueID;
        issueLinks.NoDataText = GetString("newsletter.issue.overviewnoclicks");

        mParentAndVariantsIDs = IssueInfoProvider.GetIssues()
            .Column("IssueID")
            .WhereEquals("IssueVariantOfIssueID", mIssue.IssueID)
            .GetListResult<int>();

        mParentAndVariantsIDs.Add(mIssue.IssueID);       
        
        if (!ShouldDisplayOverviewPage())
        {
            ShowInformation(GetString("newsletter.issue.overviewnotsentyet"));
            pnlContent.Visible = false;
            return;
        }

        if (!mMonitorBouncedEmails)
        {
            ShowInformation(GetString("newsletter.viewadditionalstatsmessage"));
        }

        // Hide default submit button
        issueForm.SubmitButton.Visible = false;

        // Set campaign link
        if (issueForm.FieldControls.Contains("IssueUTMCampaign"))
        {
            bool areCampaignsAvailable = LicenseKeyInfoProvider.IsFeatureAvailable(RequestContext.CurrentDomain, FeatureEnum.CampaignAndConversions);

            issueForm.FieldControls["IssueUTMCampaign"].Value = 
                areCampaignsAvailable ? 
                GetUtmCampaignNameHtmlOutput() : 
                mIssue.IssueUTMCampaign;
        }
     
        // Init engagement statistics
        InitEngagement();
        InitFunnel();
        InitOverview();
        
        pnlDelivery.Visible = mMonitorBouncedEmails;

        // Ensure tooltips
        RegisterTooltipScript();
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        HandleBreadcrumbsScripts();
    }


    protected void lnkAllLinks_OnClick(object sender, EventArgs e)
    {
        // Redirect to clicks tab
        string url = UIContextHelper.GetElementUrl(ModuleName.NEWSLETTER, "EditIssueProperties", false, mIssue.IssueID, "tabname=Newsletter.Issue.Reports.Clicks");
        URLHelper.Redirect(url);
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Handles manual rendering of breadcrumbs.
    /// On this page the breadcrumbs needs to be hard-coded in order to be able to access single email via link and ensure consistency of breadcrumbs.
    /// </summary>
    private void HandleBreadcrumbsScripts()
    {
        ScriptHelper.RegisterRequireJs(Page);

        ControlsHelper.RegisterClientScriptBlock(this, Page, typeof(string), "BreadcrumbsOverwriting", ScriptHelper.GetScript(@"
        cmsrequire(['CMS/EventHub'], function(hub) {
              hub.publish('OverwriteBreadcrumbs', " + IssueHelper.GetBreadcrumbsData((IssueInfo)EditedObject) + @");
        });"));
    }


    private bool ShouldDisplayOverviewPage()
    {
        return ((mIssue.IssueStatus == IssueStatusEnum.Finished) 
             || (mIssue.IssueStatus == IssueStatusEnum.Sending) 
             || (mIssue.IssueStatus == IssueStatusEnum.TestPhase));
    }


    private void InitOverview()
    {
        if (mIssue.IssueIsABTest)
        {
            var abTest = ABTestInfoProvider.GetABTestInfoForIssue(mIssue.IssueID);

            if (abTest == null)
            {
                return;
            }

            var winnerIssueId = ValidationHelper.GetInteger(abTest.TestWinnerIssueID, 0);

            if (winnerIssueId == 0)
            {
                return;
            }

            var issueInForm = issueForm.EditedObject as IssueInfo;
            var winnerIssue = IssueInfoProvider.GetIssueInfo(winnerIssueId);

            if ((issueInForm != null) && (winnerIssue != null))
            {
                issueInForm.IssueSubject = winnerIssue.IssueSubject;
                issueInForm.IssueSenderName = winnerIssue.IssueSenderName;
                issueInForm.IssueSenderEmail = winnerIssue.IssueSenderEmail;
            }
        }
    }


    private void InitEngagement()
    {
        // Fill table data
        var table = new DataTable();
        CreateAndSetColumn(table, "sent", mIssue.IssueSentEmails);
        CreateAndSetColumn(table, "bounces", mIssue.IssueBounces);
        CreateAndSetColumn(table, "bouncerate", CalculateBounceRate());
        CreateAndSetColumn(table, "delivered", CalculateDeliveredEmails());
        CreateAndSetColumn(table, "deliveryrate", CalculateDeliveryRate());
        CreateAndSetColumn(table, "opens", OpenedEmails);
        CreateAndSetColumn(table, "openrate", CalculateUniqueOpenRate());
        CreateAndSetColumn(table, "totalclicks", TotalClicks);
        CreateAndSetColumn(table, "clicks", UniqueClicks);
        CreateAndSetColumn(table, "clickrate", CalculateUniqueClickRate());
        CreateAndSetColumn(table, "unsubscriptions", mIssue.IssueUnsubscribed);
        CreateAndSetColumn(table, "unsubscriptionrate", CalculateUnsubscriptionRate());
        

        // Assign dataset to unigrid source
        var ds = new DataSet();
        ds.Tables.Add(table);
        ugContactLoss.DataSource = ugDelivery.DataSource = ugEngagement.DataSource = ds;

        // Hide/show relevant columns
        ugEngagement.OnBeforeDataReload += () =>
        {
            ugEngagement.NamedColumns["delivered"].Visible = mMonitorBouncedEmails;
            ugEngagement.NamedColumns["deliveryrate"].Visible = mMonitorBouncedEmails;
            ugEngagement.NamedColumns["sent"].Visible = !mMonitorBouncedEmails;
        };

        ugContactLoss.OnAfterDataReload += () =>
        {
            ugContactLoss.NamedColumns["delivered"].Visible = mMonitorBouncedEmails;
            ugContactLoss.NamedColumns["deliveryrate"].Visible = mMonitorBouncedEmails;
            ugContactLoss.NamedColumns["sent"].Visible = !mMonitorBouncedEmails;
        };

        // Create rate labels with tooltips 
        ugEngagement.OnExternalDataBound += ExternalDataBound;
        ugDelivery.OnExternalDataBound += ExternalDataBound;
        ugContactLoss.OnExternalDataBound += ExternalDataBound;

        issueLinks.UniGrid.OnAfterRetrieveData += (dsLinks) =>
        {
            lnkAllLinks.Visible = !DataHelper.DataSourceIsEmpty(dsLinks);
            return dsLinks;
        };
    }

    private string ExternalDataBound(object sender, string rateName, object parameter)
    {
        switch (rateName.ToLowerInvariant())
        {
            case "clickrate":
                return GetRate(sender, mMonitorBouncedEmails ? "newsletter.clickratetooltip.delivered" : "newsletter.clickratetooltip.sent", parameter);
            case "deliveryrate":
                return GetRate(sender, "newsletter.deliveryratetooltip", parameter);
            case "openrate":
                return GetRate(sender, mMonitorBouncedEmails ? "newsletter.openratetooltip.delivered" : "newsletter.openratetooltip.sent", parameter);
            case "bouncerate":
                return GetRate(sender, "newsletter.bounceratetooltip", parameter);
            case "unsubscriptionrate":
                return GetRate(sender, mMonitorBouncedEmails ? "newsletter.unsubscriptionratetooltip.delivered" : "newsletter.unsubscriptionratetooltip.sent", parameter);
            case "opens":
                return GetHyperLinkOrOriginalStringIfInsufficientLicense("openedEmail", Convert.ToString(parameter, CultureHelper.PreferredUICultureInfo));
            case "clicks":
                return GetHyperLinkOrOriginalStringIfInsufficientLicense("clickedLink", Convert.ToString(parameter, CultureHelper.PreferredUICultureInfo));
            case "unsubscriptions":
                return GetHyperLinkOrOriginalStringIfInsufficientLicense("unsubscription", Convert.ToString(parameter, CultureHelper.PreferredUICultureInfo));
        }

        return parameter.ToString();
    }


    private string GetHyperLinkOrOriginalStringIfInsufficientLicense(string uiElementName, string text)
    {
        return (ObjectFactory<ILicenseService>.StaticSingleton().IsFeatureAvailable(FeatureEnum.FullContactManagement) && text.ToInteger(0) > 0) ? 
            GetHyperLink(uiElementName, text) : 
            text;
    }


    private string GetRate(object sender, string rateResourceString, object parameter)
    {
        WebControl control = sender as WebControl;
        if (control != null)
        {
            ScriptHelper.AppendTooltip(control, GetString(rateResourceString), null);
        }

        return String.Format(PERCENT_FORMAT, ValidationHelper.GetDouble(parameter, 0));
    }


    private string GetHyperLink(string retrieverIdentifier, string text)
    {
        var anchor = new HyperLink
        {
            NavigateUrl = GetDemographicsUrl(retrieverIdentifier),
            Target = "_blank",
            Text = text
        };

        return anchor.GetRenderedHTML();
    }


    private string GetDemographicsUrl(string retrieverIdentifier)
    {
        var parameters = new NameValueCollection();
        parameters.Add("issueid", mIssue.IssueID.ToString());

        return URLHelper.GetAbsoluteUrl(Service.Resolve<IContactDemographicsLinkBuilder>().GetDemographicsLink(retrieverIdentifier, parameters));
    }
    

    private void CreateAndSetColumn(DataTable dt, string colName, object value)
    {
        dt.Columns.Add(colName);

        if (dt.Rows.Count == 0)
        {
            dt.Rows.Add(value);
        }
        else
        {
            DataHelper.SetDataRowValue(dt.Rows[0], colName, value);
        }
    }

    
    private string GetUtmCampaignNameHtmlOutput()
    {
        CampaignInfo campaign = CampaignInfoProvider.GetCampaignByUTMCode(mIssue.IssueUTMCampaign, CurrentSiteName);

        // Is issue linked to an existing campaign
        if (campaign != null)
        {
            var campaignDetailUrl = Service.Resolve<IUILinkProvider>().GetSingleObjectLink(CampaignInfo.TYPEINFO.ModuleName, CAMPAIGN_ELEMENT_CODENAME, 
                                                                        new ObjectDetailLinkParameters
                                                                        {
                                                                            ObjectIdentifier = campaign.CampaignID,
                                                                            AllowNavigationToListing = true
                                                                        });
            var hyperLink = new HyperLink
            {
                NavigateUrl = URLHelper.GetAbsoluteUrl(campaignDetailUrl),
                Text = HTMLHelper.HTMLEncode(campaign.CampaignDisplayName),
                Target = "_top",
            };

            return hyperLink.GetRenderedHTML();
        }

        return mIssue.IssueUTMCampaign;
    }

    
    private int CalculateDeliveredEmails()
    {
        return mIssue.IssueSentEmails - mIssue.IssueBounces;
    }


    private double CalculateDeliveryRate()
    {
        return (mIssue.IssueSentEmails == 0) ? 0 : ((double)CalculateDeliveredEmails() / mIssue.IssueSentEmails) * 100;
    }


    private int CalculateTotalClicks()
    {
        var results = ClickedLinkInfoProvider.GetClickedLinks().Source(s => s.Join<LinkInfo>("ClickedLinkNewsletterLinkID", "LinkId"))
                                             .WhereIn("LinkIssueID", mParentAndVariantsIDs);
        return results.Count;
    }


    private int CalculateUniqueClicks()
    {
        var results = ClickedLinkInfoProvider.GetClickedLinks().Source(s => s.Join<LinkInfo>("ClickedLinkNewsletterLinkID", "LinkId"))
                                             .WhereIn("LinkIssueID", mParentAndVariantsIDs)
                                             .Distinct().Columns("ClickedLinkEmail", "LinkIssueID");
        return results.Count;
    }


    private double CalculateUniqueClickRate()
    {
        var totalDeliveredEmails = CalculateTotalDeliveredEmails();
        return (totalDeliveredEmails == 0) ? 0 : ((double)UniqueClicks / totalDeliveredEmails) * 100;
    }


    private int CalculateOpenedEmails()
    {
        int variantOpens = 0;

        if (mIssue.IssueIsABTest)
        {
            variantOpens = IssueInfoProvider.GetIssues()
                                            .Column(new AggregatedColumn(AggregationType.Sum, "IssueOpenedEmails"))
                                            .WhereEquals("IssueVariantOfIssueID", mIssue.IssueID)
                                            .GetScalarResult(0); 
        }

        return mIssue.IssueOpenedEmails + variantOpens;
    }
    

    private double CalculateUniqueOpenRate()
    {
        var totalDeliveredEmails = CalculateTotalDeliveredEmails();
        return (totalDeliveredEmails == 0) ? 0 : ((double)OpenedEmails / totalDeliveredEmails) * 100;
    }


    private int CalculateTotalDeliveredEmails()
    {
        return mMonitorBouncedEmails ? CalculateDeliveredEmails() : mIssue.IssueSentEmails;
    }


    private double CalculateBounceRate()
    {
        return (mIssue.IssueSentEmails == 0) ? 0 : ((double)mIssue.IssueBounces / mIssue.IssueSentEmails) * 100;
    }


    private double CalculateUnsubscriptionRate()
    {
        var totalDeliveredEmails = CalculateTotalDeliveredEmails();
        return (totalDeliveredEmails == 0) ? 0 : ((double)mIssue.IssueUnsubscribed / totalDeliveredEmails) * 100;
    }


    /// <summary>
    /// Initializes funnel graph.
    /// </summary>
    private void InitFunnel()
    {
        var graphData = new ArrayList
        {
            new
            {
                Title = HTMLHelper.HTMLEncode(GetString("unigrid.newsletter_issue.columns.issuesentemails")),
                Value = mIssue.IssueSentEmails,
                RateText = String.Empty,
            },
            new
            {
                Title = HTMLHelper.HTMLEncode(GetString("newsletter.issue.uniqueopens")),
                Value = OpenedEmails,
                RateText = String.Format(RATE_TEXT_FORMAT, GetString("newsletter.issue.uniqueopenrate"), CalculateUniqueOpenRate()),
            },
            new
            {
                Title = HTMLHelper.HTMLEncode(GetString("unigrid.newsletter_issue_trackedlinks.columns.uniqueclicks")),
                Value = UniqueClicks,
                RateText = String.Format(RATE_TEXT_FORMAT, GetString("newsletter.issue.uniqueclickrate"), CalculateUniqueClickRate()),
            }
        };

        if (mMonitorBouncedEmails)
        {
            graphData.Insert(1, new
            {
                Title = HTMLHelper.HTMLEncode(GetString("newsletter.issue.delivered")),
                Value = CalculateDeliveredEmails(),
                RateText = String.Format(RATE_TEXT_FORMAT, GetString("newsletters.issuedeliveryrate"), CalculateDeliveryRate()),
            });
        }

        ScriptHelper.RegisterModule(pnlFunnel, "CMS.Charts/Funnel", new
        {
            chartDiv = pnlFunnel.ClientID,
            data = graphData,
        });
    }
    
    #endregion
}