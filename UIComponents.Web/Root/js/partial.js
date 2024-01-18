var uic = uic || {};

uic.partial = uic.partial || {
    reload: function (element) {
        let parent = $(element).closest('.partial-source');
        if (parent.length)
            parent.trigger('uic-reload');
        else
            location.reload();
    },
    showOverlay: function (element = null) {
        if (!element)
            element = document.body;

        $(element).LoadingOverlay('show', { image: '', fontawesome: 'fas fa-sync-alt fa-spin' });
    },

    hideOverlay: function (element = null) {
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
            showOverlay(partial);

        let result = await getDatafunc();

        partial.html(result);

        if (showOverlay)
            hideOverlay(partial);

        await partial.triggerHandler('uic-reloaded');
    }
};