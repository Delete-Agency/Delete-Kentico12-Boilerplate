﻿<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="CMS_FormSettings.ascx.cs"
    Inherits="CMSModules_BizForms_FormControls_Cloning_CMS_FormSettings" %>

<div class="form-horizontal">
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblTableName" ResourceString="clonning.settings.class.tablename"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="txtTableName" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSTextBox runat="server" ID="txtTableName" MaxLength="100" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblCloneItems" ResourceString="clonning.settings.form.cloneitems"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="chkCloneItems" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkCloneItems" Checked="true" />
        </div>
    </div>
    <div class="form-group">
        <div class="editing-form-label-cell">
            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblCloneAlternativeForms" ResourceString="clonning.settings.class.alternativeform"
                EnableViewState="false" DisplayColon="true" AssociatedControlID="chkCloneAlternativeForms" />
        </div>
        <div class="editing-form-value-cell">
            <cms:CMSCheckBox runat="server" ID="chkCloneAlternativeForms" Checked="true" />
        </div>
    </div>
</div>