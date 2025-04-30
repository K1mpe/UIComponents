/* Format a string by passing arguments or an object
 * https://stackoverflow.com/a/18234317
 */
String.prototype.format = String.prototype.format || function () {
    'use strict';

    var str = this.toString();
    if (arguments.length) {
        var type = typeof arguments[0];
        var args = ('string' === type || 'number' === type) ? Array.prototype.slice.call(arguments) : arguments[0];
        for (var key in args) {
            str = str.replace(new RegExp("\\{" + key + "\\}", 'gi'), args[key]);
        }
    }

    return str;
};

var uic = uic || {};




uic.getValue = function (element) {
    element = $(element);
    if (!element.length)
        return null;
    //If you create a function on a element like this
    //  $().on('uic-getValue', function () {
    //      return "value";
    //  });
    // This will overwrite the default behaviour of this function.
    var result = $(element).triggerHandler('uic-getValue');
    if (result != undefined)
        return result;


    var type = $(element).attr('type');
    var tag = $(element).prop('tagName');
    var name = $(element).attr('name');
    //if (name != undefined && name != null && name.includes(".")) {
    //    var parts = name.split(".");
    //    name = parts[parts.length - 1];
    //}

    var arrayElements = uic.getProperties(element).filter(`[name="${name}"][data-array-index]`);
    if (arrayElements.length) {
        var array = [];
        arrayElements.each(function (index, item) {
            array[index] = uic.getValue(item);
        })
        return array;
    }

    if ($(element).hasClass('three-state-checkbox')) {
        var threeStateVal = $(element).data('value');
        if (threeStateVal == "true" || threeStateVal == true)
            return true;
        if (threeStateVal == "false" || threeStateVal == false)
            return false;
        return null;
    }
    else if (type == "checkbox") {
        return $(element).prop('checked');

    } else if (tag == "INPUT" || tag == "SELECT" || tag == "TEXTAREA") {
        return $(element).val();

    }

    //Get object with child properties
    var properties = uic.getProperties(element);
    var value = {}
    properties.each(function (index, item) {
        var property = $(item).attr('name');
        value[property] = uic.getValue(item);
    });
    return value;
}


uic.setValue = function (element, value) {
    element = $(element);
    //If you create a function on a element like this
    //  $().on('uic-setValue', function (e, value) {
    //      ...;
    //  });
    // This will overwrite the default behaviour of this function.
    if (!element.length)
        return;

    if ($._data(element.get(0), 'events') != undefined && $._data(element.get(0), 'events')["uic-setValue"] != undefined) {
        element.trigger('uic-setValue', value);
        return;
    }
    if (element.attr('name') == undefined) {
        let properties = uic.getProperties(element);
        if (properties.length == 1) {
            uic.setValue(properties, value);
            return;
        }
    }

    if (typeof value == "object" && value != null) {
        let properties = uic.getProperties(element);
        if (properties.length == 1) {
            uic.setValue(properties, value);
            return;
        }
        let valueProps = Object.getOwnPropertyNames(value);
        valueProps.forEach(function (item, index) {
            //console.log(index, item);
            let property = properties.filter(`[name="${item}"]`);
            uic.setValue(property, value[item]);
        });

    } else {

        let type = element.attr('type');
        let tag = element.prop('tagName');
        let isDisabled = element.attr('disabled') !== undefined;
        if (isDisabled) {
            element.attr('disabled', null);
        }
        if (element.hasClass('three-state-checkbox')) {
            element.data('value', value);
            uic.form.setThreeState(element);
        }
        else if (type == "checkbox") {
            element.prop('checked', value);

        } else {
            element.val(value);
        }
        if (isDisabled)
            element.attr('disabled', true);
        element.change();
        element.trigger('uic-valueSet');
    }
}

//This function acts simular to setValue, but does not (yet) replace the changed values from non-readonly properties.
//These changed properties are marked and will only be changed after the user clicks on them.
//This function is usefull for updating a item with signalR, without changing values without user noticing
uic.markChanges = function (element, newValue) {
    element = $(element);
    if (!element.length)
        return;

    let oldValue = uic.getValue(element);
    if (uic.stringify(oldValue) == uic.stringify(newValue))
        return;

    if (uic.elementContainsEvent(element, 'uic-markChanges')) {
        //If the markChanges returns true, the default function will not continue
        if (element.triggerHandler('uic-markChanges', [oldValue, newValue]))
            return;
    }




    if (Array.isArray(newValue)) {
        let name = $(element).attr('name');
        newValue.forEach(function (val, index) {
            console.log(index, val);

            let subElement = $(element).find(`[name="${name}"][data-array-index=${index}]`);
            uic.markChanges(subElement, val);
        });
        return;
    }
    if (typeof newValue == "object" && newValue != null) {
        let properties = uic.getProperties(element);
        let valueProps = Object.getOwnPropertyNames(newValue);
        valueProps.forEach(function (item, index) {
            //console.log(index, item);
            let property = properties.filter(`[name="${item}"]`);
            uic.markChanges(property, newValue[item]);
        });
        return;

    }
    uic.markChangesOptions.applyMark(element, oldValue, newValue);
}

uic.markChangesOptions = {
    applyMark: function (element, oldValue, newValue) {
        let mark = uic.markChangesOptions.markChangesIcon.clone();
        
        let elementId = element.attr('id');
        let visualElement = element;
        if (element.prop('tagName') == "SELECT") {
            let option = element.find(`option[value=${newValue}]`);
            if (option.length)
                uic.markChangesOptions.markChangesTooltip(element, oldValue, option.text().replaceAll('\n', '').trim()).then((result) => {
                    mark.attr('title', result);
                });
            let select2Span = element.next();
            if (select2Span.length && select2Span.hasClass('select2'))
                visualElement = select2Span;
        } else {
            uic.markChangesOptions.markChangesTooltip(element, oldValue, newValue).then((result) => {
                mark.attr('title', result);
            });
        }
        if (elementId.length) {
            $(`.uic-value-changed[data-for=${elementId}]`).remove();
            mark.attr('data-for', elementId);
        }
        uic.markChangesOptions.onClickMark(element, oldValue, newValue, visualElement, mark);

        visualElement.each((index, item) => {
            item = $(item);
            if (item.attr('readonly')) {
                uic.setValue(item, newValue);
                return;
            }

            item.addClass('uic-value-changed');

            let label = $(`label[for="${item.attr('id')}"]`);
            if (label.length)
                label.append(mark);
            else
                item.before(mark);
        })
    },
    markChangesIcon: $('<i>', { class: 'fas fa-triangle-exclamation uic-value-changed' }),
    markChangesTooltip: async function (element, oldValue, newValue) {
        let translatable = {
            ResourceKey: "UIC.MarkChanges",
            DefaultValue: "Value has changed to {0}",
            Arguments: [newValue]
        };
        return await uic.translation.translate(translatable);
    },
    onClickMark: function (element, oldValue, newValue, visualElement, mark) {
        mark.click(async (ev) => {
            ev.stopPropagation();
            let modal = $('<div>', { class: "modal fade show" })
                .append($('<div>', { class: "modal-dialog" })
                    .append($('<div>', { class: "modal-content" })
                        .append($('<div>', { class: "modal-body" })
                            .append($('<div>', { class: "row" })))));


            let cloneLeft = element.clone().removeClass('uic-value-changed select2-hidden-accessible').attr('id', '');
            let cloneRight = element.clone().removeClass('uic-value-changed select2-hidden-accessible').attr('id', '');
            if (uic.elementContainsEvent(element, 'uic-getClone')) {
                cloneLeft = await element.triggerHandler('uic-getClone');
                cloneRight = await element.triggerHandler('uic-getClone');
            } else if (uic.elementContainsEvent(element.parent(), 'uic-getClone')) {
                cloneLeft = await element.parent().triggerHandler('uic-getClone');
                cloneRight = await element.parent().triggerHandler('uic-getClone');
            }
            let translations = [
                TranslatableSaver.Save("MarkChanges.CurrentValue"),
                TranslatableSaver.Save("MarkChanges.ServerValue")
            ];
            let translationResults = await uic.translation.translateMany(translations);
            let colLeft = $('<div>', { class: "col old-val" }).append(cloneLeft)
                .append($('<button>', { class: "btn btn-default mt-2" }).text(translationResults["MarkChanges.CurrentValue"]).on('click', (ev) => {
                    let value = uic.getValue(colLeft);
                    uic.setValue(element, value[elName]);
                    uic.markChangesOptions.removeMark(element, visualElement, mark);
                    modal.modal('hide');
                }));
            let colRight = $('<div>', { class: "col new-val" }).append(cloneRight)
                .append($('<button>', { class: "btn btn-default mt-2" }).text(translationResults["MarkChanges.ServerValue"]).on('click', (ev) => {
                    let value = uic.getValue(colRight);
                    uic.setValue(element, value[elName]);
                    uic.markChangesOptions.removeMark(element, visualElement, mark);
                    modal.modal('hide');
                }));


            let elName = element.attr('name');
            setTimeout(() => {
                uic.setValue(colLeft, oldValue);
                uic.setValue(colRight, newValue);
            }, 1);
            modal.find('.row')
                .append(colLeft)
                .append(colRight)
            modal.modal({
                show: true
            });
            $('body').append(modal);
            //uic.setValue(element, newValue);
            //visualElement.removeClass('uic-value-changed');
            //mark.remove();
        });
    },
    removeMark: function (element, visualElement, mark) {
        visualElement.removeClass('uic-value-changed');
        mark.remove();
    },
};

//Returns true or false if the element contains this event
uic.elementContainsEvent = function ($element, eventKey) {
    return ($._data($element.get(0), 'events') != undefined && $._data($element.get(0), 'events')[eventKey] != undefined)
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

};
//The color that is used for uic-help
uic.consoleColor = function () {
    let style = window.getComputedStyle(document.body);
    let color = style.getPropertyValue('--custom') || '#663399';
    return color;
};

//This function will return a true or false if the objects match.
// comparison => The object you would like to check
// objectMatch => results in true if all equal properties with comparison have the same value.
// objectMissMatch => results in false if any property has a match with comparison. 
uic.compareObjects = function (comparison, objectMatch, objectMissMatch = {}) {

    if (!$.isPlainObject(objectMissMatch) && objectMissMatch == comparison)
        return false;

    if (!$.isPlainObject(objectMatch))
        return uic.stringify(comparison) == uic.stringify(objectMatch);

    var comparisonProps = Object.getOwnPropertyNames(objectMissMatch);

    //Validate missmatch
    for (var i = 0; i < comparisonProps.length; i++) {
        var prop = comparisonProps[i];

        //Check if entity has property
        if (comparison.hasOwnProperty(prop)) {
            //get props from entity and comparison in lowercase to compare
            var e = "";
            var c = "";
            try {
                e = uic.stringify(comparison[prop]) || "";
            } catch { }
            try {
                c = uic.stringify(objectMissMatch[prop]) || "";
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
            e = uic.stringify(comparison[prop]) || "";
        } catch { }
        try {
            c = uic.stringify(objectMatch[prop]) || "";
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
            var value = uic.getValue($(select));
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
//Load a script on the page using javascript.
//The readyCallback is executed after the script has finished loading.
//if reloadExisting is true, the script will be reloaded even if the page already contains the script
uic.loadScript = function (source, readyCallback = null, reloadExisting = false) {
    if (!reloadExisting) {
        let existing = $(`script[src="${source}"]`);
        if (existing.length) {
            if (readyCallback != null)
                readyCallback();
            return;
        }
    }
    let s, r;
    s = document.createElement('script');
    s.type = 'text/javascript';
    s.src = source;

    if (readyCallback != null) {
        s.onload = s.onreadystatechange = function () {
            console.log(this.readyState); //uncomment this line to see which ready states are called.
            if (!r && (!this.readyState || this.readyState == 'complete')) {
                r = true;
                readyCallback();
            }
        };
    }

    if (document.body == null)
        document.body = document.createElement("body");
    document.body.append(s);
};
//Load a stylesheet on the page using javascript.
//if reloadExisting is true, the script will be reloaded even if the page already contains the script
uic.loadStyle = function (source, reloadExisting = false) {
    if (!reloadExisting) {
        let existing = $(`link[href="${source}"]`);
        if (existing.length) {
            return;
        }
    }
    let s, r;
    s = document.createElement('link');
    s.rel = 'stylesheet';
    s.href = source;

    if (document.body == null)
        document.body = document.createElement("body");
    document.body.append(s);
};

uic.getResultOrInvoke = async function (result, ...args) {
    if (typeof result == 'function')
        return await result.apply(this, args);
    return result;
}

//If input is a string, output as string, else stringify
uic.stringify = function (input) {
    if (typeof input == 'string')
        return input;
    return JSON.stringify(input);
}

//If the input is a stringified json, parse it to object. Else, return the input
uic.parse = function (input) {
    try {
        if (typeof input != 'string')
            return input;
        if (input.startsWith('{') && input.endsWith('}'))
            return JSON.parse(input);

        if (input.startsWith('[') && input.endsWith(']'))
            return JSON.parse(input);
        return input;
    } catch {
        return input;
    }
    
}

