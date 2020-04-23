(function () {
    // Registers the 'select-editor' inline editor within the page builder scripts
    window.kentico.pageBuilder.registerInlineEditor("select-editor", {
        init: function (options) {
            var editor = options.editor;
            var selectOption = editor.querySelector("select");

            selectOption.addEventListener("change", function () {

                var event = new CustomEvent("updateProperty",
                    {
                        detail: {
                            name: options.propertyName,
                            value: this.value
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