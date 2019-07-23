﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Web;

using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_System_Debug_System_DebugCacheItems : CMSDebugPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        btnClear.Text = GetString("Administration-System.btnClearCache");
    }


    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);

        ReloadData();
    }


    /// <summary>
    /// Loads the data.
    /// </summary>
    protected void ReloadData()
    {
        lock (HttpRuntime.Cache)
        {
            List<string> keyList = new List<string>();
            IDictionaryEnumerator cacheEnum = HttpRuntime.Cache.GetEnumerator();

            // Build the items list
            while (cacheEnum.MoveNext())
            {
                string key = cacheEnum.Key.ToString();
                if (!String.IsNullOrEmpty(key))
                {
                    keyList.Add(key);
                }
            }
            keyList.Sort();

            // Load the grids
            gridItems.AllItems = keyList;
            gridItems.ReloadData();

            gridDummy.AllItems = keyList;
            gridDummy.ReloadData();
        }
    }


    protected void btnClear_Click(object sender, EventArgs e)
    {
        CacheHelper.ClearCache();

        ReloadData();
    }
}