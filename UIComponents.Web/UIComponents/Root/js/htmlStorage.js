uic.htmlStorage = uic.htmlStorage || {

    //Initiate a id with a method on how to get the data and how to validate the timestamp.
    //This must be done before any other methods can be used
    addToCollection: function (id, getValueFunc, getTimestampFunc) {
        if (uic.htmlStorage._collection[id] != undefined)
            return;

        uic.htmlStorage._collection[id] = {
            getValueFunc: getValueFunc,
            getTimestampFunc: getTimestampFunc
        };
        let timestampFromStorage = localStorage.getItem(`uic.htmlStorage.${id}.timestamp`);
        if (timestampFromStorage != null) {
            uic.htmlStorage._collection[id].timestamp = timestampFromStorage;
            uic.htmlStorage._collection[id].value = localStorage.getItem(`uic.htmlStorage.${id}.value`);
        }
    },

    // This is the recommended method to use and handles all validation and storing data.
    // Validate if the timestamp is valid (Only first time per page load),
    // If valid, get the current value from storage.
    // If invalid, get the new value, place in storage and update timestamp and then return the new value.
    getValueAsync: async function (id) {
        if (!id.length) {
            throw "Id is required!";
        }
        if (uic.htmlStorage._collection[id] == undefined) {
            throw `${id} does not exist!`;
        }

        if (uic.htmlStorage._busyRequests[id] != undefined) {
            await uic.htmlStorage._busyRequests[id];
        }
        let unblockBusy;
        uic.htmlStorage._busyRequests[id] = new Promise((resolve) => {
            unblockBusy = resolve;
        });
        try {

            let timestamp = uic.htmlStorage._collection[id].timestamp;
            let getTimestampFunc = uic.htmlStorage._collection[id].getTimestampFunc;
            let value = uic.htmlStorage._collection[id].value;
            let getValueFunc = uic.htmlStorage._collection[id].getValueFunc;
            let timestampValidated = uic.htmlStorage._collection[id].timestampValidated || false;

            if (!timestampValidated) {
                let validTimestamp = await getTimestampFunc();
                if (typeof validTimestamp !== 'string') {
                    validTimestamp = JSON.stringify(validTimestamp);
                }
                if (validTimestamp == timestamp) {
                    timestampValidated = true;
                    uic.htmlStorage._collection[id].timestampValidated = true;
                }
                timestamp = validTimestamp;
            }
            if (value == undefined || !timestampValidated) {
                await uic.htmlStorage._loadDataAsync(id, getValueFunc, timestamp);
                value = uic.htmlStorage._collection[id].value;
                uic.htmlStorage._collection[id].timestampValidated = true;
            }
            return uic.htmlStorage._collection[id].value;
        }
        finally {
            unblockBusy();
            uic.htmlStorage._busyRequests[id] = undefined;
        }

    },


    //Get the current value stored. this value may be outdated or empty
    getValueNoValidation: function (id) {
        if (!id.length) {
            throw "Id is required!";
        }
        if (uic.htmlStorage._collection[id] == undefined) {
            throw `${id} does not exist!`;
        }
        return uic.htmlStorage._collection[id].value || '';
    },

    // returns true or false if the current timestamp is valid. This does not update the value if invalid
    isValid: async function (id) {
        if (!id.length) {
            return false;
        }
        if (uic.htmlStorage._collection[id] == undefined) {
            return false;
        }

        let timestamp = uic.htmlStorage._collection[id].timestamp;
        let getTimestampFunc = uic.htmlStorage._collection[id].getTimestampFunc;
        let timestampValidated = uic.htmlStorage._collection[id].timestampValidated || false;
        if (timestampValidated)
            return true;

        let validTimestamp = await getTimestampFunc();
        if (typeof validTimestamp !== 'string') {
            validTimestamp = JSON.stringify(validTimestamp);
        }
        if (validTimestamp == timestamp) {
            timestampValidated = true;
            uic.htmlStorage._collection[id].timestampValidated = true;
            return true;
        }
        return false;
    },





    //When the value is changed, this function will be called.
    onValueChanged: function (id, callBack) {
        let onValChangedFuncs = uic.htmlStorage._onValueChangedFuncs[id] || [];
        onValChangedFuncs.push(callBack);
        uic.htmlStorage._onValueChangedFuncs[id] = onValChangedFuncs;
    },
    onInitOrValueChanged: async function (id, callback) {
        await callback();
        uic.htmlStorage.onValueChanged(id, callback);
    },

    // This will expire the current value, forcing the next GetValueAsync to make a new database request 
    triggerExpired: function (id) {
        if (uic.htmlStorage._collection[id] != undefined) {
            uic.htmlStorage._collection[id].value = undefined;
            uic.htmlStorage._collection[id].timestamp = undefined;
            uic.htmlStorage._collection[id].timestampValidated = false;
        }
        uic.htmlStorage._triggerValueChanged(id);
    },

    _collection: {},
    _onValueChangedFuncs: {},
    _busyRequests: {},

    //Run the database request to load the data
    _loadDataAsync: async function (id, getValueFunc, newTimestamp) {
        let result = await getValueFunc();
        if (!!result) {
            if (typeof result !== 'string') {
                result = JSON.stringify(result);
            }
            uic.htmlStorage._addValueToStorage(id, result, newTimestamp)
            uic.htmlStorage._triggerValueChanged(id);
        }
    },

    //Set the current value in local storage
    _addValueToStorage: function (id, result, newTimestamp) {
        localStorage.setItem(`uic.htmlStorage.${id}.value`, result);
        localStorage.setItem(`uic.htmlStorage.${id}.timestamp`, newTimestamp);
        uic.htmlStorage._collection[id].value = localStorage.getItem(`uic.htmlStorage.${id}.value`);
    },

    //trigger all events subscribed on the 'onValueChanged'
    _triggerValueChanged: function (id) {
        let funcs = uic.htmlStorage._onValueChangedFuncs[id] || [];
        for (let i = 0; i < funcs.length; i++) {
            funcs[i]();
        }
    },
}