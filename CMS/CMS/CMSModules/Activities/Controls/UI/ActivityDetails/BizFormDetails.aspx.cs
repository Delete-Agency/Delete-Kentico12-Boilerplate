﻿using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.Core;
using CMS.FormEngine;
using CMS.Helpers;
using CMS.OnlineForms;
using CMS.SiteProvider;
using CMS.UIControls;


[Title("om.activitydetals.viewrecorddetail")]
[Security(Resource = ModuleName.ACTIVITIES, Permission = "ReadActivities")]
public partial class CMSModules_Activities_Controls_UI_ActivityDetails_BizFormDetails : CMSModalPage
{
    private const string FORM_ITEM_PREVIEW_ROUTE_TEMPLATE = "/Kentico.FormBuilder/FormItem/Preview/{0}/{1}";


    protected void Page_Init(object sender, EventArgs e)
    {
        // Check permissions
        if (!QueryHelper.ValidateHash("hash"))
        {
            return;
        }

        int bizId = QueryHelper.GetInteger("bizid", 0);
        int recId = QueryHelper.GetInteger("recid", 0);

        if ((bizId > 0) && (recId > 0))
        {
            var bfi = BizFormInfoProvider.GetBizFormInfo(bizId);

            if (bfi == null)
            {
                return;
            }

            if (bfi.FormDevelopmentModel == (int)FormDevelopmentModelEnum.Mvc)
            {
                var path = string.Format(FORM_ITEM_PREVIEW_ROUTE_TEMPLATE, bfi.FormID, recId);
                
                string url = (bfi.Site as SiteInfo).SitePresentationURL;
                mvcFrame.Visible = true;

                // Modify frame 'src' attribute and add administration domain into it
                ScriptHelper.RegisterModule(this, "CMS.Builder/FrameSrcAttributeModifier", new
                {
                    frameId = mvcFrame.ClientID,
                    frameSrc = url.TrimEnd('/') + VirtualContext.GetFormBuilderPath(path, CurrentUser.UserName),
                    mixedContentMessage = GetString("builder.ui.mixedcontenterrormessage"),
                    applicationPath = SystemContext.ApplicationPath
                });
            }
            else
            {
                bizRecord.ItemID = recId;
                bizRecord.SiteName = SiteInfoProvider.GetSiteName(bfi.FormSiteID);
                bizRecord.FormName = bfi.FormName;
                bizRecord.Visible = true;
            }
        }
    }


    protected void Page_PreRender(object sender, EventArgs e)
    {
        if (bizRecord != null)
        {
            bizRecord.SubmitButton.Visible = false;
        }
    }
}