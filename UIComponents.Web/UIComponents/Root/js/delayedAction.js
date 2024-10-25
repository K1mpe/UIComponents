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
    
    //This function will execute the first time and block following requests.
    //When the timer runs out and there are requests blocked, the function is called again
    throttle: function (key, action, timeout = 1000) {
        if (uic.delayedAction._throttle[key] === false) {
            uic.delayedAction._throttle[key] = true;
            return;
        }
        else if (uic.delayedAction._throttle[key] === true)
            return;

        action();
        uic.delayedAction._throttle[key] = false;

        setTimeout(() => {
            if (uic.delayedAction._throttle[key] === true)
                action();
            delete uic.delayedAction._throttle[key];
        }, timeout);
    },

    //Execute the function after a period of inactivity
    debounce: function (key, action, timeout = 1000) {
        let existing = uic.delayedAction._debounce[key];

        if (uic.delayedAction._debounce[key] != undefined && uic.delayedAction._debounce[key] != null) {
            clearTimeout(uic.delayedAction._debounce[key]);
        }
        
        uic.delayedAction._debounce[key] = setTimeout(() => {
            action();
            uic.delayedAction._debounce[key] = null;
        }, timeout);
    },

    //This function waits and triggers once after the timeout expires.
    //Multiple requests are ignored.
    delayed: function (key, action, timeout = 1000) {
        if (uic.delayedAction._delayed[key] !== undefined) {
            return;
        }

        uic.delayedAction._delayed[key] = setTimeout(() => {
            action();
            delete uic.delayedAction._delayed[key];
        }, timeout);
    },
    _running: {},
    _throttle: {},
    _debounce: {},
    _delayed: {}
    
};
