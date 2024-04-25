uic.changeWatcher = uic.changeWatcher || {
    example: function () {
        console.log('the changeWatcher uses the uic.getValue() method to check for changes. This means the selector can by any type of jquery selector, (input, select, div, form, ...)')
        console.log('You need to create the watcher before any changes occur');
        console.log('let watcher = uic.changeWatcher.create($($0)); => Watcher that only watches one element');
        console.log('if(watcher.isChanged()){');
        console.log('...');
        console.log('}');
        console.log('');
        console.log('When SignalR updatesthe page, you can redefine the initial values of the watcher:');
        console.log('uic.changeWatcher.setInitialValues($($0))');
    },

    //Create a new changewatcher
    create: function (...$selectors) {

        let watcher = {
            elements: {},
            //Method that gets all the changes and return them as a array
            getChanges: function () {
                let changes = [];
                let elNames = Object.getOwnPropertyNames(this.elements);
                elNames.forEach((elName) => {
                    let currentVal = uic.getValue(elName);
                    if (!uic.compareObjects(currentVal, this.elements[elName]))
                        changes.push({
                            element: elName,
                            name: $(elName).attr('name'),
                            initialValue: this.elements[elName],
                            currentValue: currentVal
                        });
                })
                return changes;
            },
            //Method that checks if the properties are changed, stops after first change.
            isChanged: function () {
                let elNames = Object.getOwnPropertyNames(this.elements);
                for (let i = 0; i < elNames.length; i++)
                {
                    let elName = elNames[i];
                    let currentVal = uic.getValue(elName);
                    if (!uic.compareObjects(currentVal, this.elements[elName]))
                        return true;
                }
                return false;
            }
        }
        uic.changeWatcher.setInitialValues(watcher, $selectors);
        return watcher;
    },

    //Provide this functino with a watcher and one or more selectors (array).
    //The remembered values of these selectors is than stored as the new InitialValue. (Can be triggered by SignalRChange).
    setInitialValues(watcher, $selectorsArray) {

        if (!Array.isArray($selectorsArray))
            $selectorsArray = [$selectorsArray];

        $selectorsArray.forEach(($selector) => {
            $selector.each((index, domEl) => {
                let selector = `#${$(domEl).attr('id')}`;
                if (selector == '#')
                    selector = domEl;
                let value = uic.getValue(selector);

                watcher.elements[selector] = value;
            })
        });
    },

    
};
