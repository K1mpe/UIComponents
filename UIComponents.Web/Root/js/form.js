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
            form.find('.input-no-value').addClass('d-none');

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

    delete: async function (controller, id, title = "", message = "", data = {}) {
        try {
            var content = await uic.getpost.get(`/${controller}/Delete`, { id, title, message, data });
            $('body').append(content);

        } catch (ex) {
            ErrorBox(ex);
        }
    }
};