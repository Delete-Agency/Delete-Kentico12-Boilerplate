<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DropdownChosen.ascx.cs" Inherits="CMSFormControls_DropdownChosen" %>

<div class="custom-dropdown">
    <asp:ListBox runat="server" ID="lbOptions" CssClass="" />
    <asp:HiddenField runat="server" ID="hdSelectedOptions" />
</div>

<script type="text/javascript">
    $(document).ready(function () {
        var select = $('#<%# lbOptions.ClientID %>');
        <%# Readonly ? "select.attr('disabled', 'disabled');" : "" %>
        select.chosen({
            allow_single_deselect: true,
            width: "100%"
        }).on('change', function (e, params) {
        if (<%# AllowMultiple ? "true" : "false" %>) {
                var selectedOptions = $(this).getSelectionOrder();
                if (params.deselected && params.deselected !== 'undefined') {
                    for (var i = selectedOptions.length - 1; i >= 0; i--) {
                        if (selectedOptions[i] === params.deselected) {
                            selectedOptions.splice(i, 1);
                        }
                    }
                }
                $('#<%# hdSelectedOptions.ClientID %>').val(selectedOptions.join('|'));
            } else {

                var selectedParams = "";

                if (params) {
                    selectedParams = params.selected;
                }

                $('#<%# hdSelectedOptions.ClientID %>').val(selectedParams);
            }
        });
        if (<%# AllowMultiple ? "true" : "false" %>) {
            var selection = $('#<%# hdSelectedOptions.ClientID %>').val();
            if (selection) {
                var selectionOrder = selection.split('|');
                $('#<%# lbOptions.ClientID %>').setSelectionOrder(selectionOrder);
            }
        }
    });
</script>
