﻿<%@ Page Language="C#" AutoEventWireup="true"  Codebehind="Edit.aspx.cs" MasterPageFile="~/CMSMasterPages/UI/SimplePage.master"
    Title="Objectworkflowtrigger properties" Inherits="CMSModules_ContactManagement_Pages_Tools_Automation_Process_Trigger_Edit"
    Theme="Default" %>

<%@ Register Src="~/CMSModules/ContactManagement/Controls/UI/Automation/TriggerScoreProperties.ascx"
    TagName="ScoreProperties" TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Scoring/FormControls/SelectScore.ascx" TagName="ScoreTypeSelector"
    TagPrefix="cms" %>
<%@ Register Src="~/CMSModules/Activities/FormControls/ActivityTypeSelector.ascx"
    TagName="ActivityTypeSelector" TagPrefix="cms" %>

<asp:Content ID="cntBody" runat="server" ContentPlaceHolderID="plcContent">
    <cms:CMSUpdatePanel ID="pnlUpdate" runat="server">
        <ContentTemplate>
            <cms:UIForm runat="server" ID="editForm" ObjectType="cms.objectworkflowtrigger" RedirectUrlAfterCreate="Edit.aspx?objectworkflowtriggerid={%EditedObject.ID%}&saved=1"
                DefaultFieldLayout="TwoColumns" FieldGroupHeadingIsAnchor="False">
                <SecurityCheck Resource="CMS.OnlineMarketing" Permission="ManageProcesses" />
                <LayoutTemplate>
                    <cms:FormCategory runat="server" ID="pnlGeneral" CategoryTitleResourceString="general.general">
                        <cms:FormField runat="server" ID="tDisplayName" Field="TriggerDisplayName" FormControl="LocalizableTextBox" ResourceString="general.displayname" DisplayColon="true" />
                        <cms:FormField runat="server" ID="tType" Field="TriggerType" FormControl="DropDownListControl" Layout="Inline">
                            <div class="form-group">
                                <div class="editing-form-label-cell">
                                    <cms:FormLabel CssClass="control-label" ID="lblTriggerType" runat="server" EnableViewState="false"
                                        ResourceString="general.type" DisplayColon="true" />
                                </div>
                                <div class="editing-form-value-cell">
                                    <cms:CMSDropDownList ID="ddlType" UseResourceStrings="true" CssClass="DropDownField"
                                        runat="server" AutoPostBack="true" />
                                </div>
                            </div>
                        </cms:FormField>
                        <asp:Panel runat="server" ID="pnlActivityType" CssClass="form-group">
                            <div class="editing-form-label-cell">
                                <cms:FormLabel CssClass="control-label" ID="lblActivityType" runat="server" EnableViewState="false" ResourceString="om.activity.type"
                                    DisplayColon="true" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:ActivityTypeSelector runat="server" ID="ucActivityType" />
                            </div>
                        </asp:Panel>
                        <cms:FormField runat="server" ID="tMacroCondition" Field="TriggerMacroCondition" UseFFI="true"
                            FormControl="ConditionBuilder" ResourceString="ma.trigger.macrocondition" DisplayColon="true" />
                    </cms:FormCategory>
                    <cms:FormCategory runat="server" ID="pnlScoreProperties" CategoryTitleResourceString="ma.trigger.scoreGroup">
                        <div class="form-group">
                            <div class="editing-form-label-cell">
                                <cms:FormLabel CssClass="control-label" ID="lblScoreType" runat="server" EnableViewState="false" ResourceString="scoreselect.itemname"
                                    DisplayColon="true" />
                            </div>
                            <div class="editing-form-value-cell">
                                <cms:ScoreTypeSelector runat="server" ID="ucScoreType" />
                            </div>
                        </div>
                        <cms:ScoreProperties ID="ucScoreProperties" runat="server" />
                    </cms:FormCategory>
                    <cms:FormSubmit runat="server" ID="wSubmit" />
                </LayoutTemplate>
            </cms:UIForm>
        </ContentTemplate>
    </cms:CMSUpdatePanel>
</asp:Content>