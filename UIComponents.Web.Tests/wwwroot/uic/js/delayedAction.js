uic.delayedAction = uic.delayedAction || {

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

    _throttle: {},
    _debounce: {},
    _delayed: {}
    
};
