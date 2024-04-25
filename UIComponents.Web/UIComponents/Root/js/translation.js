uic.translation = uic.translation || {
    translate: async function (translatable) {
        //If the input has to resourceKey, inputting strings will just return the string
        if (translatable.ResourceKey == undefined)
            return translatable;

        //If the key is untranslatedKey, return the first argument without translations
        if (translatable.ResourceKey == "UntranslatedKey")
            return translatable.Arguments[0];

        //Check if the translation is already requested, call the fetchTranslationText on first request
        let cachedValue = uic.translation._defaultValues[translatable.ResourceKey];
        if (cachedValue == undefined) {
            cachedValue = await uic.translation.translateKey(translatable.ResourceKey);
            uic.translation._defaultValues[translatable.ResourceKey] = cachedValue;
        }

        //Format the arguments in the text
        return (cachedValue||translatable.DefaultValue).format(translatable.Arguments);
    },
    //Get the translated defaultvalue for this key
    translateKey: async function (key) {
        return await uic.translation.translate({ ResourceKey: key });
    },
    //The function that requests the service to give the translation
    fetchTranslationText: async function (translatable) {
        let defaultValue = translatable.DefaultValue || translatable.ResourceKey.split('.').slice(-1)[0];
        return defaultValue.format(translatable.Arguments);
    },

    //Local dictionary that store translation keys with value texts. these values do not have their variables replaced yet.
    //Content of this object remains until the page reloads.
    _defaultValues: {

    }
};