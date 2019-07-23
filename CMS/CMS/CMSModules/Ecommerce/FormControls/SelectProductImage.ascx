﻿<%@ Control Language="C#" AutoEventWireup="true"  Codebehind="SelectProductImage.ascx.cs"
    Inherits="CMSModules_Ecommerce_FormControls_SelectProductImage" %>
<%@ Register TagPrefix="cms" TagName="File" Src="~/CMSModules/AdminControls/Controls/MetaFiles/File.ascx" %>
<%-- Messages --%>
<cms:MessagesPlaceHolder ID="plcMessages" runat="server" Visible="false" />
<%-- Meta file uploader --%>
<cms:File ID="metaFileElem" runat="server" ShortID="f" Visible="false" />
<%-- Image selector --%>
<cms:ImageSelector ID="imageSelectorElem" runat="server" ShortID="s" UseImagePath="true" ImageHeight="50"
    ShowImagePreview="true" ShowClearButton="true" Visible="false" />
