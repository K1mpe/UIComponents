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
    element = $(element);
    //If you create a function on a element like this
    //  $().on('uic-setValue', function (e, value) {
    //      ...;
    //  });
    // This will overwrite the default behaviour of this function.
    if (!$(element).length)
        return;

    if ($._data($(element).get(0), 'events') != undefined && $._data($(element).get(0), 'events')["uic-setValue"] != undefined) {
        $(element).trigger('uic-setValue', value);
        return;
    }
    if (element.attr('name') == undefined) {
        var properties = uic.getProperties(element);
        if (properties.length == 1) {
            uic.setValue(properties, value);
            return;
        }
    }

    if (typeof value == "object" && value != null) {
        var properties = uic.getProperties(element);
        if (properties.length == 1) {
            uic.setValue(properties, value);
            return;
        }
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
    element = $(element);
    if (!element.length)
        return;


    if ($._data($(element).get(0), 'events') != undefined && $._data($(element).get(0), 'events')["uic-setValue"] != undefined) {
        let oldValue = uic.getValue(element);
        if (oldValue != newValue && !(!oldValue && !newValue)) {
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
    if (oldValue != newValue && !(!oldValue && !newValue))
        uic.applyMark(element, oldValue, newValue);
}

//Returns true or false if the element contains this event
uic.elementContainsEvent = function($element, eventKey){
    return ($._data($element.get(0), 'events') != undefined && $._data($element.get(0), 'events')[eventKey] != undefined)        
}


uic.markChangesIcon = $('<i>', { class: 'fas fa-triangle-exclamation uic-value-changed' });
uic.markChangesTooltip = async function (element, oldValue, newValue) {
    let translatable = {
        ResourceKey: "UIC.MarkChanges",
        DefaultValue: "Value has changed to {0}\r\nClick here to update value",
        Arguments: [newValue]
    };
    return await uic.translation.translate(translatable);
}

uic.applyMark = async function (element, oldValue, newValue) {
    let mark = uic.markChangesIcon.clone();
    let tooltip = await uic.markChangesTooltip(element, oldValue, newValue);

    let elementId = element.attr('id');
    let visualElement = element;
    if (element.prop('tagName') == "SELECT") {
        let option = element.find(`option[value=${newValue}]`);
        if (option.length)
            tooltip = await uic.markChangesTooltip(element, oldValue, option.text().replaceAll('\n', '').trim());
        let select2Span = element.next();
        if (select2Span.length && select2Span.hasClass('select2'))
            visualElement = select2Span;
    }
    if (tooltip.length)
        mark.attr('title', tooltip);
    if (elementId.length) {
        $(`.uic-value-changed[data-for=${elementId}]`).remove();
        mark.attr('data-for', elementId);
    }
    mark.click(() => {
        uic.setValue(element, newValue);
        visualElement.removeClass('uic-value-changed');
        mark.remove();
    });

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

    if (!$.isPlainObject(objectMissMatch) && objectMissMatch == comparison)
            return false;

    if (!$.isPlainObject(objectMatch))
        return comparison == objectMatch;

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
﻿uic.card = uic.card || {
    openCard: async function(card) {
        await card.triggerHandler('uic-before-open');
        card.CardWidget('expand');
    },
    closeCard: async function(card) {
        await card.triggerHandler('uic-before-close');
        card.CardWidget('collapse');
    },
    toggleCard: async function(card) {
        if (card.hasClass('collapsed-card'))
            await uic.card.openCard(card);
        else
            await uic.card.closeCard(card);
    }
};
$(document).ready(function () {
    $('.card').on('expanded.lte.cardwidget', (ev) => { ev.stopPropagation(); $(ev.target).triggerHandler('uic-opened'); });
    $('.card').on('collapsed.lte.cardwidget', (ev) => { ev.stopPropagation(); $(ev.target).triggerHandler('uic-closed'); });
});﻿uic.changeWatcher = uic.changeWatcher || {
    example: function () {
        console.log('the changeWatcher uses the uic.getValue() method to check for changes. This means the selector can by any type of jquery selector, (input, select, div, form, ...)')
        console.log('You need to create the watcher before any changes occur');
        console.log('let watcher = uic.changeWatcher.create($($0)); => Watcher that only watches one element');
        console.log('if(watcher.isChanged()){');
        console.log('...');
        console.log('}');
        console.log('');
        console.log('When SignalR updatesthe page, you can redefine the initial values of the watcher:');
        console.log('uic.changeWatcher.setInitialValues($($0))');
    },

    //Create a new changewatcher
    create: function (...$selectors) {

        let watcher = {
            elements: {},
            //Method that gets all the changes and return them as a array
            getChanges: function () {
                let changes = [];
                let elNames = Object.getOwnPropertyNames(this.elements);
                elNames.forEach((elName) => {
                    let currentVal = uic.getValue(elName);
                    if (!uic.compareObjects(currentVal, this.elements[elName]))
                        changes.push({
                            element: elName,
                            name: $(elName).attr('name'),
                            initialValue: this.elements[elName],
                            currentValue: currentVal
                        });
                })
                return changes;
            },
            //Method that checks if the properties are changed, stops after first change.
            isChanged: function () {
                let elNames = Object.getOwnPropertyNames(this.elements);
                for (let i = 0; i < elNames.length; i++)
                {
                    let elName = elNames[i];
                    let currentVal = uic.getValue(elName);
                    if (!uic.compareObjects(currentVal, this.elements[elName]))
                        return true;
                }
                return false;
            }
        }
        uic.changeWatcher.setInitialValues(watcher, $selectors);
        return watcher;
    },

    //Provide this functino with a watcher and one or more selectors (array).
    //The remembered values of these selectors is than stored as the new InitialValue. (Can be triggered by SignalRChange).
    setInitialValues(watcher, $selectorsArray) {

        if (!Array.isArray($selectorsArray))
            $selectorsArray = [$selectorsArray];

        $selectorsArray.forEach(($selector) => {
            $selector.each((index, domEl) => {
                let selector = `#${$(domEl).attr('id')}`;
                if (selector == '#')
                    selector = domEl;
                let value = uic.getValue(selector);

                watcher.elements[selector] = value;
            })
        });
    },

    
};
﻿uic.contextMenu = uic.contextMenu || {


    //Enable or disable the entire contextMenu functionality
    enabled: true,

    //this is the last html element where the contextMenu has opened
    target: null,


    //consoleLogs for debuging the script
    console: {
        applyItemOptions: false,
        combineSameIds: false,
        onClick: false,
        menuItemsPerElement: false,
        renderMenuItems: false
    },


    //A list of all the menuItems that are assigned for this page
    menuItems: [],
    categories: [],


    //show available objects with their default values or function parameters
    help: {
        //A example of a menu item
        menuItem: {
            //jQuery selector for where this menu item is used
            selector: "",

            //id is a optional string, id must be unique for each contextItem, if not only the lowest selector will be used, and extended by the properties from the other contextItems with the same Id.
            id: null,

            //the name of a category this item belongs to. 
            category: null,


            //The position of this menuItem in the list, lower number on top, higher number on the bottom
            position: 1000,

            //If all items are optional, the contextMenu will not be used.
            optional: false,

            //<li>
            //    <a class="dropdown-item" href="#">
            //        <i class="fas fa-hash"></i>
            //        Text
            //    </a>
            //<li>

            //<li>
            //    <a class="dropdown-item" href="#">
            //        <span class="icon"></span>     => Use a span if no icon is used for allignment
            //        Text
            //    </a>
            //<li>
            //Html or jquery dropdown item
            //Can also be from a (async) function(target, clickedElement){}
            element: '<li><a class="dropdown-item"> <i class="fas fa-hash"></i> text</a></li>',

            //If Href is not '#', onClick will be triggered
            //target => the element that matches the selector
            //clickedElement => the element that has opened the contextMenu
            //event => the original event
            onClick: function (target, clickedElement, event) {
                console.log('menuItemClicked', item, event);
            },


            //icon: '<i class"fas fa-user"></i>',
            //icon: jQuery object,
            //icon: 'fas fa-user',
            //icon: async function (target) { return 'fas fa-user' || return '<i class"fas fa-user"></i>' }
            icon: function (target, clickedElement) {

            },

            //If this function exists, this will replace the text from the element
            //text: <a class="dropdown-item">my text</a>
            //text: jQuery object,
            //text: 'my text'
            //text: (async) function(target, clickedElement){return 'my text' || return <a class="dropdown-item">my text</a> };
            text: function (target, clickedElement) {

            },

            //If this exists, this will replace the title (tooltip) from this element
            //title : 'this is my tooltip',
            //title : (async) function(target, clickedElement){return 'this is my tooltip'}
            title: function (target, clickedElement) {

            },

            //this function returns a object where all
            //attr: { disabled: true, readonly: true};
            //attr: null;
            //attr: (async) function(target, clickedElement){return {disabled:true}};
            attr: function (target, clickedElement) {

            },

            //_target is not userAssignable and will be overwriten by the script! 
            //This property will be used to select the target in the onClick function
            _target: null,

            //_target is not userAssignable and will be overwriten by the script!
            //This is the html element that has been rendered from element from all the other properties combined.
            _renderedElement: '<li><a class="dropdown-item"> <i class="fas fa-hash"></i> text</a></li>',
        },
        category: {

            //required id for this category, this should match the "category" string from menuItem.
            categoryId: "",


            //This is a menuItem, which takes all the properties of the above mentioned menuItem
            menuItem: {},

            //a list of all the menuItems that are used in this category.
            //the element property has been adapted to include all changes from other properties
            _menuItems: [],




            //a function that returns the html on how to render the menuItems
            categoryRenderer: function (category, totalMenuItems) {

            }
        }
    },


    //Add a menuItem or category to the contextMenu
    add: function (menuItem) {
        //category requires categoryId
        if (menuItem.categoryId !== undefined) {
            uic.contextMenu.addCategory(menuItem);
        } else {
            uic.contextMenu.addMenuItem(menuItem);
        }
    },

    addMenuItem: function (menuItem) {
        uic.contextMenu.menuItems.push(menuItem);
    },
    addCategory: function (category) {
        uic.contextMenu.categories.push(category);
    },

    default: {
        divider: $('<li>').append($('<div>', { class: 'dropdown-divider' })),

        category: {
            menuItem: {
                position: 1000
            },
            categoryRenderer: (category) => uic.contextMenu.default.functions.category.group(category, false),
        },

        functions: {
            category: {
                //render a category as indevidual items
                group: (category, addDividers) => {
                    var result = $('<div>').css('order', category.menuItem?.position || 1000);

                    if (addDividers)
                        result.append(uic.contextMenu.default.divider.clone());

                    var menuItems = uic.contextMenu._sort(category._menuItems);

                    menuItems.forEach(function (value, index) {
                        result.append(value._renderedElement);
                    });

                    if (addDividers)
                        result.append(uic.contextMenu.default.divider.clone());

                    return result;
                },

                iconsOnly: (category, addDividers) => {

                    var results = $('<div>').css('order', category.menuItem?.position || 1000);
                    var itemRow = $('<li>');

                    if (addDividers)
                        results.append(uic.contextMenu.default.divider.clone());

                    var menuItems = uic.contextMenu._sort(category._menuItems);

                    menuItems.forEach(function (value) {
                        var element = $(value._renderedElement).find('.dropdown-item');

                        //If no title is available, set the item text as title
                        element.attr('title', $(value._renderedElement).attr('title') || element.text());

                        element.addClass('d-inline').textNodes().remove();

                        itemRow.append(element);
                    });
                    results.append(itemRow);

                    if (addDividers)
                        results.append(uic.contextMenu.default.divider.clone());

                    return results;
                },

                subMenu: async (category, addDividers) => {
                    let results = $('<div>').css('order', category.menuItem?.position || 1000);

                    if (!category.menuItem)
                        console.error('Category requires menuItem', category);

                    let sublistPromise = uic.contextMenu._getItemPromise(category.menuItem);
                    await Promise.resolve(sublistPromise).then((result) => {
                        let subItem = uic.contextMenu._processSingleItemPromises(result);

                        let subMenu = $(subItem._renderedElement).addClass('dropdown-submenu').addClass('dropdown-hover');
                        subMenu.find('a').addClass('dropdown-toggle').attr('role', 'button').click(function (e) { e.stopPropagation(); e.preventDefault(); });

                        if (!subMenu.find('[disabled]').length) {
                            let subList = $('<ul>', { class: 'dropdown-menu' });
                            let menuItems = uic.contextMenu._sort(category._menuItems);

                            menuItems.forEach(function (value) {
                                subList.append(value._renderedElement);
                            });

                            subList = uic.contextMenu._cleanUp(subList);
                            subMenu.append(subList);
                        }

                        results.append(subMenu);
                    })

                    if (addDividers)
                        results.append(uic.contextMenu.default.divider.clone());


                    return results;
                }
            },
            isHTML: function (text) {
                let document = new DOMParser().parseFromString(text, 'text/html');
                return Array.from(document.body.childNodes).some(node => node.nodeType === 1);
            },
        },

        //The html of the context-menu dropdown
        menu: function () {
            var menu = $('<div>', { id: 'contextMenu', class: 'uic context-menu dropdown show' }).append($('<ul>', { class: 'dropdown-menu show dropdown-icons-left' }));
            return menu;
        }
    },



    // #region Actual script code

    //Find a menuItem in the collection
    //element is jQuery, Id or htmlTag
    find: function (element) {
        if (element instanceof jQuery)
            return $(uic.contextMenu.MenuItems).find(element);

        else if (jQuery.type(element) === "string") {
            console.log('element is string');
        }
        else {
            console.log('element is object');
        }
    },

    //Find all contextMenuItems for the selected element.
    //element => useable as jQuerySelector => $(element)
    //bubbleUp => 
    //      true if you want to use this function recursively for all its parents.
    //      false if you only want the results from this element.
    findForElement: function (element, bubbleUp = true) {
        element = $(element);

        if (!element.length)
            return null;

        var results = $.map(uic.contextMenu.menuItems, function (menuItem) {
            var matches = $(menuItem.selector);
            for (var i = 0; i < matches.length; i++) {
                if (matches[i] === element[0]) {
                    menuItem._target = element;
                    return Object.assign({}, menuItem); // Object.assign => make clone
                }

            }
            return null;
        });

        if (uic.contextMenu.console.menuItemsPerElement) {
            console.log('contextMenu.menuItemsPerElement - element, menuItems', element, results);
        }

        if (bubbleUp) {
            var parentResults = uic.contextMenu.findForElement(element.parent(), true);
            if (parentResults != null) {
                for (var i = 0; i < parentResults.length; i++) {
                    var parentResult = parentResults[i];
                    if (parentResult.id !== null && parentResult.id !== undefined) {
                        //console.log(results.filter(x => x.id == parentResult.id));
                        var existingResult = results.filter(x => x.id == parentResult.id);
                        var index = results.indexOf(existingResult[0]);

                        if (existingResult.length) {

                            var combined = $.extend(true, {}, parentResult, existingResult[0]);

                            if (uic.contextMenu.console.combineSameIds) {
                                console.log('contextMenu.combineSameIds - existingResult, parentResult, combinedResult', existingResult[0], parentResult, combined);
                            }

                            results[index] = combined;
                            continue;
                        }
                    }
                    results.push(parentResult);
                }
            }

        }


        return results;
    },
    hideMenu: function () {
        $('.context-menu').remove();
    },

    //A function that can be configured to use or disable the contextMenu. F.E. if "ALT" is pressed while right clicking
    altKeyEnabled: function (ev) {
        return false;
    },

    rightClick: async function (ev) {
        if (!uic.contextMenu.enabled && !uic.contextMenu.altKeyEnabled(ev))
            return;
        if (uic.contextMenu.enabled && uic.contextMenu.altKeyEnabled(ev))
            return;

        if (!uic.contextMenu.enabled && uic.contextMenu.altKeyEnabled(ev))
            uic.contextMenu.enabled = true;

        uic.contextMenu.target = ev.target;
        //console.log('rightClickEvent', e);

        if ($('.context-menu').length) {
            uic.contextMenu.hideMenu();
            ev.preventDefault();
        }
        else {
            var menu = uic.contextMenu.default.menu();
            $(menu).css('left', `${ev.pageX}px`);
            $(menu).css('top', `${ev.pageY}px`);


            let menuItems = uic.contextMenu.findForElement(uic.contextMenu.target, true);
            if (menuItems != null && menuItems.length && menuItems.filter(x => !x.optional).length) {
                //Sort menuItems on their position
                menuItems = uic.contextMenu._sort(menuItems);
                

                ev.preventDefault();

                let promises = [];

                for (var i = 0; i < menuItems.length; i++) {
                    let menuItem = menuItems[i];
                
                    let promise = uic.contextMenu._getItemPromise(menuItem);

                    promises.push(promise);
                }

                let resultPromises = [];
                await Promise.all(promises).then(function (menuItemPromises) {
                    var menuItems = [];
                    menuItemPromises.forEach(function (item) {
                        menuItems.push(uic.contextMenu._processSingleItemPromises(item));
                    });

                    var results = [];
                    menuItems.forEach(function (value, index) {
                        if (uic.contextMenu.console.renderMenuItems) {
                            console.log(`contextMenu.renderMenuItem ${index} - menuItem, html`, value, value._renderedElement[0]);
                        }

                        if (!value.category)
                            results.push(value);
                        else {
                            let category = results.filter(x => x.categoryId == value.category);
                            if (category.length == 0) {
                                category = uic.contextMenu._getCategory(value);
                                category._menuItems = [];
                                category._menuItems.push(value);
                                results.push(category);
                            } else {
                                category = category[0];
                                category._menuItems.push(value);
                                let index = results.indexOf(category);
                                results[index] = category;
                            }
                        }
                    });

                    results = uic.contextMenu._sort(results);

                

                    results.forEach(async function (value) {
                        if (!value.categoryRenderer) {
                            resultPromises.push(value._renderedElement);
                        } else {
                            resultPromises.push(value.categoryRenderer(value, menuItemPromises.length));
                        }
                    })
                
                });
                await Promise.all(resultPromises).then((results) => {
                    results.forEach(function (item) {
                        $(menu).find('> .dropdown-menu').append(item);
                    });
                });
                menu = uic.contextMenu._cleanUp(menu);
                $('body').append($(menu));
            }
        
        }
    },
    _getCategory: function(menuItem){
        if (!menuItem.category)
            return null;

        var category = uic.contextMenu.categories.filter(x=>x.categoryId == menuItem.category);
        if (!category) {
            category = {};
        } else {
            category = category[0];
        }

        var mergedCategory = $.extend(true, { categoryId: menuItem.category }, uic.contextMenu.default.category, category);
        return mergedCategory;
    },

    //Sort the menuItems by the position
    _sort: function (menuItems) {
        menuItems = menuItems.sort(function (a, b) {
            //a is bigger than b
            if ((a.menuItem?.position || a.position || 1000) > (b.menuItem?.position || b.position || 1000))
                return 1;

            //a is smaller then b
            if ((a.menuItem?.position || a.position || 1000) < (b.menuItem?.position || b.position || 1000))
                return -1;

            //a and b are equal
            return 0;
        });
        return menuItems;
    },

    //Clean the contextMenu by removing duplicate dividers
    _cleanUp: function (menu) {
        let ul = $(menu).find('.dropdown-menu');

        //multipleDividers
        let lis = ul.find('> li, > div > li');
        let prevWasDivider = true;
        lis.each(function (index, item) {
            if ($(item).find('> .dropdown-divider, > div > .dropdown-divider').length) {

                //if previous element was divider or is last element, remove divider
                if (prevWasDivider || lis.length-1 == index) {
                    lis[index].remove();
                } else {
                    prevWasDivider = true;
                }
            } else {
                prevWasDivider = false;
            }
        })
    
        return $(menu);
    },

    _getItemPromise: function (menuItem) {
    
        let promise = Promise.all([
            menuItem,
            setElementAsync(menuItem),
            setTextAsync(menuItem),
            setTitleAsync(menuItem),
            setAttrAsync(menuItem),
            setIconAsync(menuItem)
        ]);

        return promise
        async function setElementAsync(menuItem) {

            var element = menuItem.element;

            if (!element) {
                return;
            }

            //element= function (target) {  }
            if ($.isFunction(element)) {
                element = await element($(menuItem._target), $(uic.contextMenu.target));
            }

        

            if (!uic.contextMenu.default.functions.isHTML(element) && !element instanceof jQuery) {
                console.error()
            }
            // element =
            //<li>
            //    <a class="dropdown-item" href="#">
            //        <i class="fas fa-hash"></i>
            //        Text
            //    </a>
            //<li>
            return element;
        }
        async function setTextAsync(menuItem) {

            var text = menuItem.text;

            if (!text) {
                return;
            }

            //text= function (target) { return 'mijn text'; }
            if ($.isFunction(text)) {
                text = await text($(menuItem._target), $(uic.contextMenu.target));
            }

            // text != 'mijn text'
            if (!uic.contextMenu.default.functions.isHTML(text) && 'string' == typeof text) {
                    text = `<a class="dropdown-item" href="#">${text}</a>`;

            }


            if (!uic.contextMenu.default.functions.isHTML(text) && !text instanceof jQuery) {
                console.error()
            }
            // text = <a class="dropdown-item">mijn text</a>
            return text;
        }
        async function setIconAsync(menuItem) {

            if (!menuItem.icon) {
                return;
            }

            var icon = menuItem.icon;

            //icon= function (target) { return 'fas fa-user'; }
            if ($.isFunction(icon)) {
                icon = await icon($(menuItem._target), $(uic.contextMenu.target));
            }

            // icon == 'fas fa-icon'
            if (!uic.contextMenu.default.functions.isHTML(icon) && 'string' == typeof icon) {
                icon = `<i class="${icon}"></i>`;

            }


            if (!uic.contextMenu.default.functions.isHTML(icon) && !obj instanceof jQuery) {
                console.error()
            }
            // icon = <a class="dropdown-item">mijn icon</a>
            return icon;
        }
        async function setTitleAsync(menuItem) {
            if (!menuItem.title) {
                return;
            }

            var title = menuItem.title;

            //text= function (target) { return 'mijn text'; }
            if ($.isFunction(title)) {

                title = await title($(menuItem._target), $(uic.contextMenu.target));
            }

            return title;

        }
        async function setAttrAsync(menuItem) {
            if (!menuItem.attr) {
                return;
            }

            var attr = menuItem.attr;

            //attr= function (target) { return {} }
            if ($.isFunction(attr)) {

                attr = await attr($(menuItem._target), $(uic.contextMenu.target));
            }

            return attr;
        }
    },
    _processSingleItemPromises: function(values){
        let menuItem = values[0];
        let element = values[1]
        let text = values[2];
        let title = values[3];
        let attr = values[4];
        let icon = values[5];

        if (uic.contextMenu.console.applyItemOptions) {
            console.log('contextMenu.applyItemOptions - menuItem, element, text, title, attr, icon', menuItem, element, text, title, attr, icon);
        }

        let menuElement = $(element);

        if (!menuElement.length)
            menuElement = $('<li>').append($('<a>', { class: "dropdown-item", href: "#" }));

        menuElement.css('order', menuItem.position || 1000);


    

        if (text) {
            menuElement.find('a.dropdown-item').remove();
            menuElement.append(text);
        }
    

        if (icon != undefined) {
            menuElement.find('> a.dropdown-item').find('>i, span.icon').remove();
            if (icon == null) {
                menuElement.find('> a.dropdown-item').prepend('<span class="icon"></span>');
            } else {
                menuElement.find('> a.dropdown-item').prepend(icon);
            }
        }

        let item = menuElement.find('.dropdown-item') || menuElement;

        if (title) {
            item.attr('title', title);
        }

        if (attr) {
            item.attr(attr);
        }

        if (item.attr('disabled') !== undefined)
            item.addClass('disabled');

        //add no icon span if icon is not found
        if (!menuElement.find('i, span.icon').length) {
            (menuElement.find('> a.dropdown-item') || menuElement).prepend('<span class="icon"></span>');
        }

        if (menuItem.onClick != null) {
            item.click(async function (e) {
                if (uic.contextMenu.console.onClick) {
                    console.log('contextMenu.onClick - menuElement, menuItem, event', menuElement, menuItem, event);
                }

                e.preventDefault();
                if (item.attr('disabled') || menuElement.find('.disabled').length) {
                    e.stopPropagation();
                    return;
                }

                //let menuItem = $(this).data('menuItem');
                await menuItem.onClick($(menuItem._target), $(uic.contextMenu.target), e)
            });
        }

        menuItem._renderedElement = menuElement;
        return menuItem;
    }


//#endregion


};
$(document).ready(function () {
    $(document).on('click', function () {
        uic.contextMenu.hideMenu();
    });

    document.onclick = uic.contextMenu.hideMenu;
    document.oncontextmenu = uic.contextMenu.rightClick;

});
﻿uic.delayedAction = uic.delayedAction || {


//delayedAction.Run("MyUniqueKey", 500, (data) => {data.forEach((item) => console.log(item))})
//This method will be run after the delay has run out. If the same key is used before the previous action has completed, this previous action is canceled.
//key => This value needs to be unique, as this is the identifier to reset the timer
//delayMiliseconds => The delay in miliseconds before the callback is triggered
//callback => a function that can take the list of all data that has been collected for this function
// data[0] is always the newest data
// hint: data.length will show how many times this function has been reset before executing
//(optional) data => the data that is send with the function
    run: function (key, delayMiliseconds, callback, ...data) {
        let existing = uic.delayedAction._running[key];

        if (existing == undefined || existing == null) {
            existing = {
                timer: null,
                data: []
            };
        } else {
            clearTimeout(existing.timer);
        }
        if (data.length == 0)
            existing.data.push(null);
        else if (data.length == 1)
            existing.data.push(data[0]);
        else
            existing.data.push(data);

        existing.timer = setTimeout(() => {
            callback(existing.data.reverse());
            uic.delayedAction._running[key] = null;
        }, delayMiliseconds);

        uic.delayedAction._running[key] = existing;
    },
    _running : {},
};
﻿uic.fileExplorer = uic.fileExplorer || {
    renderMethods: {
        details: async (container) => {
            let main = container.find('.file-explorer-main');
            let headerRow = $('<tr>')
                .append($('<th>', { name: 'Icon' }).text(await uic.translation.translateKey("FileExplorer.Icon")))
                .append($('<th>', { name: 'FileName' }).text(await uic.translation.translateKey("FileExplorer.FileName")))
                .append($('<th>', { name: 'FileType' }).text(await uic.translation.translateKey("FileExplorer.FileType")))
                .append($('<th>', { name: 'LastModified' }).text(await uic.translation.translateKey("FileExplorer.LastModified")))
                .append($('<th>', { name: 'Size' }).text(await uic.translation.translateKey("FileExplorer.Size")));
            let tbody = $('<tbody>');
            let table = $('<table>', {class:'table'})
                .append($('<thead>').append(headerRow))
                .append(tbody);

            let getFilesForDirectoryResultModel = container.triggerHandler('uic-getLastDirectoryResult');

            getFilesForDirectoryResultModel.Files.forEach((item) => {

                let row =$('<tr>', { class:'explorer-item', 'data-absolutePath': item.AbsolutePathReference, 'data-relativePath': item.RelativePath })
                    .append($('<td>').append(item.Icon))
                    .append($('<td>').append(item.FileName))
                    .append($('<td>').append(item.FileType??item.Extension))
                    .append($('<td>').append(moment(item.LastModified).format('LLL')))
                    .append($('<td>').append(item.Size));

                if (item.Extension == 'folder') {
                    row.addClass('explorer-folder');
                }
                tbody.append(row);

            });
            main.append(table);
        },
        large: async (container) => {
            let main = container.find('.file-explorer-main');

            let row = $('<div>', { class: 'row' });

            let getFilesForDirectoryResultModel = container.triggerHandler('uic-getLastDirectoryResult');
            getFilesForDirectoryResultModel.Files.forEach((item) => {
                let col = $('<div>', { class: 'col col-md-4 col-xl-3 explorer-item', 'data-absolutePath': item.AbsolutePathReference, 'data-relativePath': item.RelativePath })
                    .append($('<div>', {class: 'explorer-thumbnail'}).append(item.Thumbnail ?? item.Icon))
                    .append(item.FileName);
                if (item.Extension == 'folder') {
                    col.addClass('explorer-folder');
                }
                row.append(col);
            });
            main.append(row);
        },
    },
    initialize:
    {
        start: function (container) {
            uic.fileExplorer.initialize.jsTree(container);
            uic.fileExplorer.initialize.previewWindow(container);
        },
        jsTree: function (container) {
            let containerId = container.attr('id');
            let tree = $(`[for-explorer="${containerId}"] .explorer-tree`);
            tree.jstree({
                'core': {
                    check_callback: true,
                    expand_selected_onload: false,
                    'data': async function (object, callback) {

                        let relativeDir;
                        let absoluteRef = container.attr('data-rootAbsolutePath');
                        let controller = container.data('controller');

                        let treeItems = [];
                        if (object.id === '#') {
                            relativeDir = container.attr('data-rootDirectory'); 
                            treeItems.push({
                                text: relativeDir.slice(0, -1),// remove the '/' at the end
                                state: {
                                    opened: false,
                                    disabled: false,
                                    selected: false,
                                },
                                children: true,
                                li_attr: {
                                    'data-absolutePath': absoluteRef,
                                    'data-relativePath': relativeDir
                                }
                            });
                            return callback.call(this, treeItems);
                        }

                        let filterModel = {
                            AbsolutePathReference: object.li_attr['data-absolutePath'],
                            RelativePath: object.li_attr['data-relativePath'],
                            FoldersOnly: true
                        };
                        let result = await uic.getpost.get(`/${controller}/GetFilesForDirectory`, filterModel);
                        
                        try {
                            result.Files.forEach((item, index) => {
                                treeItems.push({
                                    text: item.FileName,
                                    children: item.DirectoryHasSubdirectories,
                                    li_attr:{
                                        'data-absolutePath': item.AbsolutePathReference,
                                        'data-relativePath': item.RelativePath
                                    }
                                });
                            });
                        } catch { }

                        return callback.call(this, treeItems);

                    },
                },
                dnd: {

                },
                plugins: ['dnd']
            });
            tree.on('move_node.jstree', (ev, data) => {
                console.log('moving js-tree-item');
            });
            tree.click(function (ev) {
                ev.stopPropagation();
                ev.preventDefault();
                var target = $(ev.target);

                if (target.hasClass('jstree-ocl')) //don't do anything when clicking the collapse icon
                    return;

                target = $(ev.target).closest('li');
                let directory = target.attr('data-relativePath');
                if(directory != undefined && directory.length > 0)
                    uic.fileExplorer.loadRelativeDir(container, directory);
            });

            container.on('uic-after-fetch', (ev, ...data) => {
                let filterModel = container.triggerHandler('uic-getFilterModel');
                let relativePathParts = filterModel.RelativePath.split("/");
                let path = '';
                let lastNode = null;
                for (let i = 0; i < relativePathParts.length; i++) {
                    path += (relativePathParts[i] + "/");

                    let node = tree.find(`li[data-relativepath="${path}"]`);
                    tree.jstree('open_node', node);
                    if (node.length)
                        lastNode = node;
                }
                tree.jstree('deselect_all');
                tree.jstree('select_node', lastNode);
            });
        },
        previewWindow: function (container) {
            let containerId = container.attr('id');
            let window = $(`[for-explorer="${containerId}"] .explorer-preview`);
            container.on('click', '.explorer-item', async (ev) => {
                window.html('');
                let item = $(container).find('.explorer-item.selected');
                if (item.length > 1)
                    return;
                let controller = container.attr('data-controller');
                let absolutePath = item.attr('data-absolutepath');
                let relativePath = item.attr('data-relativepath');
                uic.partial.showLoadingOverlay(window);
                await container.triggerHandler('uic-before-fetch-preview');
                let preview = await uic.getpost.post(`/${controller}/Preview`, {
                    pathModel: {
                        AbsolutePathReference: absolutePath,
                        RelativePath: relativePath
                    }
                });
                uic.partial.hideLoadingOverlay(window);
                if (preview != false)
                    window.html(preview);
            });
        }
    },
    loadRelativeDir: async function (container, directory) {
        let controller = container.data('controller');
        let filterModel = container.triggerHandler('uic-getFilterModel');
        filterModel.RelativePath = directory;
        await this.fetchFiles(container, controller, filterModel);
        await uic.fileExplorer.renderFiles(container);
    },
    fetchFiles: async function (container, controller, getFilesForDirectoryFilterModel) {
        let mainWindow = container.find('.file-explorer-main');
        uic.partial.showLoadingOverlay(mainWindow);
        await container.triggerHandler('uic-before-fetch', getFilesForDirectoryFilterModel);
        container.trigger('uic-setFilterModel', getFilesForDirectoryFilterModel);

        var result = await uic.getpost.get(`/${controller}/GetFilesForDirectory`, getFilesForDirectoryFilterModel);

        if (result == null || result == false)
            throw ("Exception occured");

        uic.partial.hideLoadingOverlay(mainWindow);
        container.trigger('uic-setLastDirectoryResult', result);
        await container.triggerHandler('uic-after-fetch', result, getFilesForDirectoryFilterModel);
    },
    renderFiles: async function (container) {

        let main = container.find('.file-explorer-main');
        main.html('');

        let renderMethodString = main.attr('data-renderer');
        let renderMethod = uic.fileExplorer.renderMethods[renderMethodString];
        await renderMethod(container);


        uic.fileExplorer.setMainEvents(container);
        container.trigger('uic-files-rendered');
    },


    setMainEvents: function (container) {
        container.find('.explorer-item').on('click', (ev) => {
            container.find('.explorer-item').removeClass('selected');
            $(ev.target).closest('.explorer-item').addClass('selected');
        });
        container.find('.explorer-folder').on('uic-openExplorerItem', (ev) => {
            let target = $(ev.target);
            let relativePath = target.attr('data-relativePath');
            uic.fileExplorer.loadRelativeDir(container, relativePath);
        })
        container.find('.explorer-item').on('dblclick', (ev) => {
            let explorerItem = $(ev.target).closest('.explorer-item');

            if (uic.elementContainsEvent(explorerItem, 'uic-openExplorerItem')) {
                explorerItem.trigger('uic-openExplorerItem');
                return;
            }

            uic.fileExplorer.openFile(explorerItem);
        })
    },
    openFile: async function (explorerItem) {
        console.log('openFile', explorerItem);
        let container = explorerItem.closest('.file-explorer-container');
        await container.triggerHandler('uic-before-open', explorerItem);
        let controller = container.attr('data-controller');
        let absolutePath = explorerItem.attr('data-absolutepath');
        let relativePath = explorerItem.attr('data-relativepath');
        await uic.fileExplorer.download(`/${controller}/DownloadFile`, {
            pathModel: {
                AbsoluePathReference: absolutePath,
                relativePath: relativePath
            }
        });
        var result = await uic.getpost.get(`/${controller}/GetFilesForDirectory`,);

        if (result == null || result == false) {

        }
            throw ("Exception occured");

        await container.triggerHandler('uic-after-open', explorerItem);
    },
    download: async function (source, data) {
        //https://stackoverflow.com/questions/16086162/handle-file-download-from-ajax-post
        await $.ajax({
            type: "POST",
            url: source,
            data: data,
            xhrFields: {
                responseType: 'blob' // to avoid binary data being mangled on charset conversion
            },
            success: function (blob, status, xhr) {
                // check for a filename
                var filename = "";
                var disposition = xhr.getResponseHeader('Content-Disposition');
                if (disposition && disposition.indexOf('attachment') !== -1) {
                    var filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
                    var matches = filenameRegex.exec(disposition);
                    if (matches != null && matches[1]) filename = matches[1].replace(/['"]/g, '');
                }

                if (typeof window.navigator.msSaveBlob !== 'undefined') {
                    // IE workaround for "HTML7007: One or more blob URLs were revoked by closing the blob for which they were created. These URLs will no longer resolve as the data backing the URL has been freed."
                    window.navigator.msSaveBlob(blob, filename);
                } else {
                    var URL = window.URL || window.webkitURL;
                    var downloadUrl = URL.createObjectURL(blob);

                    if (filename) {
                        // use HTML5 a[download] attribute to specify filename
                        var a = document.createElement("a");
                        // safari doesn't support this yet
                        if (typeof a.download === 'undefined') {
                            window.location.href = downloadUrl;
                        } else {
                            a.href = downloadUrl;
                            a.download = filename;
                            document.body.appendChild(a);
                            a.click();
                        }
                    } else {
                        window.location.href = downloadUrl;
                    }

                    setTimeout(function () { URL.revokeObjectURL(downloadUrl); }, 100); // cleanup
                    makeToast("Success", "File successfully downloaded");
                }
            }, error: function (XMLHttpRequest, textStatus, errorThrown) {
                ErrorBox(errorThrown);
            }
        });
    }
};﻿uic.form = uic.form || {
    //Check if this element or any parent is hidden or collapsed
    isHidden: function (element) {
        element = $(element);
        if (!element.length)
            return true;

        if (!element.is(':visible'))
            return true;

        if (element.closest('[hidden]').length)
            return true;
        if (element.closest('.d-none').length)
            return true;
        if (element.closest('.collapsed-card').length)
            return true;
        if (element.closest('.tab-pane:not(.active)').length)
            return true;
        return false;
    },

    //Transform input elements to make the inputs of this form not look like input fields
    readonly: function (form, showEmptyInReadonly = true, showSpanInReadonly = true, showDeleteButton = false) {
        form = $(form);
        form.find('[readonly]')
            .addClass("always-readonly");
        form.find('.form-control')
            //    .addClass("form-control-plaintext")
            //    .removeClass("form-control")
            .attr("readonly", true)
            .attr("disabled", true);
        form.find('input[type=checkbox]')
            .attr("readonly", true)
            .attr("disabled", true);
        //form.find('.select2-container')
        //    .removeClass("select2-container--bootstrap4")
        //    .find('.select2-selection__rendered').addClass('px-0');

        form.addClass('readonly-form');

        //form.find('.cdc-select-btn-add').addClass("d-none");
        //form.find('.btn-save').attr('hidden', true);
        //form.find('.btn-readonly').attr('hidden', true);
        //form.find('.btn-edit').attr('hidden', false);
        //form.find('label > span').attr('hidden', true);



        if (!showEmptyInReadonly)
            form.addClass('hide-empty');

        if (!showSpanInReadonly)
            form.find('span:not(.card-header span, .select2, .select2 span, .input-group-text)').attr('hidden', true);

        if (!showDeleteButton)
            form.find('.btn-delete').attr('hidden', true);
    },


    //This function is to undo the uic.FormReadonly function, usefull for a edit button
    editable: function (form) {
        form = $(form);
        form.find('.form-control:not(.always-readonly)')
            //.addClass("form-control")
            //.removeClass("form-control-plaintext")
            .attr("readonly", false)
            .attr("disabled", false);

        form.removeClass('readonly-form').removeClass('hide-empty');;
        //form.find('.select2-container:not(.always-readonly)')
        //    .addClass("select2-container--bootstrap4")
        //    .find('.select2-selection__rendered').removeClass('px-0');


        form.find('span').attr('hidden', false);

        form.find('input[type=checkbox]:not(.always-readonly)')
            .attr("readonly", false)
            .attr("disabled", false);


        //form.find('.cdc-select-btn-add').removeClass("d-none");
        //form.find('.btn-confirm').attr('hidden', false);
        //form.find('.btn-readonly').attr('hidden', false);
        //form.find('.btn-edit').attr('hidden', true);
        //form.find('.input-no-value').removeClass('d-none');
        //form.find('label > span').attr('hidden', false);
        //form.find('span').attr('hidden', false);

        form.find('.btn-save').attr('hidden', false);
        form.find('.btn-delete').attr('hidden', false);
    },

    //This function will remove all "." from the input value
    removeDecimals: function (element) {
        element = $(element);
        var value = element.val();
        element.val(value.replaceAll(".", ""));
    },

    //This function will set a treestateboolean in the correct state according to its data-value attribute (true, false, null)
    setThreeState: function (item) {
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
    },

    delete: async function (url, data) {
        try {
            var content = await uic.getpost.get(url, data);
            $('body').append(content);

        } catch (ex) {
            console.error(ex);
        }
    },

    setPopoverOnClickTooltipIcon: function () {
        $('.tooltip-icon').each((index, item) => {
            let title = $(item).parent().attr('title');
            if (title.length) {
                $(item).popover({
                    content: title,
                    trigger: 'focus click',
                });
                $(item).attr('title', null);
            }
        })
    },
    select2: {
        //https://select2.org/searching
        searchMethod: function (params, data) {
            // If there are no search terms, return all of the data
            if ($.trim(params.term) === '') {
                return data;
            }

            // Do not display the item if there is no 'text' property
            if (typeof data.text === 'undefined') {
                return null;
            }

            
            //Sort on each part seperated by ' '
            let parts = params.term.toLowerCase().split(" ");

            //If there are children, this is a group
            if (data.children != undefined) {
                let childResults = [];
                
                for (let i = 0; i < data.children.length; i++) {
                    let match = true;
                    let child = $.extend({}, data.children[i], true);

                    for (let j = 0; j < parts.length; j++) {
                        let part = parts[j];
                        if (child.text.toLowerCase().includes(part))
                            continue;

                        let searchAttr = $(child.element).attr('data-select-search');
                        if (!!searchAttr && searchAttr.toLowerCase().includes(part))
                            continue;

                        if (data.text.toLowerCase().includes(part)) {
                            continue;
                        }
                        let searchAttrGroup = $(data.element).attr('data-select-search');
                        if (!!searchAttrGroup && searchAttrGroup.toLowerCase().includes(part)) {
                            continue;
                        }

                        match = false;
                    }
                    if (match) {
                        if(data.text.length)
                            child.text += `( ${data.text} )`;

                        childResults.push(child);
                    }
                        
                }
                if (childResults.length)
                    return { children: childResults };
                else
                    return null;
;
            } else {
                let match = true;
                for (let i = 0; i < parts.length; i++) {
                    let part = parts[i];
                    if (data.text.toLowerCase().includes(part)) 
                        continue;

                    let searchAttr = $(data.element).attr('data-select-search');
                    if (!!searchAttr && searchAttr.toLowerCase().includes(part))
                        continue;

                    match = false;
                }

                if (match) {
                    return data;
                }
                else {
                    return null;
                }
            }
            

            //// `params.term` should be the term that is used for searching
            //// `data.text` is the text that is displayed for the data object
            //if (data.text.indexOf(params.term) > -1) {
            //    var modifiedData = $.extend({}, data, true);
            //    modifiedData.text += ' (matched)';

            //    // You can return modified objects from here
            //    // This includes matching the `children` how you want in nested data sets
            //    return modifiedData;
            //}

            //// Return `null` if the term should not be displayed
            //return null;
        },
        resultRenderer: function (state) {
            if (state.element == undefined)
                return;


            let selectId = $(state.element).closest('select').attr('id');
            let prepend;
            let append;
            if (state.children == undefined) {
                prepend = $(`.select-elements[for-select="${selectId}"] .prepend-item[for-value="${state.id}"]`).html();
                append = $(`.select-elements[for-select="${selectId}"] .append-item[for-value="${state.id}"]`).html();


            } else {
                prepend = $(`.select-elements[for-select="${selectId}"] .prepend-group[for-label="${state.text}"]`).html();
                append = $(`.select-elements[for-select="${selectId}"] .append-group[for-label="${state.text}"]`).html();
            }

            let result = $('<span>').append(prepend).append(state.text).append(append);

            let existingClass = $(state.element).attr('class');
            let existingStyle = $(state.element).attr('style');
            let existingData = $(state.element).data();

            result.attr('class', existingClass).attr('style', existingStyle).data(existingData);

            return result;
        }
    }
};
$(document).ready(() => uic.form.setPopoverOnClickTooltipIcon());
$(document).ajaxComplete(() => uic.form.setPopoverOnClickTooltipIcon());
﻿uic.getpost = uic.getpost || {
    defaultOptions : {
        get : {
            cancelPreviousRequests: true,
            loadType: null,
            handlers : []
        },

        post : {
            cancelPreviousRequests: false,
            loadType: null,
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
                    response.Errors.forEach((item) => {
                        var propertyName = item.PropertyName;
                        var errors = item.Error;

                        var spanElement = $(`span.field-validation-valid[data-valmsg-for="${propertyName}"]`);
                        if (!spanElement.length)
                            spanElement = $(`span.field-validation-valid[data-valmsg-for$=".${propertyName}"]`);
                        if (!spanElement.length) {
                            try {
                                var spanElement = $(`span[for=${propertyName}]`);
                                spanElement.removeClass();
                                spanElement.addClass("text-danger");
                            } catch {
                                //May crash silently if propertyName contains []
                            }
                            
                        }
                        spanElement.text(errors);
                        makeToast('error', null, errors, { timeOut: 60000, closeButton: true, progressBar: true, extendedTimeOut: 10000 });
                    })
                    return false;
                }
            },
            async(response) => {
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
                    let message = await uic.translation.translate(response.notification.Message);
                    if (message == "null")
                        message = "";
                    let title = await uic.translation.translate(response.notification.Title);
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
                    if (text.length > 255 || !text.length)
                        text = "A error occured";
                    //makeToast("Error", "", text);
                    return false;
                }
            }, 
            (response) => {
                if (response.type == "Redirect") {

                    if (!response.data.length)
                        location.reload();
                    else
                        location.href = response.url;
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

        uic.getpost._getRequests[url] = $.get(url, data, null, options.loadType).catch(function (...ex) {
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

        uic.getpost._postRequests[url] = $.post(url, data, null, options.loadType).catch(function (...ex) {
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
﻿uic.modal = uic.modal || {
    closeParent: function (item) {
        var modal = $(item).closest('.uic.modal');
        if (modal.length) {
            modal.trigger('uic-close');
            return true;
        }
        modal = $(item).closest('.modal');
        if (modal.length) {
            modal.modal('hide');
            return true;
        }

        var hideable = $(item).closest('.uic-can-close');
        if (hideable.length) {
            hideable.trigger('uic-close');
            return true;
        }

        return false;
    },
    moveModal: function (modal, referenceId) {
        $(uic.modal.modalDestination).append(modal);

        function removeModalIfReferenceIsGone() {
            if (!$(`#${referenceId}`).length)
                modal.remove();
        }
        if ($(`#${referenceId}`).length) {
            $(document).on('uic-reloaded', removeModalIfReferenceIsGone);
            $(document).on('uic-closed', removeModalIfReferenceIsGone);
        }
    },
    modalDestination: 'body',

};
﻿uic.partial = uic.partial || {
    showLoadingOverlay: function (element = null) {
        if (!element)
            element = document.body;

        $(element).LoadingOverlay('show', { image: '', fontawesome: 'fas fa-sync-alt fa-spin' });
    },

    hideLoadingOverlay: function (element = null) {
        if (!element)
            element = document.body;

        $(element).LoadingOverlay('hide');
    },

    handlers: [
        (response) => {
            if (response.type == "AccessDenied") {
                return $('<div>', { class: 'alert alert-danger', role: 'alert' }).append('Access Denied');
            }
        },
        (response) => {
            if (response.type == "Exception") {
                return $('<div>', { class: 'alert alert-danger', role: 'alert' }).append('Error receiving data');
            }
        },
    ],


    _reloadPartial: async function (partial, showOverlay, getDatafunc) {
        if (!partial.length)
            return;

        await partial.triggerHandler('uic-before-reload');
        if (showOverlay)
            uic.partial.showLoadingOverlay(partial);

        let result = await getDatafunc();

        //Remove the Select2 container when reloading the partial containing the select source
        let select2Container = $('.select2-container');
        if (select2Container.length) {
            let forId = select2Container.attr('data-for');
            if (partial.find(`#${forId}`).length)
                select2Container.remove();
        }
        
        partial.html(result);

        if (showOverlay)
            uic.partial.hideLoadingOverlay(partial);

        await partial.triggerHandler('uic-reloaded');
    }
};
$(document).ready(() => {
    $('body').on('uic-reload', (ev) => {
        location.reload();
    });
})

﻿uic.sidePanel = uic.sidePanel || {
    initialize: function (container, startState) {
        container = $(container);
        let isHorizontal = container.hasClass('horizontal');

        let uniqueName = container.data('sidebar-name');
        let setState = startState;
        if (uniqueName.length) {
            try {
                setState = Number(localStorage.getItem(`sidePanel.${uniqueName}.state`));
            } catch { }

        }

        switch (setState) {
            case 0:
                uic.sidePanel.setCollapse(container);
                break;
            case 1:
                uic.sidePanel.setOverlay(container);
                break;
            case 2:
                uic.sidePanel.setPinned(container);
                break;
        }


        if (isHorizontal) {
            uic.sidePanel.setHeightOnChange(container);
        }

        container.find('.side-panel').removeClass('d-none');

        container.find('.side-panel').on('open', function (e) {
            e.stopPropagation();
            uic.sidePanel.setOverlay(container);
        });
        container.find('.side-panel').on('close', function (e) {
            e.stopPropagation();
            uic.sidePanel.setCollapse(container);
        });

        container.find('.btn-sidebar-fixed').click(function (e) {
            e.preventDefault();
            uic.sidePanel.setPinned(container);
        });

        container.find('.btn-sidebar-open').click(function (e) {
            e.preventDefault();
            uic.sidePanel.setOverlay(container);
        });

        container.find('.btn-sidebar-close').click(function (e) {
            e.preventDefault();
            uic.sidePanel.setCollapse(container);
        });



        $(document).click(function (e) {

            let target = $(e.target);
            if (!container.find('.side-panel').hasClass('overlay'))
                return;

            if (target.hasClass('btn-sidebar-open') || target.closest('.btn-sidebar-open').length)
                return;

            if ($(e.target).closest('.side-panel').length)
                return;

            uic.sidePanel.setCollapse(container);
        });
    },
    setHeight: function (container) {
        let height = $(container).find('> .side-panel-content').height();

        $(container).find('.side-panel').height(height + 'px');
    },
    setHeightOnChange: function (container) {
        uic.sidePanel.setHeight(container);

        let content = $(container).find('> .side-panel-content');
        new ResizeObserver((element) => {
            let container = $(element[0].target).closest('.side-panel-container');
            uic.sidePanel.setHeight(container);
        }).observe(content[0]);
    },
    saveState: function (container, state) {
        let name = container.data('sidebar-name');
        if (!name.length)
            return;

        localStorage.setItem(`sidePanel.${name}.state`, state);
    },
    setCollapse: function (container) {
        let sidePanel = container.find('.side-panel');
        sidePanel.removeClass('col col-1 fit-content');

        sidePanel.removeClass('overlay fixed');
        sidePanel.addClass('collapsed');

        container.find('.btn-sidebar-open').removeClass('d-none');
        uic.sidePanel.saveState(container, 0);
    },
    setOverlay: function (container) {
        let sidePanel = container.find('.side-panel');
        sidePanel.removeClass('col col-1 fit-content');

        sidePanel.removeClass('collapsed fixed');
        sidePanel.addClass('overlay');

        sidePanel.find('.btn-sidebar-fixed').removeClass('d-none');
        container.find('.btn-sidebar-open').addClass('d-none');
        uic.sidePanel.saveState(container, 1);
    },
    setPinned: function (container) {

        let sidePanel = container.find('.side-panel');
        sidePanel.addClass('col col-1 fit-content');

        sidePanel.removeClass('collapsed overlay');
        sidePanel.addClass('fixed');

        sidePanel.find('.btn-sidebar-fixed').addClass('d-none');
        container.find('.btn-sidebar-open').addClass('d-none');
        uic.sidePanel.saveState(container, 2);
    }
};
﻿uic.signalR = {
    handleUIComponentFetch: async ()=>{
        await window.connection.on('SendUIComponentToUser', async (fetchComponent, userId) => {
            if (uic.signalR.currentUserId == undefined) {
                console.error("uic.signalR.currentUserId is not defined!")
                return;
            }

            if (uic.signalR.currentUserId != userId)
                return;

            let appendTo = $(fetchComponent.AppendTo);
            if (!appendTo.length)
                return;

            let result = await uic.getpost.get('/uic/getComponent', { key: fetchComponent.ComponentKey });
            appendTo.append(result);
        });
    },
    handleUIComponentRemove: async () => {
        await window.connection.on('RemoveUIComponentWithId', async (id) => {
            $(`#${id}`).trigger('uic-remove');
        });
    },
    currentUserId: undefined
}
$(document).ready(() => {
    //Wait a small delay so the connection can exist
    setTimeout(() => {
        uic.signalR.handleUIComponentFetch();
        uic.signalR.handleUIComponentRemove();
    }, 5);
});﻿uic.tabs = uic.tabs || {
    open: function (tab) {
        tab = $(tab);
        if (!tab.length)
            return;

        let tabHeader;
        let tabContent;
        if (tab.attr('role') == 'tab') {
            tabHeader = tab;
            tabContent = $(`#${tabHeader.data('target')}`);
            if (!tabContent.length)
                tabContent = $(tabHeader.attr('href'));
        }
        else if (tab.attr('role') == 'tabpanel') {
            tabContent = tab;
            tabHeader = $(`[role=tab][source='#${tabContent.attr('id')}'],[role=tab][href='#${tabContent.attr('id')}']`);
        }



        let oldActiveHeader = tabHeader.parent().children('.active[role=tab]');
        let oldActiveContent = tabContent.parent().children('.active[role=tabpanel]');

        if (tabHeader.length && tabContent.length) {
            tabHeader.parent().children('[role=tab]').removeClass('active');
            tabContent.parent().children('[role=tabpanel]').removeClass('active').removeClass('show');
            tabHeader.addClass('active');
            tabContent.addClass('active show');


            tabContent.triggerHandler('uic-opened');
            oldActiveContent.triggerHandler('uic-closed');

            let tabContainer = tab.closest('.card-tabs');
            tabContainer.triggerHandler('uic-tab-change', tabHeader, oldActiveHeader);
        }
    },
    setTabHash: function (tab) {
        var hash = [];
        $(tab).parents('.card-tabs').each(function (index, item) {
            var openTab = $(item).find('> .card-header .active[role=tab],>.row > .col-tab-headers .active[role=tab]');
            if (openTab.length)
                hash[index] = openTab.attr('href').replace("#", "");
        });
        let scrollTop = $('html').scrollTop();
        window.location.hash = hash.reverse().join(',');
        $('html, body').scrollTop(scrollTop);
    },
    openFirstTab: function (tabContainer) {
        let url = window.location;
        let hashes = url.hash.split(",");
        let conti= true;
        hashes.forEach(function (hash, index) {
            hash = hash.replace("#", "");
            let tab = tabContainer.find(`[role="tab"][href="#${hash}"]`);
            if (tab.length) {
                uic.tabs.open(tab);
                conti = false;
                return;
            }
                
            
        })
        if (!conti)
            return;
        try {
            let tabId = tabContainer.attr('id');
            let openTabId = localStorage.getItem(`tabs-lastState-${tabId}`);
            if ($(openTabId).length) {
                uic.tabs.open($(openTabId));
                conti = false;
                return;
            }
                
        } catch { }

        if (!conti)
            return;
        let firstTab = tabContainer.find('[role="tab"]')[0];
        uic.tabs.open($(firstTab));

    }
};
$(document).on('click', '.uic.card-tabs [role="tab"]', async function (ev) {

    let newTab = $(ev.target).closest('[role="tab"]');        // Newly activated tab
    ev.stopImmediatePropagation();
    uic.tabs.open(newTab);
    uic.tabs.setTabHash(newTab);


});
$(document).on('.uic.card-tabs', 'uic-help', () => {
    console.log("tabs .on('uic-tab-change', (ev, oldHeader, newHeader) => {...} Triggered when a tab changes");
});
$(document).on('.uic.card-tabs .tab-pane', 'uic-help',()=> {

    console.log("tab .on('uic-open', () => {...} Triggered when the tab opens");
    console.log("tab .on('uic-close', () => {...} Triggered when the tab closes");
});﻿uic.translation = uic.translation || {
    translate: async function (translatable) {
        //If the input has to resourceKey, inputting strings will just return the string
        if (translatable.ResourceKey == undefined)
            return translatable;

        //If the key is untranslatedKey, return the first argument without translations
        if (translatable.ResourceKey == "UntranslatedKey")
            return translatable.Arguments[0];

        //Check if the translation is already requested, call the fetchTranslationText on first request
        let cachedValue = uic.translation._defaultValues[translatable.ResourceKey];
        if (cachedValue == undefined) {
            cachedValue = await uic.translation.fetchTranslationText(translatable);
            uic.translation._defaultValues[translatable.ResourceKey] = cachedValue;
        }

        //Format the arguments in the text
        return cachedValue.format(translatable.Arguments);

        
    },
    translateKey: async function (key) {
        return await uic.translation.translate({ ResourceKey: key });
    },
    //The function that requests the service to give the translation
    fetchTranslationText: async function (translatable) {
        let defaultValue = translatable.DefaultValue || translatable.ResourceKey.split('.').slice(-1)[0];
        return defaultValue.format(translatable.Arguments);
    },

    //Local dictionary that store translation keys with value texts. these values do not have their variables replaced yet.
    //Content of this object remains until the page reloads.
    _defaultValues: {

    }
}