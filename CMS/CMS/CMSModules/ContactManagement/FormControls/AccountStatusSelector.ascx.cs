﻿using System;

using CMS.Base;
using CMS.Base.Web.UI;
using CMS.DataEngine;
using CMS.FormEngine.Web.UI;
using CMS.Helpers;
using CMS.UIControls;


public partial class CMSModules_ContactManagement_FormControls_AccountStatusSelector : FormEngineUserControl
{
    #region "Properties"

    /// <summary>
    /// Gets or sets the enabled state of the control.
    /// </summary>
    public override bool Enabled
    {
        get
        {
            return base.Enabled;
        }
        set
        {
            EnsureChildControls();
            base.Enabled = value;
            uniselector.Enabled = value;
        }
    }


    /// <summary>
    /// Gets or sets field value.
    /// </summary>
    public override object Value
    {
        get
        {
            EnsureChildControls();
            return uniselector.Value;
        }
        set
        {
            EnsureChildControls();
            uniselector.Value = ValidationHelper.GetString(value, UniSelector.US_NONE_RECORD.ToString());
        }
    }


    /// <summary>
    /// Returns Uniselector.
    /// </summary>
    public UniSelector UniSelector
    {
        get
        {
            if (uniselector == null)
            {
                pnlUpdate.LoadContainer();
            }
            return uniselector;
        }
    }


    /// <summary>
    /// CMSDropDownList used in Uniselector.
    /// </summary>
    public CMSDropDownList DropDownList
    {
        get
        {
            if (uniselector == null)
            {
                pnlUpdate.LoadContainer();
            }
            return uniselector.DropDownSingleSelect;
        }
    }


    /// <summary>
    /// Specifies, whether the selector allows selection of all items. If the dialog allows selection of all items, 
    /// it displays the (all) field in the DDL variant and All button in the Textbox variant (default false). 
    /// When property is selected then Uniselector doesn't load any data from DB, it just returns -1 value 
    /// and external code must handle data loading.
    /// </summary>
    public bool AllowAllItem
    {
        get
        {
            return ValidationHelper.GetBoolean(GetValue("AllowAllItem"), true);
        }
        set
        {
            SetValue("AllowAllItem", value);
        }
    }


    /// <summary>
    /// Gets selected AccountStatusID.
    /// </summary>
    public int AccountStatusID
    {
        get
        {
            return ValidationHelper.GetInteger(Value, 0);
        }
    }


    /// <summary>
    /// SQL WHERE condition of uniselector.
    /// </summary>
    public string WhereCondition
    {
        get;
        set;
    }


    /// <summary>
    /// Additional CSS class for drop down list control.
    /// </summary>
    public String AdditionalDropDownCSSClass
    {
        get
        {
            return uniselector.AdditionalDropDownCSSClass;
        }
        set
        {
            uniselector.AdditionalDropDownCSSClass = value;
        }
    }

    #endregion


    #region "Methods"

    protected void Page_Load(object sender, EventArgs e)
    {
        if (StopProcessing)
        {
            uniselector.StopProcessing = true;
        }
        else
        {
            ReloadData();
        }
    }


    /// <summary>
    /// Reloads control.
    /// </summary>
    public void ReloadData()
    {
        var where = new WhereCondition(WhereCondition);
        uniselector.AllowAll = AllowAllItem;

        // Do not add condition to empty condition which allows everything
        if (!String.IsNullOrEmpty(where.WhereCondition))
        {
            string status = ValidationHelper.GetString(Value, "");
            if (!String.IsNullOrEmpty(status))
            {
                where.Or().WhereEquals(uniselector.ReturnColumnName, status);
            }
        }

        uniselector.WhereCondition = where.ToString(true);
        uniselector.Reload(true);
    }


    /// <summary>
    /// Sets the given <paramref name="value"/> as the <see cref="Value"/> of current selector. In case <c>null</c> is passed, sets value denoting all statuses should be displayed.
    /// </summary>
    public override void LoadControlValue(object value)
    {
        base.LoadControlValue(value);

        if (value == null)
        {
            Value = GetDefaultValue();
        }
    }


    /// <summary>
    /// Gets where condition.
    /// </summary>
    public override string GetWhereCondition()
    {
        switch (uniselector.Value.ToInteger(GetDefaultValue()))
        {
            case UniSelector.US_ALL_RECORDS:
                return GetWhereConditionForAllRecords();
            case UniSelector.US_NONE_RECORD:
                return GetWhereConditionForNullRecords();
            default:
                return GetWhereConditionForExactMatch();
        }
    }


    private int GetDefaultValue()
    {
        return AllowAllItem ? UniSelector.US_ALL_RECORDS : UniSelector.US_NONE_RECORD;
    }


    private string GetWhereConditionForExactMatch()
    {
        return string.Format("{0} = {1}", FieldInfo.Name, uniselector.Value);
    }


    private string GetWhereConditionForNullRecords()
    {
        return string.Format("{0} IS NULL", FieldInfo.Name);
    }


    private static string GetWhereConditionForAllRecords()
    {
        return String.Empty;
    }
    #endregion
}