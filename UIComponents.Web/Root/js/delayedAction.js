uic.delayedAction = uic.delayedAction || {


//delayedAction.Run("MyUniqueKey", 500, (data) => {data.forEach((item) => console.log(item))})
//This method will be run after the delay has run out. If the same key is used before the previous action has completed, this previous action is canceled.
//key => This value needs to be unique, as this is the identifier to reset the timer
//delayMiliseconds => The delay in miliseconds before the callback is triggered
//callback => a function that can take the list of all data that has been collected for this function
// data[0] is always the newest data
// hint: data.length will show how many times this function has been reset before executing
//(optional) data => the data that is send with the function
    run: function (key, delayMiliseconds, callback, ...data) {
        let existing = uic.delayedAction._running[key];

        if (existing == undefined || existing == null) {
            existing = {
                timer: null,
                data: []
            };
        } else {
            clearTimeout(existing.timer);
        }
        if (data.length == 0)
            existing.data.push(null);
        else if (data.length == 1)
            existing.data.push(data[0]);
        else
            existing.data.push(data);

        existing.timer = setTimeout(() => {
            callback(existing.data.reverse());
            uic.delayedAction._running[key] = null;
        }, delayMiliseconds);

        uic.delayedAction._running[key] = existing;
    },
    _running : {},
};
