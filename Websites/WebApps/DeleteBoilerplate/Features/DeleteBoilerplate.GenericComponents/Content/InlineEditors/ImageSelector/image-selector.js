(function () {
    // Registers the 'image-selector' inline editor within the page builder scripts
    window.kentico.pageBuilder.registerInlineEditor("image-selector", {
        init: function (options) {
            var editor = options.editor;
            var button = editor.querySelector("button");

            button.addEventListener("click", function () {
                // Sets the options object with individual properties
                var dialogOptions = {
                    // libraryName: "DeleteBoilerplate",
                    maxFilesLimit: 1,
                    allowedExtensions: ".gif;.png;.jpg;.jpeg",
                    selectedValues: options.propertyValue,

                    // Defines the applyCallback function invoked on click of the selector's confirmation button
                    applyCallback: function (files) {
                        var newFile = files[0];

                        // Checks if the image isn't already selected
                        if (options.propertyValue && newFile.fileGuid === options.propertyValue[0].fileGuid) {
                            return {
                                closeDialog: true
                            };
                        }

                        // Creates a custom event that notifies the widget about a change in the value of a property
                        var event = new CustomEvent("updateProperty", {
                            detail: {
                                value: [{ fileGuid: newFile.fileGuid }],
                                name: options.propertyName
                            }
                        });

                        editor.dispatchEvent(event);

                        return {
                            closeDialog: true
                        };
                    }
                };

                // Opens the selector modal dialog
                window.kentico.modalDialog.mediaFilesSelector.open(dialogOptions);
            });
        }
    });
})();