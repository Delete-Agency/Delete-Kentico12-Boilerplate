﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_AdminControls_Controls_Class_NewClassWizard"
    CodeBehind="NewClassWizard.ascx.cs" %>

<%@ Register Src="~/CMSFormControls/System/LocalizableTextBox.ascx" TagName="LocalizableTextBox"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/UI/UniSelector/UniSelector.ascx" TagName="UniSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/AdminControls/Controls/Class/FieldEditor/FieldEditor.ascx"
    TagName="FieldEditor" TagPrefix="cms" %>
<%@ Register Src="~/CMSAdminControls/Wizard/Header.ascx" TagName="WizardHeader" TagPrefix="cms" %>
<%@ Register Src="~/CMSFormControls/Classes/SelectClass.ascx" TagName="SelectClass"
    TagPrefix="cms" %>

<div class="GlobalWizard">
    <table>
        <tr class="Top">
            <td class="Left">&nbsp;
            </td>
            <td class="Center">
                <cms:WizardHeader ID="ucHeader" runat="server" />
            </td>
            <td class="Right">&nbsp;
            </td>
        </tr>
        <tr class="Middle">
            <td class="Center" colspan="3">
                <div id="wzdBody">
                    <asp:Wizard ID="wzdNewDocType" runat="server" DisplaySideBar="false" OnNextButtonClick="wzdNewDocType_NextButtonClick"
                        OnFinishButtonClick="wzdNewDocType_FinishButtonClick" CssClass="Wizard">
                        <StartNavigationTemplate>
                            <div id="buttonsDiv" class="WizardButtons">
                                <cms:LocalizedButton UseSubmitBehavior="True" ID="StepNextButton" runat="server"
                                    CommandName="MoveNext" Text="{$general.next$}" ButtonStyle="Primary" />
                            </div>
                        </StartNavigationTemplate>
                        <StepNavigationTemplate>
                            <div id="buttonsDiv" class="WizardButtons">
                                <cms:LocalizedButton UseSubmitBehavior="True" ID="StepNextButton" runat="server"
                                    CommandName="MoveNext" Text="{$general.next$}" ButtonStyle="Primary" />
                            </div>
                        </StepNavigationTemplate>
                        <FinishNavigationTemplate>
                            <div id="buttonsDiv" class="WizardButtons">
                                <cms:LocalizedButton UseSubmitBehavior="True" ID="StepFinishButton" runat="server"
                                    CommandName="MoveComplete" ResourceString="general.finish" ButtonStyle="Primary" />
                            </div>
                        </FinishNavigationTemplate>
                        <WizardSteps>
                            <%-- Step 1 --%>
                            <asp:WizardStep ID="wzdStep1" runat="server" AllowReturn="false">
                                <asp:Panel ID="pnlWzdStep1" runat="server" CssClass="GlobalWizardStep">
                                    <cms:MessagesPlaceHolder runat="server" ID="pnlMessages1" />
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="editing-form-label-cell">
                                                <cms:LocalizedLabel CssClass="control-label" DisplayColon="True" runat="server" ID="lblDisplayName" AssociatedControlID="txtDisplayName"
                                                    ShowRequiredMark="true" />
                                            </div>
                                            <div class="editing-form-value-cell">
                                                <cms:LocalizableTextBox runat="server" ID="txtDisplayName" MaxLength="100" />
                                                <cms:CMSRequiredFieldValidator ID="rfvDisplayName" runat="server" ControlToValidate="txtDisplayName:cntrlContainer:textbox"
                                                    Display="dynamic" />
                                            </div>
                                        </div>
                                    </div>
                                    <cms:LocalizedHeading runat="server" ID="lblFullCodeName" Level="4" />
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="editing-form-label-cell">
                                                <cms:LocalizedLabel CssClass="control-label" DisplayColon="True" runat="server" ID="lblNamespaceName" ShowRequiredMark="true" />
                                            </div>
                                            <div class="editing-form-value-cell">
                                                <cms:CMSTextBox runat="server" ID="txtNamespaceName" MaxLength="49" />
                                                <cms:CMSRequiredFieldValidator ID="rfvNamespaceName" runat="server" EnableViewState="false"
                                                    ControlToValidate="txtNamespaceName" Display="dynamic" />
                                                <cms:CMSRegularExpressionValidator ID="revNameSpaceName" runat="server" EnableViewState="false"
                                                    Display="dynamic" ControlToValidate="txtNamespaceName" />
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <div class="editing-form-label-cell">
                                                <cms:LocalizedLabel CssClass="control-label" DisplayColon="True" runat="server" ID="lblCodeName" ShowRequiredMark="true" />
                                            </div>
                                            <div class="editing-form-value-cell">
                                                <cms:CMSTextBox runat="server" ID="txtCodeName" MaxLength="50" />
                                                <cms:CMSRequiredFieldValidator ID="rfvCodeName" runat="server" EnableViewState="false"
                                                    ControlToValidate="txtCodeName" Display="dynamic" />
                                                <cms:CMSRegularExpressionValidator ID="revCodeName" runat="server" EnableViewState="false"
                                                    Display="dynamic" ControlToValidate="txtCodeName" />
                                            </div>
                                        </div>
                                    </div>
                                </asp:Panel>
                            </asp:WizardStep>
                            <%-- Step 2 --%>
                            <asp:WizardStep ID="wzdStep2" runat="server" AllowReturn="false">
                                <asp:Panel ID="pnlWzdStep2" runat="server" CssClass="GlobalWizardStep">
                                    <cms:MessagesPlaceHolder runat="server" ID="pnlMessages2" />
                                    <div class="form-horizontal">
                                        <div class="radio-list-vertical">
                                            <cms:CMSRadioButton ID="radCustom" runat="server" Checked="True" GroupName="DocType" />
                                            <asp:PlaceHolder ID="plcExisting" runat="server" Visible="false">
                                                <cms:CMSRadioButton ID="radNewTable" runat="server" Checked="True" GroupName="CustomTable"
                                                    AutoPostBack="true" OnCheckedChanged="radExistingTable_CheckedChanged" />
                                                <cms:CMSRadioButton ID="radExistingTable" runat="server" GroupName="CustomTable" AutoPostBack="true"
                                                    OnCheckedChanged="radExistingTable_CheckedChanged" />
                                            </asp:PlaceHolder>
                                            <div class="selector-subitem">
                                                <div class="form-group">
                                                    <div class="editing-form-label-cell">
                                                        <cms:LocalizedLabel CssClass="control-label" DisplayColon="True" runat="server" ID="lblTableName" ShowRequiredMark="true" />
                                                    </div>
                                                    <div class="editing-form-value-cell">
                                                        <cms:CMSDropDownList runat="server" ID="drpExistingTables" Visible="false" CssClass="DropDownField" />
                                                        <cms:CMSTextBox ID="txtTableName" runat="server" MaxLength="100" />

                                                        <asp:Label ID="lblTableNameError" runat="server" CssClass="form-control-error" Visible="false" />
                                                    </div>
                                                </div>
                                                <asp:PlaceHolder runat="server" ID="plcPKName">
                                                    <div class="form-group">
                                                        <div class="editing-form-label-cell">
                                                            <cms:LocalizedLabel CssClass="control-label" DisplayColon="True" runat="server" ID="lblPKName" ShowRequiredMark="true" />
                                                        </div>
                                                        <div class="editing-form-value-cell">
                                                            <cms:CMSTextBox ID="txtPKName" runat="server" MaxLength="100" />
                                                            <asp:Label ID="lblPKNameError" runat="server" CssClass="form-control-error" Visible="false" />
                                                        </div>
                                                    </div>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder runat="server" ID="plcDocTypeOptions" Visible="false">
                                                    <div class="form-group">
                                                        <div class="editing-form-label-cell">
                                                            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblInherits" EnableViewState="false" ResourceString="DocumentType.InheritsFrom" AssociatedControlID="selInherits" />
                                                        </div>
                                                        <div class="editing-form-value-cell">
                                                            <cms:SelectClass ID="selInherits" runat="server" DisplayNoneValue="true" />
                                                        </div>
                                                    </div>
                                                    <div class="form-group">
                                                        <div class="editing-form-label-cell">
                                                            <cms:LocalizedLabel CssClass="control-label" runat="server" ID="lblContentOnly" EnableViewState="false" ResourceString="DocumentType.ContentOnly" DisplayColon="true" AssociatedControlID="chbContentOnly" />
                                                        </div>
                                                        <div class="editing-form-value-cell">
                                                            <cms:CMSCheckBox ID="chbContentOnly" runat="server" />
                                                        </div>
                                                    </div>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="plcMNClassOptions" runat="server" Visible="false">
                                                    <div class="form-group">
                                                        <div class="editing-form-label-cell">
                                                            <asp:Label CssClass="control-label" ID="lblIsMNTable" runat="server" AssociatedControlID="chbIsMNTable" />
                                                        </div>
                                                        <div class="editing-form-value-cell">
                                                            <cms:CMSCheckBox ID="chbIsMNTable" runat="server" Checked="False" />
                                                        </div>
                                                    </div>
                                                    <div class="form-group">
                                                        <div class="editing-form-label-cell">
                                                            <asp:Label CssClass="control-label" ID="lblClassGuid" runat="server" AssociatedControlID="chkClassGuid" />
                                                        </div>
                                                        <div class="editing-form-value-cell">
                                                            <cms:CMSCheckBox ID="chkClassGuid" runat="server" Checked="true" />
                                                        </div>
                                                    </div>
                                                    <div class="form-group">
                                                        <div class="editing-form-label-cell">
                                                            <asp:Label CssClass="control-label" ID="lblClassLastModified" runat="server" AssociatedControlID="chkClassLastModified" />
                                                        </div>
                                                        <div class="editing-form-value-cell">
                                                            <cms:CMSCheckBox ID="chkClassLastModified" runat="server" Checked="true" />
                                                        </div>
                                                    </div>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder ID="plcCustomTablesOptions" runat="server" Visible="false">
                                                    <div class="form-group">
                                                        <div class="editing-form-label-cell">
                                                            <asp:Label CssClass="control-label" ID="lblItemGUID" runat="server" AssociatedControlID="chkItemGUID" />
                                                        </div>
                                                        <div class="editing-form-value-cell">
                                                            <cms:CMSCheckBox ID="chkItemGUID" runat="server" Checked="true" />
                                                        </div>
                                                    </div>
                                                    <div class="form-group">
                                                        <div class="editing-form-label-cell">
                                                            <asp:Label CssClass="control-label" ID="lblItemCreatedBy" runat="server" AssociatedControlID="chkItemCreatedBy" />
                                                        </div>
                                                        <div class="editing-form-value-cell">
                                                            <cms:CMSCheckBox ID="chkItemCreatedBy" runat="server" Checked="true" />
                                                        </div>
                                                    </div>
                                                    <div class="form-group">
                                                        <div class="editing-form-label-cell">
                                                            <asp:Label CssClass="control-label" ID="lblItemCreatedWhen" runat="server" AssociatedControlID="chkItemCreatedWhen" />
                                                        </div>
                                                        <div class="editing-form-value-cell">
                                                            <cms:CMSCheckBox ID="chkItemCreatedWhen" runat="server" Checked="true" />
                                                        </div>
                                                    </div>
                                                    <div class="form-group">
                                                        <div class="editing-form-label-cell">
                                                            <asp:Label CssClass="control-label" ID="lblItemModifiedBy" runat="server" AssociatedControlID="chkItemModifiedBy" />
                                                        </div>
                                                        <div class="editing-form-value-cell">
                                                            <cms:CMSCheckBox ID="chkItemModifiedBy" runat="server" Checked="true" />
                                                        </div>
                                                    </div>
                                                    <div class="form-group">
                                                        <div class="editing-form-label-cell">
                                                            <asp:Label CssClass="control-label" ID="lblItemModifiedWhen" runat="server" AssociatedControlID="chkItemModifiedWhen" />
                                                        </div>
                                                        <div class="editing-form-value-cell">
                                                            <cms:CMSCheckBox ID="chkItemModifiedWhen" runat="server" Checked="true" />
                                                        </div>
                                                    </div>
                                                    <asp:PlaceHolder runat="server" ID="plcOrder">
                                                        <div class="form-group">
                                                            <div class="editing-form-label-cell">
                                                                <asp:Label CssClass="control-label" ID="lblItemOrder" runat="server" AssociatedControlID="chkItemOrder" />
                                                            </div>
                                                            <div class="editing-form-value-cell">
                                                                <cms:CMSCheckBox ID="chkItemOrder" runat="server" Checked="true" />
                                                            </div>
                                                        </div>
                                                    </asp:PlaceHolder>
                                                </asp:PlaceHolder>
                                            </div>
                                            <cms:CMSRadioButton ID="radContainer" runat="server" GroupName="DocType" />
                                        </div>
                                    </div>
                                </asp:Panel>
                            </asp:WizardStep>
                            <%-- Step 3 --%>
                            <asp:WizardStep ID="wzdStep3" runat="server" AllowReturn="false">
                                <asp:Panel ID="pnlWzdStep3" runat="server" CssClass="GlobalWizardStep FieldEditorPanel"
                                    Height="500px">
                                    <cms:FieldEditor ID="FieldEditor" IsLiveSite="false" runat="server" DisplaySourceFieldSelection="false" UseCustomHeaderActions="true" ShowQuickLinks="false" AllowDummyFields="true" />
                                </asp:Panel>
                            </asp:WizardStep>
                            <%-- Step 4 --%>
                            <asp:WizardStep ID="wzdStep4" runat="server" AllowReturn="false">
                                <asp:Panel ID="pnlWzdStep4" runat="server" CssClass="GlobalWizardStep">
                                    <div class="form-horizontal">
                                        <div class="form-group">
                                            <div class="editing-form-label-cell">
                                                <cms:LocalizedLabel DisplayColon="True" CssClass="control-label" ID="lblSelectField" runat="server" AssociatedControlID="lstFields" />
                                            </div>
                                            <div class="editing-form-value-cell">
                                                <cms:CMSDropDownList ID="lstFields" runat="server" />
                                            </div>
                                        </div>
                                    </div>
                                </asp:Panel>
                            </asp:WizardStep>
                            <%-- Step 5 --%>
                            <asp:WizardStep ID="wzdStep5" runat="server" AllowReturn="false">
                                <asp:Panel ID="pnlWzdStep5" runat="server" CssClass="GlobalWizardStep">
                                    <cms:UniSelector ID="usParentTypes" runat="server" IsLiveSite="false" ObjectType="cms.documenttype"
                                        SelectionMode="Multiple" ResourcePrefix="allowedclasscontrol" DisplayNameFormat="{%ClassDisplayName%} ({%ClassName%})" />
                                </asp:Panel>
                            </asp:WizardStep>
                            <%-- Step 6 --%>
                            <asp:WizardStep ID="wzdStep6" runat="server" AllowReturn="false">
                                <asp:Panel ID="pnlWzdStep6" runat="server" CssClass="GlobalWizardStep">
                                    <cms:MessagesPlaceHolder runat="server" ID="pnlMessages6" />
                                    <cms:UniSelector ID="usSites" runat="server" IsLiveSite="false" ObjectType="cms.site"
                                        SelectionMode="Multiple" ResourcePrefix="sitesselect" />
                                </asp:Panel>
                            </asp:WizardStep>
                            <%-- Step 7 --%>
                            <asp:WizardStep ID="wzdStep7" runat="server" AllowReturn="false">
                                <asp:Panel ID="pnlWzdStep7" runat="server" CssClass="GlobalWizardStep">
                                    <cms:LocalizedHeading runat="server" ID="headInfoStep7" Level="3" EnableViewState="false" ResourceString="documenttype_new_step8.info" />
                                    <asp:Label runat="server" ID="lblDocumentCreated" CssClass="WizardFinishedStep" />
                                    <asp:Label runat="server" ID="lblTableCreated" CssClass="WizardFinishedStep" />
                                    <asp:Label runat="server" ID="lblChildTypesAdded" CssClass="WizardFinishedStep" />
                                    <asp:Label runat="server" ID="lblSitesSelected" CssClass="WizardFinishedStep" />
                                    <asp:Label runat="server" ID="lblPermissionNameCreated" CssClass="WizardFinishedStep" />
                                    <asp:Label runat="server" ID="lblDefaultIconCreated" CssClass="WizardFinishedStep" />
                                    <asp:Label runat="server" ID="lblSearchSpecificationCreated" CssClass="WizardFinishedStep" />
                                </asp:Panel>
                            </asp:WizardStep>
                        </WizardSteps>
                    </asp:Wizard>
                </div>
            </td>
        </tr>
    </table>
</div>
