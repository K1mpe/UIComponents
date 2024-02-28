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

    //If you create a function on a element like this
    //  $().on('GetValue', function () {
    //      return "value";
    //  });
    // This will overwrite the default behaviour of this function.
    var result = $(element).triggerHandler('getValue');
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
        arrayElements.each(function (index, item){
            array[index] = uic.getValue(item);
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
    var properties = uic.getProperties(element);
    var value = {}
    properties.each(function (index, item) {
        var property = $(item).attr('name');
        value[property] = uic.getValue(item);
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

    if ($._data($(element).get(0), 'events') != undefined && $._data($(element).get(0), 'events')["setValue"] != undefined) {
        $(element).trigger('setValue', value);
        return;
    }

    if (Array.isArray(value)) {
        var name = $(element).attr('name');
        value.forEach(function (val, index) {
            console.log(index, val);

            var subElement = $(element).find(`[name="${name}"][data-array-index=${index}]`);
            uic.setValue(subElement, val);
        });
        
    }
    else if (typeof value == "object" && value != null) {
        var properties = uic.getProperties(element);
        var valueProps = Object.getOwnPropertyNames(value);
        valueProps.forEach(function (item, index) {
            //console.log(index, item);
            var property = properties.filter(`[name="${item}"]`);
            uic.setValue(property, value[item]);
        });

    } else {

        var type = $(element).attr('type');
        var tag = $(element).prop('tagName');

        if ($(element).hasClass('three-state-checkbox')) {
            $(element).data('value', value);
            uic.form.setThreeState(element);
        }
        else if (type == "checkbox") {
            $(element).prop('checked', value);

        } else {
            $(element).val(value);
        }
        $(element).change();
    }
}

//This function acts simular to setValue, but does not (yet) replace the changed values from non-readonly properties.
//These changed properties are marked and will only be changed after the user clicks on them.
//This function is usefull for updating a item with signalR, without changing values without user noticing
uic.markChanges = function (element, newValue) {
    if (!$(element).length)
        return;
        

    if ($._data($(element).get(0), 'events') != undefined && $._data($(element).get(0), 'events')["setValue"] != undefined) {
        let oldValue = uic.getValue(element);
        if (oldValue != newValue) {
            uic.applyMark(element, oldValue, newValue);
        }
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
    let oldValue = uic.getValue(element);
    if (oldValue != newValue)
        uic.applyMark(element, oldValue, newValue);
}

//Returns true or false if the element contains this event
uic.elementContainsEvent = function($element, eventKey){
    return ($._data($element.get(0), 'events') != undefined && $._data($element.get(0), 'events')[eventKey] != undefined)        
}


uic.markChangesIcon = $('<i>', { class: 'fas fa-triangle-exclamation uic-value-changed' });
uic.markChangesTooltip = function (element, oldValue, newValue) {
    return `Value has changed to '${newValue}'\r\nClick here to update value`;
}

uic.applyMark = async function (element, oldValue, newValue) {
    let mark = uic.markChangesIcon.clone();
    let tooltip = await uic.markChangesTooltip(element, oldValue, newValue);

    if (tooltip.length)
        mark.attr('title', tooltip);
    mark.click(() => {
        uic.setValue(element, newValue);
        element.removeClass('uic-value-changed');
        mark.remove();
    });

    element.each((index, item) => {
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
