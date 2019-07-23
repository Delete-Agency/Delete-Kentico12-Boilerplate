﻿<%@ Control Language="C#" AutoEventWireup="true"
    Inherits="CMSModules_ImportExport_Controls_Import_Site_cms_document"  Codebehind="cms_document.ascx.cs" %>

<script type="text/javascript">
    //<![CDATA[
    function CheckChange() {
        for (i = 0; i < childIDs.length; i++) {
            var child = document.getElementById(childIDs[i]);
            if (child != null) {
                child.disabled = !ed_parent.checked;
                child.checked = ed_parent.checked;
            }
        }
    }

    function InitCheckboxes() {
        if (!ed_parent.checked) {
            for (i = 0; i < childIDs.length; i++) {
                var child = document.getElementById(childIDs[i]);
                if (child != null) {
                    child.disabled = true;
                }
            }
        }
    }
    //]]>
</script>

<div class="wizard-section">
    <div class="form-horizontal">
        <div class="checkbox-list-vertical">
            <asp:PlaceHolder runat="server" ID="pnlCheck">
                <cms:CMSCheckBox ID="chkDocuments" runat="server" />
            </asp:PlaceHolder>
            <div class="selector-subitem">
                <div class="checkbox-list-vertical">
                    <asp:PlaceHolder runat="server" ID="pnlDocumentData">
                        <cms:CMSCheckBox ID="chkDocumentsHistory" runat="server" />
                        <cms:CMSCheckBox ID="chkRelationships" runat="server" />
                        <cms:CMSCheckBox ID="chkACLs" runat="server" />
                        <cms:CMSCheckBox ID="chkUserPersonalizations" runat="server" />
                    </asp:PlaceHolder>
                    <asp:PlaceHolder runat="server" ID="pnlModules">
                        <cms:CMSCheckBox ID="chkBlogComments" runat="server" />
                        <cms:CMSCheckBox ID="chkEventAttendees" runat="server" />
                    </asp:PlaceHolder>
                </div>
            </div>
        </div>
    </div>
</div>
<asp:Literal ID="ltlScript" EnableViewState="false" runat="Server" />
