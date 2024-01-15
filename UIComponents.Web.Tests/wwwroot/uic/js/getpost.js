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
            if (options.cancelPreviousRequests && _getRequests[url] != undefined) {
                _getRequests[url].abort();
            }
        } catch { }

        _getRequests[url] = $.get(url, data).catch(function (...ex) {
            console.log('Failed! Server error', ex);
            return { type: 'Exception', exception: ex };
        });

        let response = await _getRequests[url];
        _getRequests[url] = undefined;

        let handlers = options.handlers.concat(defaultHandlers.get).concat(defaultHandlers.both);

        return await handleResponse(handlers, response);
    },

    post : async function (url, data, options = {}) {
        options = $.extend({}, uic.getpost.defaultOptions.get, options);

        var data = formatNumbersForDecimalStrings(data);

        try {
            if (options.cancelPreviousRequests && _postRequests[url] != undefined) {
                _postRequests[url].abort();
            }
        } catch { }

        _postRequests[url] = $.get(url, data).catch(function (...ex) {
            console.log('Failed! Server error', ex);
            return { type: 'Exception', exception: ex };
        });

        let response = await _postRequests[url];
        _postRequests[url] = undefined;

        let handlers = options.handlers.concat(defaultHandlers.post).concat(defaultHandlers.both);

        return await handleResponse(handlers, response);
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
                    data[prop] = formatNumbersForDecimalStrings(val);
            }
        }
        return data;
    },

    _getRequests :{},
    _postRequests : {},



};




