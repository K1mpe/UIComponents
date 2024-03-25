uic.contextMenu = uic.contextMenu || {


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
