using CMS.FormEngine.Web.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using CMS.Base.Web.UI;
using CMS.FormEngine;
using CMS.Helpers;

public partial class CMSFormControls_DropdownChosen : FormEngineUserControl
{
    private List<string> _selectedValues;

    public bool AllowMultiple => ValidationHelper.GetBoolean(GetValue("AllowMultiple"), true);
    public bool Readonly => ValidationHelper.GetBoolean(GetValue("Readonly"), false);
    public string Options => GetResolvedValue<string>("options", null);
    public string MacroSource => GetValue("Macro", string.Empty);
    public string QuerySource => GetValue("Query", string.Empty);
    public string TextFormat => GetValue("TextFormat", string.Empty);
    public string ValueFormat => GetValue("ValueFormat", string.Empty);
    public bool SortItems => GetValue("SortItems", false);

    protected void Page_Load(object sender, EventArgs e)
    {
        ScriptHelper.RegisterScriptFile(Page, "~/CMSScripts/DeleteBoilerplate/jquery/jquery-3.3.1.min.js");
        ScriptHelper.RegisterScriptFile(Page, "~/CMSScripts/DeleteBoilerplate/chosen/chosen.jquery.js");
        ScriptHelper.RegisterScriptFile(Page, "~/CMSScripts/DeleteBoilerplate/chosen/chosen.order.jquery.js");
        CssRegistration.RegisterCssLink(Page, "~/CMSScripts/DeleteBoilerplate/chosen/chosen.css");

        LoadAndSelectList(DependsOnAnotherField);
        DataBind();
    }

    public override object Value
    {
        get
        {
            return hdSelectedOptions.Value;
        }

        set
        {
            hdSelectedOptions.Value = ValidationHelper.GetString(value, string.Empty);

            LoadAndSelectList();

            if ((value != null) || ((FieldInfo != null) && FieldInfo.AllowEmpty))
            {
                if (FieldInfo != null)
                {
                    value = ConvertInputValue(value);
                }

                _selectedValues = ValidationHelper.GetString(value, string.Empty).Split('|').ToList();

                lbOptions.ClearSelection();
                foreach (ListItem option in lbOptions.Items)
                {
                    option.Selected =
                        _selectedValues.Contains(option.Value, StringComparer.InvariantCultureIgnoreCase);
                }
            }
        }
    }

    private void LoadAndSelectList(bool forceReload = false)
    {
        lbOptions.SelectionMode = AllowMultiple ? ListSelectionMode.Multiple : ListSelectionMode.Single;

        if (forceReload)
        {
            // Keep selected value
            _selectedValues = lbOptions.Items.Cast<ListItem>().Where(x => x.Selected).Select(x => x.Value).ToList();

            // Clears values if forced reload is requested
            lbOptions.Items.Clear();
        }

        if (lbOptions.Items.Count == 0)
        {
            try
            {
                var def = new SpecialFieldsDefinition(null, FieldInfo, ContextResolver, SortItems);
                if (string.IsNullOrEmpty(MacroSource))
                {
                    if (string.IsNullOrEmpty(QuerySource))
                    {
                        // Load from text source
                        def.LoadFromText(Options);
                    }
                    else
                    {
                        // Load from query source
                        def.LoadFromQuery(QuerySource);
                    }
                }
                else
                {
                    // Load from macro source
                    def.LoadFromMacro(MacroSource, ValueFormat, TextFormat);
                }
                def.FillItems(lbOptions.Items);
            }
            catch (Exception ex)
            {
                DisplayException(ex);
            }

            foreach (ListItem lbOptionsItem in lbOptions.Items)
            {
                if (_selectedValues != null && _selectedValues.Contains(lbOptionsItem.Value))
                {
                    lbOptionsItem.Selected = true;
                }
            }

            lbOptions.Items.Insert(0, new ListItem(String.Empty, String.Empty));
        }
    }

    private void DisplayException(Exception ex)
    {
        var ctrlError = new FormControlError
        {
            FormControlName = FormFieldControlTypeCode.DROPDOWNLIST,
            InnerException = ex
        };
        Controls.Add(ctrlError);
        lbOptions.Visible = false;
    }
}