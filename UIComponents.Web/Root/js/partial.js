var uic = uic || {};

uic.partial = uic.partial || {
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
};
$('body').on('uic-reload', (ev) => {
    location.reload();
});