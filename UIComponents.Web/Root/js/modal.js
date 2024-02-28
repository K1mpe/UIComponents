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

        var popup = $(item).closest('.uic-can-hide');
        if (popup.length) {
            window.close();
            return true;
        }

        return false;
    },

};
