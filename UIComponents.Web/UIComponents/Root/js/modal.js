uic.modal = uic.modal || {
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

};
