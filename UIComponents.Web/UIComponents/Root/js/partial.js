uic.partial = uic.partial || {
    help: function (id) {
        console.log($(`#${id}`));
        console.log(`$('#${id}').trigger('uic-reload') => reload the content of this card`);
        console.log(`$('#${id}').trigger('uic-forceReload') => Always reload this partial. (Not recommended!)`);
        console.log(`$('#${id}').trigger('uic-disableReload') => Disable reloading the partial`);
        console.log(`$('#${id}').trigger('uic-enableReload') => Enable reloading the partial. Will also trigger reload if a reload has previously been blocked`);
        console.log(`$('#${id}').trigger('uic-addReloadCondition', ()=> {...}) => Add a new function that is checked on each reload. Returning false will disable the reload`);
        console.log(`$('#${id}').trigger('uic-reloadConditionChanged') => One or more of the reload conditions have changed.`);
        console.log(`$('#${id}').triggerHandler('uic-source') => returns the controller and action of this request`);
        console.log(`$('#${id}').on('uic-before-reload', () => {...}) => triggered before the reload starts`);
        console.log(`$('#${id}').on('uic-reloaded', () => {...}) => triggered after the reload has finished`);
    },

    addReloadCondition: function (partialId, condition) {
        uic.partial._partialData[partialId].reloadConditions.push(condition);
    },

    //Check if this partial Id is able to reload
    canReload: async function (partialId) {
        let data = uic.partial._partialData[partialId];
        if (!data.canReload)
            return false;
        for (let i = 0; data.reloadConditions.length > i; i++) {
            let condition = data.reloadConditions[i];
            let result = await condition();
            if (!result)
                return false;
        }
        return true;
    },

    hideLoadingOverlay: function (element = null) {
        if (!element)
            element = document.body;

        $(element).LoadingOverlay('hide');
    },

    showLoadingOverlay: function (element = null) {
        if (!element)
            element = document.body;

        $(element).LoadingOverlay('show', {
            image: '',
            fontawesome: 'fas fa-sync-alt fa-spin'
        });
    },

    handlers: [
        (response) => {
            if (response.type == "AccessDenied") {
                return $('<div>', {
                    class: 'alert alert-danger',
                    role: 'alert'
                }).append('Access Denied');
            }
        },
        (response) => {
            if (response.type == "Exception") {
                return $('<div>', {
                    class: 'alert alert-danger',
                    role: 'alert'
                }).append('Error receiving data');
            }
        },
    ],

    reloadPartial: async function (partial) {
        let id = partial.attr('id');
        if (await uic.partial.canReload(id))
            partial.trigger('uic-forceReload');
        else
            uic.partial._partialData[id].tryReload = true;
    },

    _init: function (partial, initialLoad, reloadDelay) {
        let id = partial.attr('id');
        if (!id.length)
            throw `partial has no Id`;

        uic.partial._partialData[id] = {
            tryReload: initialLoad,
            canReload: true,
            reloadConditions: []
        }
        partial.on('uic-reload', (ev) => {
            ev.stopPropagation();
            if (reloadDelay > 0) {
                uic.delayedAction.run(`partial-Reload-${id}`, reloadDelay, uic.partial.reloadPartial(partial));
                return;
            }
            uic.partial.reloadPartial(partial);
        });

        partial.on('uic-disableReload', (ev) => {
            ev.stopPropagation();
            uic.partial._partialData[id].canReload = false;
        });
        partial.on('uic-enableReload', (ev) => {
            ev.stopPropagation();
            uic.partial._partialData[id].canReload = true;
            partial.trigger('uic-reloadConditionChanged');
        });
        partial.on('uic-addReloadCondition', (ev, condition) => {
            ev.stopPropagation();
            uic.partial.addReloadCondition(id, condition);
        })
        partial.on('uic-reloadConditionChanged', (ev) => {
            if (uic.partial._partialData[id].tryReload) {
                partial.trigger('uic-reload');
            }
        })
    },
 
    _partialData: {},
    _reloadPartial: async function (partial, showOverlay, getDatafunc) {
        if (!partial.length)
            return;

        await partial.triggerHandler('uic-before-reload');
        if (showOverlay)
            uic.partial.showLoadingOverlay(partial);

        let result = await getDatafunc();

        //Remove the Select2 container when reloading the partial containing the select source
        let select2Container = $('.select2-container');
        if (select2Container.length) {
            let forId = select2Container.attr('data-for');
            if (partial.find(`#${forId}`).length)
                select2Container.remove();
        }
        if (result == 'null')
            result = null;

        partial.html(result);

        if (showOverlay)
            uic.partial.hideLoadingOverlay(partial);

        await partial.triggerHandler('uic-reloaded');
    },
};
$(document).ready(() => {
    $('body').on('uic-reload', (ev) => {
        location.reload();
    });
});
