uic.signalR = {
    handleUIComponentFetch: async () => {
        await window.connection.on('SendUIComponentToUser', async (fetchComponent, userId) => {
            if (uic.signalR.currentUserId == undefined) {
                console.error("uic.signalR.currentUserId is not defined!")
                return;
            }

            if (uic.signalR.currentUserId != userId)
                return;

            let appendTo = $(fetchComponent.AppendTo);
            if (!appendTo.length)
                return;

            let result = await uic.getpost.get('/uic/getComponent', { key: fetchComponent.ComponentKey });
            appendTo.append(result);
        });
    },
    handleUIComponentRemove: async () => {
        await window.connection.on('RemoveUIComponentWithId', async (id) => {
            $(`#${id}`).trigger('uic-remove');
        });
    },
    findSignalR: (element) => {
        let signalRElements = $(element).find('.uic-signalR');
        signalRElements.each((index, item) => {
            $(item).trigger('uic-find-signalR');
        });
    },
    debug: false,

    currentUserId: undefined
}
$(document).ready(() => {
    //Wait a small delay so the connection can exist
    setTimeout(() => {
        uic.signalR.handleUIComponentFetch();
        uic.signalR.handleUIComponentRemove();
    }, 5);
});