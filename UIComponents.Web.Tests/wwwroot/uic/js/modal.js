var uic = uic || {};

uic.modal = uic.modal || {};

uic.modal.help = function () {
    console.log(".trigger('uic-hide') => triggers the modal to hide");
    console.log(".on('uic-before-hide', function()) => runs before the modal can hide, returning false will disable de modal to hide");
    console.log(".on('uic-hidden', function()) => triggered after the modal has hidden.");
}




uic.modal.closeParent = function(element){
    var modal = $(item).closest('.modal');
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
}

$(document).ready(function () {
    $(document).on('uic-help', '.uic.modal', function () {
        uic.modal.help();
    });

    $(document).on('uic-hide', '.uic.modal', async function () {
        let beforeHideResult = await $(this).on('uic-before-hide');
        if (beforeHideResult === false)
            return;

        $(this).modal('hide');
        $(this).trigger('uic-hidden');
    })
});

