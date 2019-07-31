(function () {
    window.kentico.pageBuilder.registerInlineEditor("richtext-editor", {
        init: function (options) {
            var editor = options.editor;
            var config =
            {
                placeholder: editor.dataset.placeholder,
                allowedContent: true
            };
            if (editor.dataset.enableformatting === 'false') {
                config.toolbar = [];
            } else {
                config.stylesSet = JSON.parse(editor.dataset.styles);
            }

            var ckeditor = CKEDITOR.inline(editor,config);
            ckeditor.on('change', function (evt) {
                var value = evt.editor.getData();
                if (editor.dataset.enableformatting === 'false')
                    value= evt.editor.getData().replace(/<\/?[^>]+(>|$)/g, "");
                var event = new CustomEvent("updateProperty",
                    {
                        detail: {
                            name: options.propertyName,
                            value: value,
                            refreshMarkup: false
                        }
                    });

                editor.dispatchEvent(event);
            });
           
        },

        destroy: function (options) {
        },

        dragStart: function (options) {
        }
    });
})();
