(function () {
    const originalInitialize = window.initialize;
    window.initialize = function (dotNetObjectRef, element, elementId, options) {
        const wrappedDotNetRef = {
            invokeMethodAsync: function (methodName, ...args) {
                if (methodName === 'UploadFile') {
                    return dotNetObjectRef.invokeMethodAsync(methodName, ...args)
                        .then(result => {
                            const fileEntry = args[0];
                            const url = fileEntry.uploadUrl ||
                                fileEntry.UploadUrl ||
                                fileEntry.uploadurl ||
                                fileEntry.UPLOADURL;
                            if (url && typeof url === 'string') {
                                return url;
                            } else {
                                return '';
                            }
                        });
                }
                return dotNetObjectRef.invokeMethodAsync(methodName, ...args);
            },
            invokeMethod: dotNetObjectRef.invokeMethod?.bind(dotNetObjectRef)
        };
        return originalInitialize.call(this, wrappedDotNetRef, element, elementId, options);
    };
})();