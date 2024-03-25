uic.form = uic.form || {
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
