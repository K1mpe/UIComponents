uic.jsgrid = uic.jsgrid || {
    fieldRenderers: {
        string: new jsGrid.Field({

        }),
    },
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
};