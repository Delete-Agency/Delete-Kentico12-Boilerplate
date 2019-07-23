﻿using CMS.Ecommerce;
using CMS.Ecommerce.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_Ecommerce_FormControls_SupplierSelector : SiteSeparatedObjectSelector
{
    #region "Variables"

    private bool mReflectGlobalProductsUse;

    #endregion


    #region "Properties"

    /// <summary>
    /// Allows to access uniselector object
    /// </summary>
    public override UniSelector UniSelector
    {
        get
        {
            return uniSelector;
        }
    }


    /// <summary>
    ///  If true, selected value is SupplierName, if false, selected value is SupplierID.
    /// </summary>
    public override bool UseNameForSelection
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("UseSupplierNameForSelection"), base.UseNameForSelection);
        }
        set
        {
            SetValue("UseSupplierNameForSelection", value);
            base.UseNameForSelection = value;
        }
    }


    /// <summary>
    /// Indicates whether global items are to be offered.
    /// </summary>
    public override bool DisplayGlobalItems
    {
        get
        {
            return base.DisplayGlobalItems || (ReflectGlobalProductsUse && ECommerceSettings.AllowGlobalProducts(SiteID));
        }
        set
        {
            base.DisplayGlobalItems = value;
        }
    }


    /// <summary>
    /// Gets or sets a value that indicates if the global items should be displayed when the global products are used on the site.
    /// </summary>
    public bool ReflectGlobalProductsUse
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("ReflectGlobalProductsUse"), mReflectGlobalProductsUse);
        }
        set
        {
            SetValue("ReflectGlobalProductsUse", value);
            mReflectGlobalProductsUse = value;
        }
    }

    #endregion


    #region "Methods"

    /// <summary>
    /// Convert given supplier name to its ID for specified site.
    /// </summary>
    /// <param name="name">Code name of the supplier.</param>
    /// <param name="siteName">Name of the site to translate code name for.</param>
    protected override int GetID(string name, string siteName)
    {
        var supplierInfoObj = SupplierInfoProvider.GetSupplierInfo(name, siteName);

        return (supplierInfoObj != null) ? supplierInfoObj.SupplierID : 0;
    }

    #endregion
}