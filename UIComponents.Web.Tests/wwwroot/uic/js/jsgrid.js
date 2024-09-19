uic.jsgrid = uic.jsgrid || {
    help: function(id){
        console.log($(`#${id}`));
        console.log(`$('#${id}').trigger('uic-reload') => Reload the table if allowed.`);
        console.log(`$('#${id}').trigger('uic-forceReload') => Always reload the table. (Not recommended!)`);
        console.log(`$('#${id}').trigger('uic-disableReload') => Disable reloading the table`);
        console.log(`$('#${id}').trigger('uic-enableReload') => Enable reloading the table. Will also trigger reload if a reload has failed`);
        console.log(`$('#${id}').trigger('uic-addReloadCondition', ()=> {...}) => Add a new function that is checked on each reload. Returning false will disable the reload`);
        console.log(`$('#${id}').trigger('uic-reloadConditionChanged') => One or more of the reload conditions have changed.`);
        console.log(`$('#${id}').triggerHandler('uic-currentData') => Get the data that is currently displayed in the table`);
        console.log(`$('#${id}').triggerHandler('uic-getLastFilters') => Get the last used filters for loading data`);
        console.log(`$('#${id}').on('uic-getFilters', (ev, args)=> {... return args}) => Called before loading data. Using this callback you can overwrite the filter data or sorting.`);
        console.log(`$('#${id}').on('uic-beforeFetch', (ev, args)=> {...}) => Triggered just before the LoadData function is called.`);
        console.log(`$('#${id}').on('uic-afterFetch', (ev, data, args)=> {...}) => Triggered after the LoadData function is called.`);
        console.log(`$('#${id}').on('uic-beforeInsert', (ev, item)=> {...}) => Triggered before inserting a item in the table.`);
        console.log(`$('#${id}').on('uic-afterFetch', (ev, item)=> {...}) => Triggered after inserting a item in the table.`);
        console.log(`$('#${id}').on('uic-beforeUpdate', (ev, item)=> {...}) => Triggered before updating a item in the table.`);
        console.log(`$('#${id}').on('uic-beforeUpdate', (ev, item)=> {...}) => Triggered after updating a item in the table.`);
        console.log(`$('#${id}').on('uic-beforeDelete', (ev, item)=> {...}) => Triggered before deleting a item in the table.`);
        console.log(`$('#${id}').on('uic-afterDelete', (ev, item)=> {...}) => Triggered after deleting a item in the table.`);
        console.log(`$('#${id}').on('uic-dataLoaded', (args)=> {...}) => Triggered after data is loaded.`);
        console.log(`$('#${id}').on('uic-editingData', (args)=> {...}) => Triggered when opening a edit row.`);
        console.log(`$('#${id}').on('uic-dataLoadedAndFiltered') => Triggered after the selectlists are filtered.`);
    },
    onInit: async function (id, loadUserPreference, args, sorter, defaultFilter) {
        var loadedFilter = defaultFilter;
        if (loadUserPreference) {
            try {
                //Download users last filters and set them in the filter row
                var filterJson = localStorage.getItem(`Grid.${id}.Filters`) || '{}';
                loadedFilter = $.parseJSON(filterJson);


            } catch (ex) {
                console.error('Failed to save userPreference', ex);
            }

        }
        setTimeout(() => {
            //console.log('grid-filters', loadedFilter, filterJson);
            $(args.grid.fields).each(function (index, item) {
                {
                    try {
                        var fieldName = item.name || '';
                        $.each(loadedFilter, function (key, value) {
                            {
                                if (key == fieldName && value != '') {
                                    if (item.filterControl != undefined) {
                                        item.filterControl.val(value);
                                    } else {
                                        let filterElement = item._grid._filterRow.find('td')[index];
                                        uic.setValue(filterElement, value)
                                    }
                                }
                            }
                        });
                    } catch { }
                }
            });

            try { //Try to sort data
                if (sorter != null)
                    $(`#${id}`).jsGrid('sort', sorter);
            } catch { }
            setTimeout(() => {
                $(`#${id}`).trigger('uic-reload');
            }, 1);
            
        }, 1);

    },


    //filterSelectFilters = bool => if true will only show filters that are available in results.
    //this option will not work when using a paged list!
    filterSelectListFilters: async function (args, id) {

        var grid = args.grid;
        var data = grid.data;
        for (var i = 0; i < grid.fields.length; i++) {
            {
                var field = grid.fields[i];
                if (!field["items"])
                    continue;
                if (!field["all_items"])
                    field["all_items"] = field["items"];
                else if (grid.pagesize <= data.length)
                    field["items"] = field["all_items"]; //undo filter if paged list
                if (grid.pageSize > data.length)
                    field["items"] = field["all_items"].filter(item => !item.Value || data.some(d => { { return d[field.name] == item.Value } }));
                field["filterControl"].find("option").each((index, option) => {
                    {
                        if (field["items"].some(item => item.Value == option.value))
                            $(option).show().prop("disabled", false);
                        else
                            $(option).hide().prop("disabled", true);
                    }
                });
                field.items = field.all_items;
                $(`#${id}`).trigger('uic-dataLoadedAndFiltered');
            }
        }
    },


    filterClientSide: function (filter, data) {
        let filtered = [];

        var filterNames = Object.getOwnPropertyNames(filter);

        for (var i = 0; i < data.length; i++) {
            let item = data[i];
            let match = true;

            for (var j = 0; j < filterNames.length; j++) {
                let filterName = filterNames[j];
                let filterValue = filter[filterName];
                if (filterValue === null || filterValue === undefined || filterValue === "")
                    continue;
                if (item[filterName] === undefined)
                    continue;

                //console.log(typeof item[filterName]);

                // filter
                if (typeof filterValue === "string") {
                    match = (item[filterName] || "").toString().toLowerCase().includes(filterValue.toLowerCase())
                }
                else if (typeof filterValue === "object") {
                    //Check if filter is dateTime
                    if (filterValue.hasOwnProperty('Start') && filterValue.hasOwnProperty('End')) {
                        var date = new moment(item[filterName]);
                        var start = new moment(filterValue["Start"]);
                        var end = new moment(filterValue["End"]);


                        if (filterValue.hasOwnProperty('Start'))
                            if (date < start)
                                match = false;
                        if (filterValue.hasOwnProperty('End'))
                            if (date > end)
                                match = false;
                    }
                } else {
                    match = filterValue == item[filterName];
                }


                if (match === false)
                    break;
            }

            if (match)
                filtered.push(item);
        }

        if (!!filter.sortField) {
            if (filter.sortOrder == "asc") {
                filtered = filtered.sort((a, b) => {
                    if (a[filter.sortField] > b[filter.sortField])
                        return 1;
                    else if (b[filter.sortField] > a[filter.sortField])
                        return -1;
                    else return 0;
                });
            } else {
                filtered = filtered.sort((a, b) => {
                    if (a[filter.sortField] > b[filter.sortField])
                        return -1;
                    else if (b[filter.sortField] > a[filter.sortField])
                        return 1;
                    else return 0;
                });
            }
        }
        return filtered;

    },

    //This function only pages if this has not been done serverside
    pageClientSide: function (data, pageIndex, pageSize) {
        if (data["data"] != undefined)
            return data;

        let start = (pageIndex - 1) * pageSize;
        return {
            itemsCount: data.length,
            data: data.splice(start, pageSize)
        };
    },


    //Create top and bottom pager if none exists
    pager_generate: function (id, pagerSelector) {
        if (!$(`${pagerSelector}`).length) {
            if (pagerSelector.startsWith(".")) {
                $(`#${id}-container`).prepend(`<div class="${pagerSelector.substring(1)}"></div>`);
                $(`#${id}-container`).append(`<div class="${pagerSelector.substring(1)}"></div>`);
            } else if (pagerSelector.startsWith("#")) {
                $(`#${id}-container`).prepend(`<div id="${pagerSelector.substring(1)}"></div>`);
            }

        };
    },

    defaultHeaderTemplate: function (title, titleVisibiltiy, icon, iconvisibility, tooltip) {
        let el = $('<span>', { title: tooltip || title });

        if (icon != null)
            el.append(icon.addClass(iconvisibility));
        el.append($('<span>', { class: titleVisibiltiy }).append(title));
        return el;
    },

    controlOverride: {
        conditionalEditButton: function (item) {
            if (this.editButtonCondition != null && !this.editButtonCondition(item))
                return;
            return this._createGridButton(this.editButtonClass, this.editButtonTooltip, function (b, c) {
                b.editItem(item),
                    c.stopPropagation()
            });
        },
        conditionalDeleteButton: function (item) {
            if (this.deleteButtonCondition != null && !this.deleteButtonCondition(item))
                return;
            return this._createGridButton(this.deleteButtonClass, this.deleteButtonTooltip, function (b, c) {
                b.editItem(item),
                    c.stopPropagation()
            });
        },
    },

    fieldRenderers: {}, //This has the prototypes for fieldRenderers that are used by UIC generators. These are configured in the next block to enable $.extend()
};
{
    let r = uic.jsgrid.fieldRenderers;

    r.dateonly = {
        css: "date-field",            // redefine general property 'css'
        align: "center",              // redefine general property 'align'

        format: "L",

        filter_start: null,
        filter_end: null,

        sorter: function (date1, date2) {
            return new Date(date1) - new Date(date2);
        },

        itemTemplate: function (value) {
            if (value == null || value == "")
                return null;
            var mValue = moment(value);
            //always show the full date
            return mValue.format(this.format);
        },

        insertTemplate: function (value) {
            if (!this.inserting)
                return "";

            return this.insertControl = this._createTextBox();
        },

        editTemplate: function (value) {
            if (!this.editing)
                return this.itemTemplate.apply(this, arguments);

            var $result = this.editControl = this._createTextBox();
            $result.val(value);
            return $result;
        },

        insertValue: function () {
            return this.insertControl.val();
        },

        editValue: function () {
            return this.editControl.val();
        },

        _createTextBox: function () {
            let input = $("<input>").attr("type", "date")
                .prop("readonly", !!this.readOnly);
            if (this.step != undefined)
                input.attr('step', this.step);
            if (this.min != undefined)
                input.attr('min', this.min);
            if (this.max != undefined)
                input.attr('max', this.max);
            return input;
        },

        filterTemplate: function () {
            let _this = this;
            // [ from(nullable) ][->][ to(nullable) ]

            let group = $("<div>", { "class": "input-group" });
            let input1 = $("<input>", { "class": "form-control" });
            // input from date
            group.append(input1);

            if ($.isFunction($.fn.daterangepicker)) {
                input1.daterangepicker({
                    format: "DD/MM/YYYY",
                    "singleDatePicker": true,
                    "autoApply": true,
                    "linkedCalendars": false,
                    "showCustomRangeLabel": false,
                    "timePicker": true,
                    "timePicker24Hour": true,
                    autoUpdateInput: false,
                }).on("apply.daterangepicker", (e, picker) => _this.applyFilterStart(picker, _this))
                    .on("change", (e, picker) => _this.applyFilterStart(picker, _this));
            }

            // divider
            group.append($("<div>", { "class": "input-group-prepend input-group-append" }).append($("<span>", { "class": "input-group-text fas fa-arrow-right" })));

            // input to date
            let input2 = $("<input>", { "class": "form-control" });
            group.append(input2);

            if ($.isFunction($.fn.daterangepicker)) {
                input2.daterangepicker({
                    format: "DD/MM/YYYY",
                    "singleDatePicker": true,
                    "autoApply": true,
                    "linkedCalendars": false,
                    "showCustomRangeLabel": false,
                    "timePicker": false,
                    autoUpdateInput: false,
                }).on("apply.daterangepicker", (e, picker) => _this.applyFilterEnd(picker, _this))
                    .on("change", (e, picker) => _this.applyFilterEnd(picker, _this));
            }

            return group;
        },
        applyFilterStart: function (picker, _this) {
            _this.filter_start = picker?.startDate || null;

            if (picker != null)
                $(picker.element).val(picker.startDate.format("DD/MM/YYYY"));

            _this._grid.loadData();
        },
        applyFilterEnd: function (picker, _this) {
            _this.filter_end = picker?.endDate || null;

            if (picker != null)
                $(picker.element).val(picker.endDate.format("DD/MM/YYYY"));

            _this._grid.loadData();
        },
        filterValue: function () {
            return {
                Start: this.filter_start ? this.filter_start.local().toISOString(true) : null,
                End: this.filter_end ? this.filter_end.local().toISOString(true) : null
            }
        }
    };

    r.datetime = {
        css: "date-field",            // redefine general property 'css'
        align: "center",              // redefine general property 'align'

        format: "L LT",

        filter_start: null,
        filter_end: null,

        sorter: function (date1, date2) {
            return new Date(date1) - new Date(date2);
        },

        itemTemplate: function (value) {
            if (value == null || value == "")
                return null;
            var mValue = moment(value);
            //always show the full date
            return mValue.format(this.format);
        },

        insertTemplate: function (value) {
            if (!this.inserting)
                return "";

            return this.insertControl = this._createTextBox();
        },

        editTemplate: function (value) {
            if (!this.editing)
                return this.itemTemplate.apply(this, arguments);

            var $result = this.editControl = this._createTextBox();
            $result.val(value);
            return $result;
        },

        insertValue: function () {
            return this.insertControl.val();
        },

        editValue: function () {
            return this.editControl.val();
        },

        _createTextBox: function () {
            let input = $("<input>").attr("type", "datetime-local")
                .prop("readonly", !!this.readOnly);
            if (this.step != undefined)
                input.attr('step', this.step);
            if (this.min != undefined)
                input.attr('min', this.min);
            if (this.max != undefined)
                input.attr('max', this.max);
            return input;
        },

        filterTemplate: function () {
            let _this = this;
            // [ from(nullable) ][->][ to(nullable) ]

            let group = $("<div>", { "class": "input-group" });
            let input1 = $("<input>", { "class": "form-control" });
            // input from date
            group.append(input1);

            if ($.isFunction($.fn.daterangepicker)) {
                input1.daterangepicker({
                    format: "DD/MM/YYYY",
                    "singleDatePicker": true,
                    "autoApply": true,
                    "linkedCalendars": false,
                    "showCustomRangeLabel": false,
                    "timePicker": true,
                    "timePicker24Hour": true,
                    autoUpdateInput: false,
                }).on("apply.daterangepicker", (e, picker) => _this.applyFilterStart(picker, _this))
                    .on("change", (e, picker) => _this.applyFilterStart(picker, _this));
            }

            // divider
            group.append($("<div>", { "class": "input-group-prepend input-group-append" }).append($("<span>", { "class": "input-group-text fas fa-arrow-right" })));

            // input to date
            let input2 = $("<input>", { "class": "form-control" });
            group.append(input2);

            if ($.isFunction($.fn.daterangepicker)) {
                input2.daterangepicker({
                    format: "DD/MM/YYYY",
                    "singleDatePicker": true,
                    "autoApply": true,
                    "linkedCalendars": false,
                    "showCustomRangeLabel": false,
                    "timePicker": true,
                    "timePicker24Hour": true,
                    autoUpdateInput: false,
                }).on("apply.daterangepicker", (e, picker) => _this.applyFilterEnd(picker, _this))
                    .on("change", (e, picker) => _this.applyFilterEnd(picker, _this));
            }

            return group;
        },
        applyFilterStart: function (picker, _this) {
            _this.filter_start = picker?.startDate || null;

            if (picker != null)
                $(picker.element).val(picker.startDate.format("DD/MM/YYYY"));

            _this._grid.loadData();
        },
        applyFilterEnd: function (picker, _this) {
            _this.filter_end = picker?.endDate || null;

            if (picker != null)
                $(picker.element).val(picker.endDate.format("DD/MM/YYYY"));

            _this._grid.loadData();
        },
        filterValue: function () {
            return {
                Start: this.filter_start ? this.filter_start.local().toISOString(true) : null,
                End: this.filter_end ? this.filter_end.local().toISOString(true) : null
            }
        }
    };

    r.decimal = $.extend({}, jsGrid.fields.number.prototype, {
        insertValue: function () {
            return this.insertControl.val()
                ? parseFloat(this.insertControl.val())
                : null;
        },

        editValue: function () {
            if (!this.editing)
                return this.itemTemplate.apply(this, arguments);
            return this.editControl.val()
                ? parseFloat(this.editControl.val())
                : null;
        },
    });

    r.hexcolor = {
        css: "",                        // redefine general property 'css'
        align: "center",                // redefine general property 'align'

        cellRenderer: function (value, item) {
            var cell = $("<td>");

            if (value) {
                var container = $("<div>").css({
                    "display": "flex",
                    "justify-content": "center",
                    "align-items": "center",
                });

                var textContainer = $("<div>").css({
                    "margin": "5px",
                });

                var colorBlock = $("<div>").css({
                    "background-color": value,
                    "width": "20px",
                    "height": "20px",
                    "margin-right": "5px",
                    "border": "1px solid black",
                });

                textContainer.append(this.itemTemplate(value, item));

                container.append([colorBlock, textContainer]);
                cell.append(container);
            }

            return cell;
        },
        //itemTemplate: function (value, item) {

        //    var element = $("<input>", { "class": this.class, "type": "color", "value": value }).on('input', function () {
        //        value = $(this).val();
        //    });

        //    return element;
        //},
        insertTemplate: function (value, item) {
            let _this = this;

            var element = $("<input>", { "class": this.class, "type": "color", "value": value }).on('input', function () {
                item[_this.name] = $(this).val();
            });

            return element;
        },
        editTemplate: function (value, item) {
            if (!this.editing)
                return this.itemTemplate.apply(this, arguments);
            let _this = this;

            var element = $("<input>", { "class": this.class, "type": "color", "value": value }).on('input', function () {
                item[_this.name] = $(this).val();
            });

            return element;
        }
    };

    const GRID_ROW_CONTENT_ARRAY = "_ROW_CONTENT_ARRAY";
    r.rowcontent = {
        align: "center",

        deleting: false,
        editing: false,
        details: false,
        sorting: false,

        url: null,
        icon_up: "fa fa-chevron-down",
        icon_down: "fa fa-chevron-right",
        multiple: false,
        before_cell_render: true,
        identifier: "Id",
        width: "25px",

        // Triggered when any changes happen
        rowContentChanged: $.noop,

        itemTemplate: function (value, item) {
            let _this = this;
            let element = $("<i>", { "class": _this.icon_down });

            return element;
        },

        cellRenderer: function (value, item) {
            if (!this.checkBeforeCellRender(item))
                return $("<td>");

            let _this = this;

            // Clicking the cell does not trigger the sidebar
            var cell = $("<td onclick=\"event.stopPropagation();\" class=\"rowcontent\">");

            cell.append(this.itemTemplate(value, item));

            cell.on("click", async function (e) {
                e.stopPropagation();


                if (_this._isRowOpened(_this._grid, item)) {
                    // close this item 
                    _this._closeRow(_this._grid, item);

                    cell.trigger('uic-closePartial');
                }
                else {
                    if (!_this.multiple) {
                        // close any items that are open
                        _this._closeRows(_this._grid);
                    }

                    cell.trigger('uic-openPartial');
                    // open this item
                    _this._openRow(_this._grid, item);
                }


            });

            return cell;
        },

        insertTemplate: $.noop,
        editTemplate: $.noop,
        insertValue: $.noop,
        editValue: $.noop,

        _init: function (grid) {
            if (!Array.isArray(grid[GRID_ROW_CONTENT_ARRAY]))
                grid[GRID_ROW_CONTENT_ARRAY] = [];
        },

        _isRowOpened: function (grid, item) {
            let _this = this;
            _this._init(grid);

            let entry = grid[GRID_ROW_CONTENT_ARRAY].find(entry => entry.item[_this.identifier] === item[_this.identifier]);
            if (!entry)
                return false;

            // Check if the content is still connected
            if (!entry.content[0].isConnected) {
                // remove from the array
                let index = grid[GRID_ROW_CONTENT_ARRAY].indexOf(entry);
                grid[GRID_ROW_CONTENT_ARRAY].splice(index, 1);

                return false;
            }

            return true;
        },

        _closeRow: function (grid, item) {
            let _this = this;
            _this._init(grid);

            // Find the entry on the grid
            let entry = grid[GRID_ROW_CONTENT_ARRAY].find(entry => entry.item[_this.identifier] === item[_this.identifier]);
            if (!entry)
                return;

            // Remove from the grid array
            let index = grid[GRID_ROW_CONTENT_ARRAY].indexOf(entry);
            grid[GRID_ROW_CONTENT_ARRAY].splice(index, 1);

            // Remove the HTML
            entry.content.remove();

            // Set the correct icon
            entry.row.find(`i.${_this.icon_up.replace(" ", ".")}`)
                .removeClass(_this.icon_up)
                .addClass(_this.icon_down);
        },

        _closeRows: function (grid) {
            let _this = this;
            _this._init(grid);

            for (var index = 0; index < grid[GRID_ROW_CONTENT_ARRAY].length; index++) {
                _this._closeRow(grid, grid[GRID_ROW_CONTENT_ARRAY][index].item);
            }
        },

        _openRow: async function (grid, item) {
            let _this = this;
            _this._init(grid);

            let row = grid.rowByItem(item);
            let content = _this._appendEmptyRow(row, item);

            content.trigger('uic-reload');

            grid[GRID_ROW_CONTENT_ARRAY].push({
                item: item,
                row: row,
                content: content
            });
        },

        _appendEmptyRow: function (itemRow, item) {

            let colspan = 0;
            this._grid._header.find('.jsgrid-header-row th').each((index, field) => {
                if (!uic.form.isHidden(field))
                    colspan += 1;
            });

            this._checkResizeColspan(this._grid._container);
            let contentRow = $("<tr>", { "class": "jsgrid-row" })
                .append($("<td>", { "class": "jsgrid-cell jsgrid-partial-content", "colspan": colspan }));

            contentRow.on('uic-reload', async (ev) => {
                ev.stopPropagation();
                uic.partial.showLoadingOverlay(contentRow);
                let content = await this.loadContent(null, item);
                uic.partial.hideLoadingOverlay(contentRow);
                if (content != false)
                    contentRow.find('td:first').html(content);
            })
            itemRow.after(contentRow);

            // Change the icon
            itemRow.find(`i.${this.icon_down.replace(" ", ".")}`)
                .removeClass(this.icon_down)
                .addClass(this.icon_up);

            return contentRow;
        },

        _checkResizeColspan: function (container) {
            if (container.hasClass('jsgrid-resizeablePartial'))
                return;

            if (!uic.jsgrid._hasResizeWatcher) {
                $(window).resize(() => {
                    $('.jsgrid-resizeablePartial').each((index, grid) => {
                        let container = $(grid);
                        if (!container.find('.jsgrid-partial-content').length)
                            return;

                        let partialIndex = container.find('.jsgrid-partial-content').closest('tr').index();
                        let partialSourceRow = $(container.find('.jsgrid-grid-body tr')[partialIndex - 1]);
                        if (!partialSourceRow.find('.rowcontent:visible').length) {
                            container.find('.jsgrid-partial-content').closest('tr').remove();
                            return;
                        }
                        let colspan = 0;
                        container.find('.jsgrid-header-row th').each((index, field) => {
                            if (!uic.form.isHidden(field))
                                colspan += 1;
                        });

                        container.find('.jsgrid-partial-content').attr('colspan', colspan);
                    })
                });
                uic.jsgrid._hasResizeWatcher = true;
            }

            container.addClass("jsgrid-resizeablePartial");
        }

    }

    r.selectlist = {
        //align: "center",

        nullIsEmptyString: true,
        nullable: true,

        itemTemplate: function (value) {
            var items = this.items;
            let text = value;
            if (text == "0")
                text = "";
            let textVal = "" + value; // make sure the value is seen as text
            let item = items.filter(x => x.Value === textVal);
            return item[0]?.Text || text;
        },

        filterTemplate: function () {
            if (!this.filtering)
                return "";

            var grid = this._grid,
                $result = this.filterControl = this._createSelect();

            if (this.autosearch) {
                $result.on("change", function (e) {
                    grid.search();
                });
            }

            return $result;
        },

        insertTemplate: function () {
            if (!this.inserting)
                return "";

            return this.insertControl = this._createSelect();
        },

        editTemplate: function (value) {
            if (!this.editing)
                return this.itemTemplate.apply(this, arguments);

            var $result = this.editControl = this._createSelect();

            // When editing no nullable option should be available
            if (this.nullable === false) {
                $result.find("option:not([value])").prop("disabled", true).hide();
            }

            (value !== undefined) && $result.val(value);
            return $result;
        },

        filterValue: function () {
            var val = this.filterControl.val();
            return val;
        },

        insertValue: function () {
            var val = this.insertControl.val();
            return val;
        },

        editValue: function () {
            var val = this.editControl.val();
            return val;
        },

        _createSelect: function () {
            var $result = $("<select>"),
                valueField = this.valueField,
                textField = this.textField,
                selectedIndex = this.selectedIndex;

            $.each(this.items, function (index, item) {
                var value = item.Value;
                var text = item.Text;

                var $option = $("<option>")
                    .attr("value", value)
                    .text(text)
                    .appendTo($result);

                $option.prop("selected", (selectedIndex === index));
            });

            $result.prop("disabled", !!this.readOnly);

            return $result;
        },
    };

    r.timeonly = {
        css: "date-field",            // redefine general property 'css'
        align: "center",              // redefine general property 'align'

        format: "LTS",

        filter_start: null,
        filter_end: null,

        sorter: function (date1, date2) {
            return new Date(date1) - new Date(date2);
        },

        itemTemplate: function (value) {
            if (value == null || value == "")
                return null;
            var mValue = moment(value);
            //always show the full date
            return mValue.format(this.format);
        },

        insertTemplate: function (value) {
            if (!this.inserting)
                return "";

            return this.insertControl = this._createTextBox();
        },

        editTemplate: function (value) {
            if (!this.editing)
                return this.itemTemplate.apply(this, arguments);

            var $result = this.editControl = this._createTextBox();
            $result.val(value);
            return $result;
        },

        insertValue: function () {
            return this.insertControl.val();
        },

        editValue: function () {
            return this.editControl.val();
        },

        _createTextBox: function () {
            let input = $("<input>").attr("type", "time")
                .prop("readonly", !!this.readOnly);
            if (this.step != undefined)
                input.attr('step', this.step);
            if (this.min != undefined)
                input.attr('min', this.min);
            if (this.max != undefined)
                input.attr('max', this.max);
            return input;
        },

        filterTemplate: function () {
            let _this = this;
            // [ from(nullable) ][->][ to(nullable) ]

            let group = $("<div>", { "class": "input-group" });
            let input1 = $("<input>", { "class": "form-control" });
            // input from date
            group.append(input1);

            if ($.isFunction($.fn.daterangepicker)) {
                input1.daterangepicker({
                    format: "DD/MM/YYYY",
                    "singleDatePicker": true,
                    "autoApply": true,
                    "linkedCalendars": false,
                    "showCustomRangeLabel": false,
                    "timePicker": true,
                    "timePicker24Hour": true,
                    autoUpdateInput: false,
                }).on("apply.daterangepicker", function (e, picker) {
                    _this.filter_start = picker.startDate;

                    $(this).val(picker.startDate.format("DD/MM/YYYY"));

                    _this._grid.loadData();
                });
            }

            // divider
            group.append($("<div>", { "class": "input-group-prepend input-group-append" }).append($("<span>", { "class": "input-group-text fas fa-arrow-right" })));

            // input to date
            let input2 = $("<input>", { "class": "form-control" });
            group.append(input2);

            if ($.isFunction($.fn.daterangepicker)) {
                input2.daterangepicker({
                    format: "DD/MM/YYYY",
                    "singleDatePicker": true,
                    "autoApply": true,
                    "linkedCalendars": false,
                    "showCustomRangeLabel": false,
                    "timePicker": true,
                    "timePicker24Hour": true,
                    autoUpdateInput: false,
                }).on("apply.daterangepicker", function (e, picker) {
                    _this.filter_end = picker.startDate;

                    $(this).val(picker.startDate.format("DD/MM/YYYY"));

                    _this._grid.loadData();
                });
            }

            return group;
        },

        filterValue: function () {
            return {
                Start: this.filter_start ? this.filter_start.local().toISOString(true) : null,
                End: this.filter_end ? this.filter_end.local().toISOString(true) : null
            }
        }
    };
    r.timespan = {
        css: "timespan-field",            // redefine general property 'css'
        align: "center",              // redefine general property 'align'

        format: "",

        filter_start: null,
        filter_end: null,
        sorter: function (date1, date2) {
            return new Date(date1) - new Date(date2);
        },

        itemTemplate: function (value) {
            if (value == null)
                return null;
            var mValue = moment.duration(value);
            if (mValue.asMilliseconds() == 0)
                return null;

            //always show the full date
            return mValue.humanize();
        },

        insertTemplate: function (value) {
            if (!this.inserting)
                return "";

            return this.insertControl = this._createTextBox();
        },

        editTemplate: function (value) {
            if (!this.editing)
                return this.itemTemplate.apply(this, arguments);

            var $result = this.editControl = this._createTextBox();
            $result.val(value);
            return $result;
        },

        insertValue: function () {
            return this.insertControl.val();
        },

        editValue: function () {
            return this.editControl.val();
        },

        _createTextBox: function () {
            return $("<input>").attr("type", "text")
                .prop("readonly", !!this.readOnly);
        },

        cellRenderer: function (value, item) {
            let content = this.itemTemplate(value, item);
            let cell = $('<td>').attr('title', this.tooltip || value);
            cell.append(content);

            return cell;
        }


    }

    r.toggle = $.extend({}, r.checkbox, {
        css: "toggle",
        align: "center",
        nullable: false,
        sorter: function (check1, check2) {
            console.log('sort checkbox', check1, check2);
            if (check1 == check2)
                return 0;

            if (check1 > check2)
                return 1;
            else
                return -1;
        },
        itemTemplate: function (value, item) {
            let toggle = this._renderCheckbox(value, true, this.nullable);
            toggle.on('click', (ev) => { ev.preventDefault(); }) //disable the togglebutton in normal row (not insert, filter or edit)
            return toggle;
        },
        insertTemplate: function () {
            let value = this.nullable ? null : false;
            let checkbox = this._renderCheckbox(value, true, this.nullable);
            return checkbox;
        },
        insertValue: function () {
            return uic.getValue(this._grid._insertRow.find(`input[name="${this.name}"]`));
        },
        editTemplate: function (value, item) {
            if (!this.editing) {
                let result = this.itemTemplate.apply(this, arguments);
                result.find('input').attr('disabled', true).attr('readonly', true)
                return result;
            }


            let checkbox = this._renderCheckbox(value, true, this.nullable);
            return checkbox;
        },
        editValue: function () {
            return uic.getValue(this._grid._container.find(`.jsgrid-edit-row input[name="${this.name}"]`));
        },
        filterTemplate: function () {
            let checkbox = this._renderCheckbox(null, true, true);
            if (this.autosearch)
                checkbox.on('change', () => { this._grid.search(); });

            //checkbox.on('change', () => {
            //    setTimeout(() => this._grid.search(), 50);
            //});

            return checkbox;
        },

        filterValue: function () {
            return uic.getValue(this._grid._filterRow.find(`input[name="${this.name}"]`));
        },

        _renderCheckbox: function (value, withId, nullable) {
            let checkbox = $('<input>', { type: 'checkbox', class: 'custom-control-input', 'data-value': value, name: this.name });
            if (nullable)
                checkbox.addClass('three-state-checkbox');

            let group = $('<div>', { class: 'custom-control custom-switch icheck-primary' })
                .append(checkbox)
                .append($('<label>', { class: 'custom-control-label' }));

            if (withId) {
                let id = "check" + Math.floor(Math.random() * 100000);
                checkbox.attr('id', id);
                group.find('label').attr('for', id);

                uic.setValue(checkbox, value);
                if (nullable) {
                    uic.form.setThreestateToggles(checkbox);
                }
            }
            return group;
        }
    });
}
$(document).ready(() => {
    let SetRenderer = (name) => {
        let target = uic.jsgrid.fieldRenderers[name];
        if (typeof target === 'function')
            target = target();

        let fieldFunc = function (config) {
            jsGrid.Field.call(this, config);
        }
        fieldFunc.prototype = new jsGrid.Field(target);
        jsGrid.fields[name] = fieldFunc;
    };

    SetRenderer("dateonly");
    SetRenderer("datetime");
    SetRenderer("decimal");
    SetRenderer("hexcolor");
    SetRenderer("toggle");
    SetRenderer("selectlist");
    SetRenderer("timeonly");
    SetRenderer("timespan");
    SetRenderer("rowcontent");
});
