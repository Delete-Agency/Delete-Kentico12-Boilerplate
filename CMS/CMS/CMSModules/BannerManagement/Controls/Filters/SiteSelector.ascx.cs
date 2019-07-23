﻿using System;

using CMS.DocumentEngine.Web.UI;
using CMS.SiteProvider;
using CMS.UIControls;


/// <summary>
/// Site selector for selecting site specific, all sites or global objects.
/// </summary>
public partial class CMSModules_BannerManagement_Controls_Filters_SiteSelector : CMSAbstractBaseFilterControl
{
    #region "Properties"

    /// <summary>
    /// Where condition.
    /// </summary>
    public override string WhereCondition
    {
        get
        {
            base.WhereCondition = GetWhereCondition();
            return base.WhereCondition;
        }
        set
        {
            base.WhereCondition = value;
        }
    }


    /// <summary>
    /// Gets or sets site ID.
    /// </summary>
    public override int SiteID
    {
        get
        {
            return siteSelector.SiteID;
        }
        set
        {
            siteSelector.SiteID = value;
        }
    }


    /// <summary>
    /// Filter value.
    /// </summary>
    public override object Value
    {
        get
        {
            return siteSelector.Value;
        }
        set
        {
            siteSelector.Value = value;
        }
    }

    #endregion


    #region "Methods and events"

    /// <summary>
    /// OnLoad override - check whether filter is set.
    /// </summary>
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        siteSelector.Selector.SelectedIndexChanged += new EventHandler(Selector_SelectedIndexChanged);
    }


    /// <summary>
    /// Generates where condition.
    /// </summary>
    public string GetWhereCondition()
    {
        int currentSiteID = SiteContext.CurrentSiteID;

        switch (siteSelector.SiteID)
        {
            case UniSelector.US_GLOBAL_RECORD: // Global objects only
                return "BannerCategorySiteID IS NULL";

            case UniSelector.US_GLOBAL_AND_SITE_RECORD: // Global and site objects
                return "(BannerCategorySiteID = " + currentSiteID + ") OR BannerCategorySiteID IS NULL";

            default: // Site objects only
                return "BannerCategorySiteID = " + currentSiteID;
        }
    }


    /// <summary>
    /// Handles Site selector OnSelectionChanged event.
    /// </summary>
    protected void Selector_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Raise OnFilterChange event
        RaiseOnFilterChanged();
    }

    #endregion
}