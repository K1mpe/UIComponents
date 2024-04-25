uic.jsgrid = uic.jsgrid || {
    onInit: async function (id, loadUserPreference, args, sorter) {
        var loadedFilter = {};
        if (loadUserPreference) {
            try {
                //Download users last filters and set them in the filter row
                var filterJson = localStorage.getItem(`Grid.${id}.Filters`) || '{}';
                loadedFilter = $.parseJSON(filterJson);
                setTimeout(() => {
                    //console.log('grid-filters', loadedFilter, filterJson);
                    $(args.grid.fields).each(function (index, item) {
                        {
                            var fieldName = item.name || '';
                            $.each(loadedFilter, function (key, element) {
                                {
                                    if (key == fieldName && element != '' && item.filterControl != undefined) {
                                        {
                                            item.filterControl.val(element);
                                        }
                                    }
                                }
                            });
                        }
                    });
                }, 10);

            } catch (ex) {
                console.error('Failed to save userPreference', ex);
            }

        }

        //This timeout of 0 ms is to enable the jsGrid to render before sorting.
        setTimeout(() => {
            //Load grid data
            try { //Try to sort data
                if (sorter != null)
                    $(`#${id}`).jsGrid('sort', sorter);
            } catch { }
            $(`#${id}`).jsGrid('loadData');
        }, 20);
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
                if (filterValue === null || filterValue === undefined || filterValue ==="")
                    continue;
                if(item[filterName] === undefined)
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


        return filtered;

    },
    pageClientSide: function (data, rowCount) {
        if (data["data"] != undefined)
            return data;

        return {
            itemsCount: data.length,
            data: data.slice(0, rowCount)
        };
    },
    loadData: async function (args, id, saveFiltersInUserPreference, dataSource, whereCondition, additionalData) {
        if (saveFiltersInUserPreference) {
            try {
                await localStorage.setItem(`Grid.${id}.Filters`, JSON.stringify(args));
            }
            catch { }
        }

        var filter = $(`#${id}`).triggerHandler('Grid.GetFilters');
        if (filter == undefined)
            filter = args;
        else {
            try {
                filter.pageIndex = args.pageIndex;
                filter.pageSize = args.pageSize;
                filter.sortField = args.sortField;
                filter.sortOrder = args.sortOrder;
            }
            catch { }
        }

        var response = await uic.getpost.post(dataSource, { filter: filter, gridcondition: whereCondition, data: additionalData }, { CancelPreviousRequests: true });
        return response;
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
            if (value == null)
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
            return $("<input>").attr("type", "date")
                .prop("readonly", !!this.readOnly);
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

    r.datetime = $.extend({}, r.dateonly, {
        format: 'L LT'
    });

    r.decimal = $.extend({}, jsGrid.fields.number.prototype, {
        insertValue: function () {
            return this.insertControl.val()
                ? parseFloat(this.insertControl.val())
                : null;
        },

        editValue: function () {
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
            let _this = this;

            var element = $("<input>", { "class": this.class, "type": "color", "value": value }).on('input', function () {
                item[_this.name] = $(this).val();
            });

            return element;
        }
    };

    r.selectlist = {
        align: "center",

        nullIsEmptyString: true,
        nullable: true,

        itemTemplate: function (value) {
            var items = this.items,
                valueField = this.valueField,
                textField = this.textField,
                resultItem;

            if (this.nullIsEmptyString && value === null)
                value = '';

            if (valueField) {
                resultItem = $.grep(items, function (item, index) {
                    return item[valueField] == value;
                })[0] || {};
            }
            else {
                resultItem = items[value];
            }

            var result = (textField ? resultItem[textField] : resultItem);

            return (result === undefined || result === null) ? "" : result;
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
            return this._renderCheckbox(value, true, this.nullable);
        },
        editTemplate: function (value, item) {
            let checkbox = this._renderCheckbox(value, true, this.nullable);
            return checkbox;
        },
        filterTemplate: function () {
            let checkbox = this._renderCheckbox(null, true, true);
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

                if (nullable) {
                    uic.form.setThreestateToggles(checkbox);
                }
            }
            return group;
        }
    });
}
$(document).ready(() => {
    let SetRenderer = (name)=> {
        let target = uic.jsgrid.fieldRenderers[name];
        if (typeof target ==='function')
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
    SetRenderer("timespan");
});
