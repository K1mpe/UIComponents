uic.signalR = {
    
    //Join a group if you have not joined already
    joinGroupAsync: async function(groupName){
        if(groupName == null || groupName.length == 0)
            return;
         // Only join the group when this name does not yet exist in the groups object
        if (uic.signalR._joinedGroups.hasOwnProperty(groupName) && uic.signalR._joinedGroups[groupName] > 0) {
            uic.signalR._joinedGroups[groupName]++;
        }
        else {
            await connection.invoke("Join", groupName);
            uic.signalR._joinedGroups[groupName] = 1;
        }
    },
    //Leave a group if there are no remaining subscriptions
    leaveGroupAsync: async function(){
        if(groupName == null || groupName.length == 0)
            return;
        if (!uic.signalR._joinedGroups.hasOwnProperty(groupName))
            return;

        uic.signalR._joinedGroups[groupName]--;

        // Still some other components in this group
        if (uic.signalR._joinedGroups[groupName] !== 0)
            return;

        await connection.invoke("Leave", groupName);
    },
    //Method is called when connection with signalR or when already connected
    whenConnected: function (method){
        if(window["connection"] != undefined){
            if(connection["q"] === "Connected"){
                method();
            }
            connection.on('connected', ()=>{
                method();
            })
            connection.on('reconnected', ()=>{
                method();
            })
        }
        else{
            setTimeout(()=>{
                uic.signalR.whenConnected(method);
            },1);
        }
        
    },
    handleUIComponentFetch: async () => {
        uic.signalR.whenConnected(()=>{
            window.connection.on('SendUIComponentToUser', async (fetchComponent, userId) => {
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
        })
       
    },
    handleUIComponentRemove: async () => {
        uic.signalR.whenConnected(()=>{
            window.connection.on('RemoveUIComponentWithId', async (id) => {
                $(`#${id}`).trigger('uic-remove');
            });
        });
    },
    findSignalR: (element) => {
        let signalRElements = $(element).find('.uic-signalR');
        signalRElements.each((index, item) => {
            $(item).trigger('uic-find-signalR');
        });
    },
    debug: false,
    color: function(conditionMatch){
        let style = window.getComputedStyle(document.body);
        if(conditionMatch){
            return style.getPropertyValue('--success') || '#00FF00';
        } else{
            return style.getPropertyValue('--default') || '#0000FF';
        }
    },
    currentUserId: undefined,
    _joinedGroups:{},
}
$(document).ready(() => {
    //Wait a small delay so the connection can exist
    setTimeout(() => {
        uic.signalR.handleUIComponentFetch();
        uic.signalR.handleUIComponentRemove();
    }, 5);
});