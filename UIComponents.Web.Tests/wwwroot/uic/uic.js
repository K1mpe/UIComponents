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




﻿var uic = uic || {};

uic.modal = uic.modal || {
    help: function () {
        console.log(".trigger('uic-hide') => triggers the modal to hide");
        console.log(".on('uic-before-hide', function()) => runs before the modal can hide, returning false will disable de modal to hide");
        console.log(".on('uic-hidden', function()) => triggered after the modal has hidden.");
    },

    closeParent: function (element) {
        var modal = $(item).closest('.uic.modal');
        if (modal.length) {
            modal.trigger('uic-hide');
            return true;
        }
        modal = $(item).closest('.modal');
        if (modal.length) {
            modal.modal('hide');
            return true;
        }

        var popup = $(item).closest('.uic-can-hide');
        if (popup.length) {
            window.close();
            return true;
        }

        return false;
    },

};


$(document).ready(function () {
    $(document).on('uic-help', '.uic.modal', function () {
        uic.modal.help();
    });

    $(document).on('uic-hide', '.uic.modal', async function () {
        let beforeHideResult = await $(this).on('uic-before-hide');
        if (beforeHideResult === false)
            return;

        $(this).modal('hide');
        $(this).trigger('uic-hidden');
    })
});

﻿var uic = uic || {};


//Transform input elements to make the inputs of this form not look like input fields
uic.formReadonly = function (form, showEmptyInReadonly = true, showSpanInReadonly = true, showDeleteButton = false) {
    form = $(form);
    form.find('[readonly]')
        .addClass("always-readonly");
    form.find('.form-control')
        .addClass("form-control-plaintext")
        .removeClass("form-control")
        .attr("readonly", true)
        .attr("disabled", true);
    form.find('input[type=checkbox]')
        .attr("readonly", true)
        .attr("disabled", true);
    form.find('.select2-container')
        .removeClass("select2-container--bootstrap4")
        .find('.select2-selection__rendered').addClass('px-0');


    form.find('.cdc-select-btn-add').addClass("d-none");
    form.find('.btn-save').attr('hidden', true);
    form.find('.btn-readonly').attr('hidden', true);
    form.find('.btn-edit').attr('hidden', false);
    form.find('label > span').attr('hidden', true);

    

    if(!showEmptyInReadonly)
        form.find('.input-no-value').addClass('d-none');

    if(!showSpanInReadonly)
        form.find('span:not(.card-header span, .select2, .select2 span)').attr('hidden', true);

    if (!showDeleteButton)
        form.find('.btn-delete').attr('hidden', true);
}


//This function is to undo the uic.FormReadonly function, usefull for a edit button
uic.formEditable = function (form) {
    form = $(form);
    form.find('.form-control-plaintext:not(.always-readonly)')
        .addClass("form-control")
        .removeClass("form-control-plaintext")
        .attr("readonly", false)
        .attr("disabled", false);

    form.find('.select2-container:not(.always-readonly)')
        .addClass("select2-container--bootstrap4")
        .find('.select2-selection__rendered').removeClass('px-0');

    form.find('input[type=checkbox]:not(.always-readonly)')
        .attr("readonly", false)
        .attr("disabled", false);




    form.find('.cdc-select-btn-add').removeClass("d-none");
    form.find('.btn-confirm').attr('hidden', false);
    form.find('.btn-readonly').attr('hidden', false);
    form.find('.btn-edit').attr('hidden', true);
    form.find('.input-no-value').removeClass('d-none');
    form.find('label > span').attr('hidden', false);
    form.find('span').attr('hidden', false);

    form.find('.btn-save').attr('hidden', false);
    form.find('.btn-delete').attr('hidden', false);
}

//This function will remove all "." from the input value
uic.removeDecimals = function (element) {
    element = $(element);
    var value = element.val();
    element.val(value.replaceAll(".", ""));
}

//This function will set a treestateboolean in the correct state according to its data-value attribute (true, false, null)
uic.setThreeState = function (item){
    var value = item.data('value');
    if (value == null)
        value = "null";
    item.removeClass('indeterminate');
    if (value.toString() === "false") {
        item.prop('checked', false);
        item[0].indeterminate = false;
    } else if (value.toString() == "true") {
        item.prop('checked', true);
        item[0].indeterminate = false;
    } else if (value == null || value.toString() == "null" || value == "") {
        item.prop('checked', null);
        item.addClass('indeterminate');
        item[0].indeterminate = true;
    }
}


uic.getValue = function (element) {

    //If you create a function on a element like this
    //  $().on('GetValue', function () {
    //      return "value";
    //  });
    // This will overwrite the default behaviour of this function.
    var result = $(element).triggerHandler('GetValue');
    if (result != undefined)
        return result;


    var type = $(element).attr('type');
    var tag = $(element).prop('tagName');
    var name = $(element).attr('name');
    //if (name != undefined && name != null && name.includes(".")) {
    //    var parts = name.split(".");
    //    name = parts[parts.length - 1];
    //}
        
    var arrayElements = uic.GetProperties(element).filter(`[name="${name}"][data-array-index]`);
    if (arrayElements.length) {
        var array = [];
        arrayElements.each(function (index, item){
            array[index] = uic.GetValue(item);
        })
        return array;
    }

    if ($(element).hasClass('three-state-checkbox')) {
        var threeStateVal = $(element).data('value');
        if (threeStateVal == "true")
            return true;
        if (threeStateVal == "false")
            return false;
        return null;
    }
    else if (type == "checkbox") {
        return $(element).prop('checked');

    } else if (tag == "INPUT" || tag == "SELECT" || tag == "TEXTAREA") {
        return $(element).val();

    }

    //Get object with child properties
    var properties = uic.GetProperties(element);
    var value = {}
    properties.each(function (index, item) {
        var property = $(item).attr('name');
        value[property] = uic.GetValue(item);
    });
    return value;
}


uic.setValue = function (element, value) {
    //If you create a function on a element like this
    //  $().on('SetValue', function (e, value) {
    //      ...;
    //  });
    // This will overwrite the default behaviour of this function.
    if (!$(element).length)
        return;

    if ($._data($(element).get(0), 'events') != undefined && $._data($(element).get(0), 'events')["SetValue"] != undefined) {
        $(element).trigger('SetValue', value);
        return;
    }

    if (Array.isArray(value)) {
        var name = $(element).attr('name');
        value.forEach(function (val, index) {
            console.log(index, val);

            var subElement = $(element).find(`[name="${name}"][data-array-index=${index}]`);
            uic.SetValue(subElement, val);
        });
        
    }
    else if (typeof value == "object" && value != null) {
        var properties = uic.GetProperties(element);
        var valueProps = Object.getOwnPropertyNames(value);
        valueProps.forEach(function (item, index) {
            //console.log(index, item);
            var property = properties.filter(`[name="${item}"]`);
            uic.SetValue(property, value[item]);
        });

    } else {

        var type = $(element).attr('type');
        var tag = $(element).prop('tagName');

        if ($(element).hasClass('three-state-checkbox')) {
            $(element).data('value', value);
            uic.SetThreeState(element);
        }
        else if (type == "checkbox") {
            $(element).prop('checked', value);

        } else {
            
            $(element).val(value);
        }
    }
}

uic.clearValues = function (object) {
    for (let [key, val] of Object.entries(object)) {
        if (val instanceof Object) {
            object[key] = uic.ClearValues(val);
        } else {
            object[key] = null;
        }
    }
    return object;
}

//This function gets all child elements with the name property, but not recusivly
uic.getProperties = function (element) {
    $(element).addClass('uic-find-subnames');
    var results = $(element).find('[name]');

    //This filter is to make sure that only the direct children with a name are selected.
    results = results.filter(function () {
        var parentWithName = $(this).parent().closest('[name], .uic-find-subnames');
        return parentWithName.hasClass('uic-find-subnames');
    });

    $(element).removeClass('uic-find-subnames');
    return results;

}


//This function will return a true or false if the objects match.
// comparison => The object you would like to check
// objectMatch => results in true if all equal properties with comparison have the same value.
// objectMissMatch => results in false if any property has a match with comparison. 
uic.compareObjects = function (comparison, objectMatch, objectMissMatch = {}) {

    var comparisonProps = Object.getOwnPropertyNames(objectMissMatch);

    //Validate missmatch
    for (var i = 0; i < comparisonProps.length; i++) {
        var prop = comparisonProps[i];

        //Check if entity has property
        if (comparison.hasOwnProperty(prop))


            //get props from entity and comparison in lowercase to compare
            var e = "";
            var c = "";
            try {
                e = comparison[prop].toString() || "";
            } catch { }
            try {
                c = objectMissMatch[prop].toString() || "";
            } catch { }

            //if comparison contains *, replace * with any possible
            if (c.includes("*")) {
                var cparts = c.split("*");

                //e must contain each part of c
                for (var j = 0; j < cparts.length; j++) {
                    var part = cparts[j];
                    if (!e.includes(part)) {
                        continue;
                    }
                }
                //e must start with first part of c
                if (!e.startsWith(cparts[0]))
                    continue;

                //e must end with last part of c
                if (!e.endsWith(cparts[cparts.length - 1]))
                    continue;

                //if no * is used, exact match is required
            } else {
                if (e != c)
                    continue;
            }
            return false;
        }


    comparisonProps = Object.getOwnPropertyNames(objectMatch);
    //Validate match
    for (var i = 0; i < comparisonProps.length; i++) {
        var prop = comparisonProps[i];

        //Check if entity has property
        if (comparison.hasOwnProperty(prop))


            //get props from entity and comparison in lowercase to compare
            var e = "";
        var c = "";
        try {
            e = comparison[prop].toString() || "";
        } catch { }
        try {
            c = objectMatch[prop].toString() || "";
        } catch { }

        //if comparison contains *, replace * with any possible
        if (c.includes("*")) {
            var cparts = c.split("*");

            //e must contain each part of c
            for (var j = 0; j < cparts.length; j++) {
                var part = cparts[j];
                if (!e.includes(part)) {
                    return false;
                }
            }
            //e must start with first part of c
            if (!e.startsWith(cparts[0]))
                return false;

            //e must end with last part of c
            if (!e.endsWith(cparts[cparts.length - 1]))
                return false;

            //if no * is used, exact match is required
        } else {
            if (e != c)
                return false;
        }
    }

    return true;
}


//This function disables all the selectlistitems that have the same value as the other selects.
// Warning: Do not use this function on diffrent selectlists, since they can disable eachothers ids.
// Warning: Do not use this function if some selectlistoptions are already disabled, if these values are not used in any of the selects, the become enabled again.
uic.disableUsedListItems = function (...selects) {

    updateListItems = function (selects) {
        $(selects).find('option').removeAttr('disabled');
        $(selects).each(function (index, select) {
            var value = uic.GetValue($(select));
            var values = [];

            if (Array.isArray(value)) {
                values = value;
            } else {
                values.push(value);
            }
            for (var i = 0; i < selects.length; i++) {
                if (i == index)
                    continue;

                values.forEach(function (val, j) {
                    $(selects[i]).find(`option[value=${val}]`).attr('disabled', true);
                })
                
            }
        })
    }


    if (selects[0] instanceof jQuery)
        selects = selects[0].toArray();

    //set the onchange event
    selects.forEach(function (value) {
        $(value).on('change', function () {
            updateListItems(selects);
        });
    })

   
}
