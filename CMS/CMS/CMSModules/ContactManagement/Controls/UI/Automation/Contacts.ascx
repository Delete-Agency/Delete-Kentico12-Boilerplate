﻿<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_ContactManagement_Controls_UI_Automation_Contacts"  Codebehind="Contacts.ascx.cs" %>
<%@ Register Src="~/CMSAdminControls/UI/UniGrid/UniGrid.ascx" TagName="UniGrid" TagPrefix="cms" %>
<%@ Register Namespace="CMS.UIControls.UniGridConfig" TagPrefix="ug" Assembly="CMS.UIControls" %>
<cms:UniGrid runat="server" ID="listElem" ObjectType="ma.automationstate" OrderBy="StateStatus, StateStepID" Columns="StateID, StateStepID, StateObjectID, StateStatus, StateCreated, StateUserID"
    IsLiveSite="false" EditActionUrl="Process_Detail.aspx?stateid={0}&processid={?processid?}&issitemanager={?issitemanager?}" RememberStateByParam="issitemanager">
    <GridActions Parameters="StateID;StateObjectID">
        <ug:Action Name="edit" Caption="$ma.process.manage$" FontIconClass="icon-edit" FontIconStyle="Allow" />
        <ug:Action Name="view" ExternalSourceName="view" Caption="$om.contact.viewdetail$" FontIconClass="icon-eye" FontIconStyle="Allow" CommandArgument="StateObjectID" />
        <ug:Action Name="#delete" ExternalSourceName="delete" Caption="$autoMenu.RemoveState$" FontIconClass="icon-bin" FontIconStyle="Critical" ModuleName="CMS.OnlineMarketing" Permissions="RemoveProcess" />
    </GridActions>
    <GridColumns>
        <ug:Column Caption="$om.contact.firstname$" Source="StateObjectID" ExternalSourceName="#transform: om.contact : {%contactfirstname%}"  AllowSorting="false" Wrap="false">
            <Filter Type="text" Format="StateObjectID IN (SELECT ContactID FROM OM_Contact WHERE {3})" Source="ContactFirstName" Size="100" />
        </ug:Column>
        <ug:Column Caption="$om.contact.lastname$" Source="StateObjectID" ExternalSourceName="#transform: om.contact : {%contactlastname%}" AllowSorting="false" Wrap="false">
            <Filter Type="text" Format="StateObjectID IN (SELECT ContactID FROM OM_Contact WHERE {3})" Source="ContactLastName" Size="100" />
        </ug:Column>
        <ug:Column Caption="$general.emailaddress$" Source="StateObjectID" ExternalSourceName="#transform: om.contact : {%contactemail%}" AllowSorting="false" Wrap="false">
            <Filter Type="text" Format="StateObjectID IN (SELECT ContactID FROM OM_Contact WHERE {3})" Source="ContactEmail" Size="100" />
        </ug:Column>
        <ug:Column Caption="$Unigrid.Automation.Columns.StepName$" Source="StateStepID" ExternalSourceName="#transform: cms.workflowstep.stepdisplayname" AllowSorting="false" Wrap="false">
            <Filter Type="text" Format="StateStepID IN (SELECT StepID FROM CMS_WorkflowStep WHERE {3})" Source="StepDisplayName" Size="100" />
        </ug:Column>
        <ug:Column Caption="$Unigrid.Automation.Columns.StateStatus$" Source="StateStatus" ExternalSourceName="StateStatus" Wrap="false">
            <Filter Type="custom" Path="~/CMSModules/Automation/FormControls/ProcessStatusSelector.ascx" />
        </ug:Column>
        <ug:Column Caption="$general.site$" Wrap="false" AllowSorting="false" ExternalSourceName="#transform: om.contact :{%site.sitedisplayname|(default){$general.global$}%}" Source="StateObjectID" Localize="true" />
        <ug:Column Source="StateCreated" Caption="$Unigrid.Workflow.Columns.InitiatedWhen$" Wrap="false" />
        <ug:Column Source="StateUserID" Caption="$Unigrid.Workflow.Columns.InitiatedBy$" ExternalSourceName="#formattedusername|{$ma.automationstate.automatically$}" Wrap="false" />
        <ug:Column CssClass="filling-column" />
    </GridColumns>
    <GridOptions DisplayFilter="true" />
</cms:UniGrid>
