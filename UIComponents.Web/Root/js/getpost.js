var uic = uic || {};

uic.getpost = uic.getpost || {
    defaultOptions : {
        get : {
            cancelPreviousRequests: true,
            handlers : []
        },

        post : {
            cancelPreviousRequests: false,
            handlers : []
        },
    },
    defaultHandlers: {
        get : [],
        post : [],
        both : [
            (response) => {
                if (response.type == "ValidationErrors") {
                    $('span.text-danger').each(function () {
                        if ($(this).text() != "*")
                            $(this).text("");
                    });
                    response.errors.forEach((item) => {
                        var propertyName = item.PropertyName;
                        var errors = item.Errors;

                        var spanElement = $(`span.field-validation-valid[data-valmsg-for="${propertyName}"]`);
                        if (!spanElement.length)
                            spanElement = $(`span.field-validation-valid[data-valmsg-for$=".${propertyName}"]`);
                        if (!spanElement.length) {
                            var spanElement = $(`span[for=${propertyName}]`);
                            spanElement.removeClass();
                            spanElement.addClass("text-danger");
                        }
                        spanElement.text(errors);
                        makeToast('error', null, errors, { timeOut: 60000, closeButton: true, progressBar: true, extendedTimeOut: 10000 });
                    })
                    return false;
                }
            },
            (response) => {
                if (response.type == "ToastResponse") {
                    var level;
                    switch (response.notification.Type) {
                        case 1:
                            level = "Success";
                            break;
                        case 2:
                            level = "Info";
                            break;
                        case 3:
                            level = "Warning";
                            break;
                        case 4:
                            level = "Error";
                            console.error(response.data);
                            break;
                    }
                    let message = response.notification.Message.DefaultValue.format(response.notification.Message.Arguments);
                    if (message == "null")
                        message = "";
                    let title = response.notification.Title.DefaultValue.format(response.notification.Title.Arguments);
                    if (title == "null")
                        title = "";

                    makeToast(level, message, title, { timeOut: response.Duration * 1000, closeButton: true, progressBar: true, preventDuplicates: true, }); //TOASTR
                    if (level === 'Success') {
                        if (response.data != null)
                            return response.data;
                        return true;
                    }
                    return false;
                }
            },
            (response) => {
                if (response.type == "AccessDenied") {
                    makeToast("Error", "", response.Message)
                    return false;
                }
            },
            (response) => {
                if (response.type == "Exception") {
                    let text = response.exception[0].responseText;
                    if (text.length > 255 || !test.length)
                        text = "A error occured";
                    makeToast("Error", "", text);
                    return false;
                }
            }
        ],
    },

    

    //Perform a get request
    //url => the url where to get the data
    //data => data added in the request
    //additional options. This extends the uic.getpost.defaultOptions
    get : async function (url, data, options = {}) {
        options = $.extend({}, uic.getpost.defaultOptions.get, options);

        try {
            if (options.cancelPreviousRequests && uic.getpost._getRequests[url] != undefined) {
                uic.getpost._getRequests[url].abort();
            }
        } catch { }

        uic.getpost._getRequests[url] = $.get(url, data).catch(function (...ex) {
            console.log('Failed! Server error', ex);
            return { type: 'Exception', exception: ex };
        });

        let response = await uic.getpost._getRequests[url];
        uic.getpost._getRequests[url] = undefined;

        let handlers = options.handlers.concat(uic.getpost.defaultHandlers.get).concat(uic.getpost.defaultHandlers.both);

        return await uic.getpost.handleResponse(handlers, response);
    },

    post : async function (url, data, options = {}) {
        options = $.extend({}, uic.getpost.defaultOptions.get, options);

        var data = uic.getpost.formatNumbersForDecimalStrings(data);

        try {
            if (options.cancelPreviousRequests && uic.getpost._postRequests[url] != undefined) {
                uic.getpost._postRequests[url].abort();
            }
        } catch { }

        uic.getpost._postRequests[url] = $.post(url, data).catch(function (...ex) {
            console.log('Failed! Server error', ex);
            return { type: 'Exception', exception: ex };
        });

        let response = await uic.getpost._postRequests[url];
        uic.getpost._postRequests[url] = undefined;

        let handlers = options.handlers.concat(uic.getpost.defaultHandlers.post).concat(uic.getpost.defaultHandlers.both);

        return await uic.getpost.handleResponse(handlers, response);
    },

    handleResponse : async function (handlers, response) {
        for (var i = 0; i < handlers.length; i++) {
            let handler = handlers[i];
            var handlerResult = await handler(response);
            if(handlerResult != null && handlerResult != undefined)
                return handlerResult;
        };
        return response;
    },

    formatNumbersForDecimalStrings : function (data) {
        if (data == null)
            return null;
        if (typeof data == "string") {
            //data = data.replaceAll(".", ",");
        } else {
            var postProperties = Object.getOwnPropertyNames(data);
            for (var i = 0; i < postProperties.length; i++) {
                var prop = postProperties[i];
                var val = data[prop];
                if (!isNaN(val) && val !== null)
                    data[prop] = val.toString().replace(".", ",");
                else if (typeof val == "object")
                    data[prop] = uic.getpost.formatNumbersForDecimalStrings(val);
            }
        }
        return data;
    },

    _getRequests :{},
    _postRequests : {},



};




