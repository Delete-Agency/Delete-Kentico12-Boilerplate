<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TreeTaxonomyMultiSelector.ascx.cs" Inherits="CMSFormControls_TreeTaxonomyMultiSelector" %>


<div id="jstree-container">
    <div>
        <input type="text" class="form-control" name="selectedNames" readonly />
    </div>
    <div id="jstree"></div>
    <input type="hidden" ID="hfSelectedTaxonomies"  class="tax-input" runat="server" />
</div>

<script type="text/javascript">
    (function () {
        $(document).ready(function() {
            $('#jstree-container:not(.jstree-inited)').each(function() {
                var treeItem = $('#jstree', this);
                var selectedValuesItem = $('input[name*="selectedNames"]', this);
                var treeContainer = $(this);

                var results = <%= TaxonomyTree %>;
                var ids = $('.tax-input', treeContainer).val();
                if (ids) {
                    var selected = JSON.parse(ids);
                    var selectedValues = [];
                    results.forEach(function(t) {
                        if (selected.indexOf(t.id) >= 0) {
                            t.state = { selected: true };
                            selectedValues.push(t.text);
                        }
                    });
                    $(selectedValuesItem).val(selectedValues.join(','));
                }
                $(treeItem).jstree({
                    'plugins': ['search', 'checkbox', 'wholerow'],
                    'core': {
                        'data': results,
                        'animation': false,
                        'themes': {
                            'icons': false
                        }
                    }
                });

                $(treeItem).on('changed.jstree',
                    function(e, data) {
                        var objects = data.instance.get_selected(true);
                        var leaves = $.grep(objects, function(o) { return data.instance.is_leaf(o) });
                        var selectedValues = [];
                        var selected = [];
                        $.each(leaves,
                            function(i, o) {
                                selected.push(o.id);
                                selectedValues.push(o.text);
                            });
                        $('.tax-input', treeContainer).val(JSON.stringify(selected));
                        $(selectedValuesItem).val(selectedValues.join(','));
                    });
                $(this).addClass('jstree-inited');
            });

        });
    })();
</script>
