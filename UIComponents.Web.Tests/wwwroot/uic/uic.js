var uic = uic || {};

//Check if this element or any parent is hidden or collapsed
uic.isHidden = function (element) {
    element = $(element);
    if (!element.length)
        return true;

    if (element.closest('[hidden]').length)
        return true;
    if (element.closest('.d-none').length)
        return true;
    if (element.closest('.collapsed-card').length)
        return true;

    return false;
}

//Transform input elements to make the inputs of this form not look like input fields
uic.formReadonly = function (form, showEmptyInReadonly = true, showSpanInReadonly = true, showDeleteButton = false) {
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

    

    if(!showEmptyInReadonly)
        form.find('.input-no-value').addClass('d-none');

    if(!showSpanInReadonly)
        form.find('span:not(.card-header span, .select2, .select2 span, .input-group-text)').attr('hidden', true);

    if (!showDeleteButton)
        form.find('.btn-delete').attr('hidden', true);
}


//This function is to undo the uic.FormReadonly function, usefull for a edit button
uic.formEditable = function (form) {
    form = $(form);
    form.find('.form-control:not(.always-readonly)')
        //.addClass("form-control")
        //.removeClass("form-control-plaintext")
        .attr("readonly", false)
        .attr("disabled", false);

    form.removeClass('readonly-form');
    //form.find('.select2-container:not(.always-readonly)')
    //    .addClass("select2-container--bootstrap4")
    //    .find('.select2-selection__rendered').removeClass('px-0');

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
            uic.SetValue(subElement, val);
        });
        
    }
    else if (typeof value == "object" && value != null) {
        var properties = uic.getProperties(element);
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
        contextMenu.menuItems.push(menuItem);
    },
    addCategory: function (category) {
        contextMenu.categories.push(category);
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
                        result.append(contextMenu.default.divider.clone());

                    var menuItems = contextMenu._sort(category._menuItems);

                    menuItems.forEach(function (value, index) {
                        result.append(value._renderedElement);
                    });

                    if (addDividers)
                        result.append(contextMenu.default.divider.clone());

                    return result;
                },

                iconsOnly: (category, addDividers) => {

                    var results = $('<div>').css('order', category.menuItem?.position || 1000);
                    var itemRow = $('<li>');

                    if (addDividers)
                        results.append(contextMenu.default.divider.clone());

                    var menuItems = contextMenu._sort(category._menuItems);

                    menuItems.forEach(function (value) {
                        var element = $(value._renderedElement).find('.dropdown-item');

                        //If no title is available, set the item text as title
                        element.attr('title', $(value._renderedElement).attr('title') || element.text());

                        element.addClass('d-inline').textNodes().remove();

                        itemRow.append(element);
                    });
                    results.append(itemRow);

                    if (addDividers)
                        results.append(contextMenu.default.divider.clone());

                    return results;
                },

                subMenu: async (category, addDividers) => {
                    let results = $('<div>').css('order', category.menuItem?.position || 1000);

                    if (!category.menuItem)
                        console.error('Category requires menuItem', category);

                    let sublistPromise = contextMenu._getItemPromise(category.menuItem);
                    await Promise.resolve(sublistPromise).then((result) => {
                        let subItem = contextMenu._processSingleItemPromises(result);

                        let subMenu = $(subItem._renderedElement).addClass('dropdown-submenu').addClass('dropdown-hover');
                        subMenu.find('a').addClass('dropdown-toggle').attr('role', 'button').click(function (e) { e.stopPropagation(); e.preventDefault(); });

                        if (!subMenu.find('[disabled]').length) {
                            let subList = $('<ul>', { class: 'dropdown-menu' });
                            let menuItems = contextMenu._sort(category._menuItems);

                            menuItems.forEach(function (value) {
                                subList.append(value._renderedElement);
                            });

                            subList = contextMenu._cleanUp(subList);
                            subMenu.append(subList);
                        }

                        results.append(subMenu);
                    })

                    if (addDividers)
                        results.append(contextMenu.default.divider.clone());


                    return results;
                }
            },
        },

        //The html of the context-menu dropdown
        menu: function () {
            var menu = $('<div>', { id: 'contextMenu', class: 'context-menu dropdown show' }).append($('<ul>', { class: 'dropdown-menu show dropdown-icons-left' }));
            return menu;
        }
    },



    // #region Actual script code

    //Find a menuItem in the collection
    //element is jQuery, Id or htmlTag
    find: function (element) {
        if (element instanceof jQuery)
            return $(contextMenu.MenuItems).find(element);

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

        var results = $.map(contextMenu.menuItems, function (menuItem) {
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

    rightClick: async function (e) {
        if (!uic.contextMenu.enabled && !uic.contextMenu.altKeyEnabled)
            return;
        if (uic.contextMenu.enabled && uic.contextMenu.altKeyEnabled)
            return;

        if (!uic.contextMenu.enabled && uic.contextMenu.altKeyEnabled)
            uic.contextMenu.enabled = true;

        uic.contextMenu.target = e.target;
        //console.log('rightClickEvent', e);

        if ($('.context-menu').length) {
            uic.contextMenu.hideMenu();
            e.preventDefault();
        }
        else {
            var menu = uic.contextMenu.default.menu();
            $(menu).css('left', `${e.pageX}px`);
            $(menu).css('top', `${e.pageY}px`);


            let menuItems = uic.contextMenu.findForElement(contextMenu.Target, true);
            if (menuItems != null && menuItems.length && menuItems.filter(x => !!x.optional).length) {
                //Sort menuItems on their position
                menuItems = uic.contextMenu._sort(menuItems);
                

                e.preventDefault();

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
                element = await element($(menuItem._target), $(contextMenu.Target));
            }

        

            if (!isHTML(element) && !element instanceof jQuery) {
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
                text = await text($(menuItem._target), $(contextMenu.Target));
            }

            // text != 'mijn text'
            if (!isHTML(text) && 'string' == typeof text) {
                    text = `<a class="dropdown-item" href="#">${text}</a>`;

            }


            if (!isHTML(text) && !text instanceof jQuery) {
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
                icon = await icon($(menuItem._target), $(contextMenu.Target));
            }

            // icon == 'fas fa-icon'
            if (!isHTML(icon) && 'string' == typeof icon) {
                icon = `<i class="${icon}"></i>`;

            }


            if (!isHTML(icon) && !obj instanceof jQuery) {
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

                title = await title($(menuItem._target), $(contextMenu.Target));
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

                attr = await attr($(menuItem._target), $(contextMenu.Target));
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

﻿var uic = uic || {};
uic.delayedAction = uic.delayedAction || {


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
﻿var uic = uic || {};

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
                    makeToast("Error", "", response.Message);
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

uic.partial = uic.partial || {
    reload: function (element) {
        let parent = $(element).closest('.partial-source');
        if (parent.length)
            parent.trigger('uic-reload');
        else
            location.reload();
    },
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
        }
    ],


    _reloadPartial: async function (partial, showOverlay, getDatafunc) {
        if (!partial.length)
            return;

        await partial.triggerHandler('uic-before-reload');
        if (showOverlay)
            uic.partial.showLoadingOverlay(partial);

        let result = await getDatafunc();

        partial.html(result);

        if (showOverlay)
            uic.partial.hideLoadingOverlay(partial);

        await partial.triggerHandler('uic-reloaded');
    }
};﻿uic.sidePanel = uic.sidePanel || {
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
                uic.sidePanel.SetCollapse(container);
                break;
            case 1:
                uic.sidePanel.SetOverlay(container);
                break;
            case 2:
                uic.sidePanel.SetPinned(container);
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
        uic.sidePanel.SaveState(container, 0);
    },
    setOverlay: function (container) {
        let sidePanel = container.find('.side-panel');
        sidePanel.removeClass('col col-1 fit-content');

        sidePanel.removeClass('collapsed fixed');
        sidePanel.addClass('overlay');

        sidePanel.find('.btn-sidebar-fixed').removeClass('d-none');
        container.find('.btn-sidebar-open').addClass('d-none');
        uic.sidePanel.SaveState(container, 1);
    },
    setPinned: function (container) {

        let sidePanel = container.find('.side-panel');
        sidePanel.addClass('col col-1 fit-content');

        sidePanel.removeClass('collapsed overlay');
        sidePanel.addClass('fixed');

        sidePanel.find('.btn-sidebar-fixed').addClass('d-none');
        container.find('.btn-sidebar-open').addClass('d-none');
        uic.sidePanel.SaveState(container, 2);
    }
};
﻿uic.tabs = uic.tabs || {
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


            tabContent.triggerHandler('uic-open');
            oldActiveContent.triggerHandler('uic-close');

            let tabContainer = tab.closest('.card-tabs');
            tabContainer.triggerHandler('uic-tab-change', oldActiveHeader, tabHeader);
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

$(document).on('click', '[role="tab"]', async function (ev) {

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
});