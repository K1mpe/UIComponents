uic.getpost = uic.getpost || {
    defaultOptions : {
        get : {
            cancelPreviousRequests: true,
            loadType: null,
            handlers: [],
            catch: function (...ex) {
                console.log('Failed! Server error', ex);
                if (ex[0].responseJSON != null)
                    return ex[0].responseJSON;
                return { type: 'Exception', exception: ex };
            }
        },

        post : {
            cancelPreviousRequests: false,
            loadType: null,
            handlers: [],
            catch: function (...ex) {
                console.log('Failed! Server error', ex);
                if (ex[0].responseJSON != null)
                    return ex[0].responseJSON;
                return { type: 'Exception', exception: ex };
            }
        },
    },
    defaultHandlers: {
        get : [],
        post : [],
        both : [
            (response) => {
                let jsonResponse = uic.parse(response);
                if (jsonResponse.Type == "ValidationErrors") {
                    $('span.text-danger').each(function () {
                        if ($(this).text() != "*")
                            $(this).text("");
                    });
                    let errors = [];
                    jsonResponse.Errors.forEach((item) => {
                        let propertyName = item.PropertyName;
                        let error = item.Error;
                        let spanElement = uic.validation._getValidationSpan(propertyName, $(`form[action="${jsonResponse.Url}"]`));
                        spanElement.text(error);
                        errors.push(error)
                    })
                    let error = errors.join('<br />');
                    makeToast('error', null, error, { timeOut: 30000, closeButton: true, progressBar: true, extendedTimeOut: 5000 });
                    return false;
                }
            },
            async (response) => {
                let jsonResponse = uic.parse(response);
                if (jsonResponse.Type == "ToastResponse") {
                    let level;
                    switch (jsonResponse.Notification.Type) {
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
                            if (jsonResponse.Data != null && jsonResponse.Data != undefined)
                                console.error(jsonResponse.Data);
                            break;
                    }
                    let message = await uic.translation.translate(jsonResponse.Notification.Message);
                    if (message == "null")
                        message = "";
                    let title = await uic.translation.translate(jsonResponse.Notification.Title);
                    if (title == "null")
                        title = "";

                    makeToast(level, message, title, { timeOut: jsonResponse.Duration * 1000, closeButton: true, progressBar: true, preventDuplicates: true, }); //TOASTR
                    if (level === 'Success') {
                        if (jsonResponse.Data != null)
                            return jsonResponse.Data;
                        return true;
                    }
                    return false;
                }
            },
            async (response) => {
                let jsonResponse = uic.parse(response);
                if (jsonResponse.Type == "AccessDenied") {
                    let message = await uic.translation.translate(jsonResponse.Message);
                    makeToast("Error", "", message)
                    return false;
                }
            },
            (response) => {
                let jsonResponse = uic.parse(response);
                if (jsonResponse.type == "Exception") {
                    let text = jsonResponse.exception[0].responseText;
                    if (text.length > 255 || !text.length)
                        text = "A error occured";
                    //makeToast("Error", "", text);
                    return false;
                }
            }, 
            (response) => {
                let jsonResponse = uic.parse(response);
                if (jsonResponse.Type == "Redirect") {

                    if (jsonResponse.Url == undefined || jsonResponse.Url == null || jsonResponse.Url.length)
                        location.reload();
                    else
                        location.href = jsonResponse.Url;
                    return true;
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

        uic.getpost._getRequests[url] = $.get(url, data, null, options.loadType).catch(options.catch);

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

        uic.getpost._postRequests[url] = $.post(url, data, null, options.loadType).catch(options.catch);

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
