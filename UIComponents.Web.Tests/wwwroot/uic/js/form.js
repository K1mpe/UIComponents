uic.form = uic.form || {
    help: function (id) {
        console.log(`$('#${id}').trigger('submit') => Submit this form`);
        console.log(`await $('#${id}').triggerHandler('awaitSubmit') => Submit this form and return the result`);
    },

    //Check if this element or any parent is hidden or collapsed
    isHidden: function (element) {
        element = $(element);
        if (!element.length)
            return true;

        if (!element.is(':visible') && element.css('display') != 'contents') //contents is always marked as not visible
            return true;
        if (element.closest('[hidden]').length)
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

        form.trigger('uic-formReadonly');
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

        form.removeClass('readonly-form').removeClass('hide-empty');
        form.trigger('uic-formEditable');
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
        if (value == undefined) {
            value == null;
            item.data('value', 'null');
        }

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
        item.change();
    },

    delete: async function (url, data) {
        try {
            var content = await uic.getpost.post(url, data);
            $('body').append(content);

        } catch (ex) {
            console.error(ex);
        }
    },

    //Alert the user that there are unsaved changes before enabling save
    warnUserForConflicts: async function (form) {
        //stop if no changes are found
        let nrOfConflicts = form.find('.uic-value-changed').length;
        if (!nrOfConflicts)
            return true;

        var translations = await uic.translation.translateMany([
            TranslatableSaver.Save("UIC.Form.SaveFormWithConficts.Title", "There are still conflicts", nrOfConflicts),
            TranslatableSaver.Save("UIC.Form.SaveFormWithConficts.Message", "Do you want to ignore these conflicts?"),
            TranslatableSaver.Save("Button.Continue"),
            TranslatableSaver.Save("Button.Cancel"),
        ]);

        let result = await Swal.fire($.extend(true, {}, uic.defaults.swal,
            {
                title: translations["UIC.Form.SaveFormWithConficts.Title"],
                text: translations["UIC.Form.SaveFormWithConficts.Message"],
                showCloseButton: true,
                showCancelButton: true,
                confirmButtonText: translations["Button.Continue"],
                cancelButtonText: translations["Button.Cancel"],
            }));
        return result.isConfirmed;
    },

    setPopoverOnClickTooltipIcon: function () {
        $('.tooltip-icon').each((index, item) => {
            let title = $(item).closest('[title]').attr('title');
            if (title.length) {
                $(item).popover({
                    content: title,
                    trigger: 'focus click',
                });
                $(item).attr('title', null);
            }
            $(item).on('click', ev => {
                ev.preventDefault();
            });
            uic.partial.onDispose(item, () => $(item).popover('dispose'));
        })
    },
    setThreestateToggles: function ($element) {
        $element.addClass('configured');
        $element.off('click');
        $element.each((index, item) => {
            uic.form.setThreeState($(item));
        });
        $element.on('click', (ev) => {
            ev.preventDefault();
            ev.stopPropagation();
            ev.stopImmediatePropagation();

            let target = $(ev.target).closest('input');
            let oldVal = target.data('value');
            if (oldVal == "null" || oldVal == "")
                target.data('value', "true");
            else if (oldVal == "false")
                target.data('value', "null");
            else
                target.data('value', "false");

            uic.form.setThreeState(target);
            setTimeout(() => uic.form.setThreeState(target), 1);
        });
    },

    selectlistItems: {

        //Method is not yet completed!
        convertJsonToSelectOptions: function (selectListItems, sorting, noItemText) {

            let results = [];

            if (selectListItems == undefined || selectListItems.length == 0 || selectListItems == false) {
                results.push($('<option>', { text: noItemText }));
            } else {
                switch (sorting) {
                    case 0:
                        break;
                    case 1: //text assending
                        selectListItems = selectListItems.sort((a, b) => {
                            let textA = a.Text?.toUpperCase() || '';
                            let textB = b.Text?.toUpperCase() || '';
                            if (textA > textB)
                                return 1;
                            else if (textB > textA)
                                return -1;
                            return 0;
                        });
                        break;
                    case 2:  //text desending
                        selectListItems = selectListItems.sort((a, b) => {
                            let textA = a.Text?.toUpperCase() || '';
                            let textB = b.Text?.toUpperCase() || '';
                            if (textA > textB)
                                return -1;
                            else if (textB > textA)
                                return 1;
                            return 0;
                        });
                        break;
                    case 3: //value assending
                        selectListItems = selectListItems.sort((a, b) => {
                            let valueA = a.Value?.toUpperCase() || '';
                            let valueB = b.Value?.toUpperCase() || '';
                            if (valueA > valueB)
                                return 1;
                            else if (valueB > valueA)
                                return -1;
                            return 0;
                        });
                        break;
                    case 4: //value descending
                        selectListItems = selectListItems.sort((a, b) => {
                            let valueA = a.Value?.toUpperCase() || '';
                            let valueB = b.Value?.toUpperCase() || '';
                            if (valueA > valueB)
                                return -1;
                            else if (valueB > valueA)
                                return 1;
                            return 0;
                        });
                        break;
                    default:
                        throw new NotImplementedException();
                }
                selectListItems.forEach((item) => {
                    if (item.Render === false)
                        return;

                    let option = $('<option>', { value: item.Value, text: item.Text });

                    if (item.SearchTag != undefined) {
                        option.attr('data-select-search', item.SearchTag);
                    }

                    if (item.Attributes != undefined) {

                        let attributes = Object.getOwnPropertyNames(item.Attributes);
                        for (let i = 0; attributes.length > i; i++) {
                            let attribute = attributes[i];
                            let value = item.Attributes[attribute];
                            option.attr(attribute, value);
                        }
                    }
                    if (item.Disabled)
                        option.attr('disabled', true);
                    if (item.Hidden)
                        option.attr('hidden', true);
                    if (item.Tooltip != undefined && item.Tooltip.length)
                        option.attr('title', item.Tooltip)


                    if (item.Group != undefined && item.Group != null) {
                        let group = item.Group;
                        let groupEl = results.find(x => x.attr('label') == group.Name);
                        if (groupEl != undefined && groupEl.length) {
                            groupEl.append(option);
                        } else {
                            groupEl = $('<optgroup>', { label: group.Name });
                            groupEl.append(option);
                            let attributes = Object.getOwnPropertyNames(group.Attributes);
                            for (let i = 0; attributes.length > i; i++) {
                                let attribute = attributes[i];
                                let value = group.Attributes[attribute];
                                option.attr(attribute, value);
                            }
                            if (group.Disabled)
                                groupEl.attr('disabled', true);
                            if (group.Hidden)
                                groupEl.attr('hidden', true);
                            if (group.Tooltip != undefined && group.Tooltip.length)
                                groupEl.attr('title', group.tooltip)
                            results.push(groupEl);
                        }
                    } else {
                        results.push(option);
                    }

                });
            }
            return results;
        }
    },
    select2: {
        //https://select2.org/searching
        searchMethod: function (params, data, ...rest) {

            // If you want to limit the search results, add the data-max-results attribute to the select
            if (params.maxResultCount == undefined) {
                try {
                    let id = $(event.target).attr('aria-controls') || $(event.target).closest('.select2-selection').attr('aria-owns');
                    let select = $(`#${id.split('-')[1]}`);
                    let maxCount = select.attr('data-max-results');
                    if (maxCount != null && maxCount != undefined)
                        params.maxResultCount = Number(maxCount);
                    else
                        params.maxResultCount = -1;
                } catch (error) {
                    console.error(error);
                    params.maxResultCount = -1;
                }
            }
            params.currentResultCount = params.currentResultCount || 0;

            if (params.maxResultCount != undefined && params.maxResultCount > 0) {
                if (params.currentResultCount >= params.maxResultCount)
                    return null;
            }

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
                        if (data.text.length)
                            child.text += `( ${data.text} )`;

                        params.currentResultCount++;
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
                    params.currentResultCount++;
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

        //This is a collection of all the selectlists that do not contain at least one option with these properties.
        //This is used for larger list to prevent unnessesary searching in $query
        selectIdsWithout: {
            prepend: [],
            append: [],
            class: [],
            styles: [],
        },

        resultRenderer: function (state) {
            if (state.element == undefined)
                return;



            let selectId = $(state.element).closest('select').attr('id');


            let result, prepend, append;

            let without = uic.form.select2.selectIdsWithout;

            //A performance check to ensure that we don't search for the prepend if not nessesary.
            if (!without.prepend.includes(selectId)) {
                var prependContainer = $(`.select-elements[for-select="${selectId}"]`);

                if (prependContainer.length) {
                    if (state.children == undefined) {
                        prepend = prependContainer.find(`.prepend-item[for-value="${state.id}"]`).html();
                    }
                    else {
                        prepend = prependContainer.find(`.prepend-group[for-label="${state.text}"]`).html();
                    }
                } else {
                    //If the selectList does not have a prependContainer, add this to the list
                    without.prepend.push(selectId);
                }
            }

            //A performance check to ensure that we don't search for the append if not nessesary.
            if (!without.append.includes(selectId)) {
                var appendContainer = $(`.select-elements[for-select="${selectId}"]`);

                if (appendContainer.length) {
                    if (state.children == undefined) {
                        append = appendContainer.find(`.append-item[for-value="${state.id}"]`).html();
                    }
                    else {
                        append = appendContainer.find(`.append-group[for-label="${state.text}"]`).html();
                    }
                } else {
                    //If the selectList does not have a appendContainer, add this to the list
                    without.append.push(selectId);
                }
            }
            result = $('<span>').append(prepend).append(state.text).append(append);


            if (!without.class.includes(selectId)) {
                if (!$(`#${selectId} option[class]:not([class='uic'])`).length)
                    without.class.push(selectId);

                let existingClass = $(state.element).attr('class');
                result.attr('class', existingClass);
            }
            if (!without.styles.includes(selectId)) {
                if (!$(`#${selectId} option[style]`).length)
                    without.styles.push(selectId);

                let existingStyle = $(state.element).attr('style');
                result.attr('style', existingStyle);
            }


            let existingData = $(state.element).data();
            result.data(existingData);


            return result;
        }
    },
    textarea: {
        addRow: function (textArea) {
            let currentRows = $(textArea).attr('rows') || $(textArea).val().split('\n').length;
            $(textArea).attr('rows', +currentRows + 1);
            this.setRowHeight(textArea);
        },
        setRowHeight: function (textArea) {
            let currentRows = $(textArea).attr('rows') || $(textArea).val().split('\n').length;
            let minRows = $(textArea).attr('min-rows')||1;
            let maxRows = $(textArea).attr('max-rows');

            if (currentRows < minRows)
                currentRows = minRows;
            if (maxRows != undefined && currentRows > maxRows)
                currentRows = maxRows;
            $(textArea).attr('rows', currentRows);
        }
    }
};
$(document).ready(() => {
    $(document).on('click', ev => {
        if ($(ev.target).closest('.tooltip-icon').length)
            return;
        $('.tooltip-icon').popover('hide');
    })
    $(document).on('keydown', 'textarea', (ev) => {
        if (ev.keyCode == '13') {
            uic.form.textarea.addRow(ev.target);
        }
    });
    uic.form.setPopoverOnClickTooltipIcon();
    uic.form.setThreestateToggles($('.three-state-checkbox:not(.configured)'));
});
$(document).ajaxComplete(() => {
    uic.form.setPopoverOnClickTooltipIcon();
    uic.form.setThreestateToggles($('.three-state-checkbox:not(.configured)'));
});
