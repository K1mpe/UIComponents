uic.modal = uic.modal || {
    help: function(id){
        console.log($(`#${id}`));
        console.log(`$('#${id}').trigger('uic-open') => Open this modal`);
        console.log(`$('#${id}').trigger('uic-close') => Close this modal`);
        console.log(`$('#${id}').trigger('uic-destroy') => Destroy the modal and remove the html`);
        console.log(`$('#${id}').on('uic-showing') => Triggered when the modal is opening`);
        console.log(`$('#${id}').on('uic-opened') => Triggered after the modal has finished opening`);
        console.log(`$('#${id}').on('uic-beforeClose') => Triggered before the modal closes. This can be awaited.`);
        console.log(`$('#${id}').on('uic-closed') => Triggered after the modal has been closed`);
    },
    closeParent: function (item) {
        var modal = $(item).closest('.uic.modal');
        if (modal.length) {
            modal.trigger('uic-close');
            return true;
        }
        modal = $(item).closest('.modal');
        if (modal.length) {
            modal.modal('hide');
            return true;
        }

        var hideable = $(item).closest('.uic-can-close');
        if (hideable.length) {
            hideable.trigger('uic-close');
            return true;
        }

        return false;
    },
    moveModal: function (modal, referenceId) {
        $(uic.modal.modalDestination).append(modal);

        function removeModalIfReferenceIsGone() {
            if (!$(`#${referenceId}`).length)
                modal.remove();
        }
        if ($(`#${referenceId}`).length) {
            $(document).on('uic-reloaded', removeModalIfReferenceIsGone);
            $(document).on('uic-closed', removeModalIfReferenceIsGone);
        }
    },
    modalDestination: 'body',
    _init: function(modal){
        modal.on('uic-open', (ev)=>{
            ev.stopPropagation();
            modal.modal('show');
            modal.trigger('uic-showing');
        });
        modal.on('uic-close', async (ev)=>{
            ev.stopPropagation();
            if(uic.elementContainsEvent(modal, 'uic-beforeClose')){
                await modal.triggerHandler('uic-beforeClose');
            }
            modal.modal('hide');
            $(`#${modal.attr('id') + "scripts"}`).next('.modal-backdrop').removeClass('show');
            $('body').removeClass('modal-open');
        });
        modal.on('uic-destroy', (ev)=>{
            let id = modal.attr('id');
            ev.stopPropagation();
            modal.trigger('uic-close');
            modal.modal('dispose');
            modal.remove();

            setTimeout(() => {
                $(`#${id + "scripts"}`).next('.modal-backdrop').remove();
                $(`#${id + "scripts"}`).remove();
            }, 1000);
            
        });
        modal.on('hidden.bs.modal', (ev)=>{
            modal.trigger('uic-closed');
        });
        modal.on('shown.bs.modal', (ev)=>{
            modal.trigger('uic-opened');
        });
    },
};
