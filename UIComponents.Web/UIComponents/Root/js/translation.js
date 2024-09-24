uic.translation = uic.translation || {
    //Takes a single Translatable to translate
    //translatable.ResourceKey,
    //translatable.DefaultValue,
    //translatable.Arguments
    translate: async function (translatable) {

        //If the input has no resourceKey, inputting strings will just return the string
        if (translatable.ResourceKey == undefined)
            return translatable;

        //If the key is untranslatedKey, return the first argument without translations
        if (translatable.ResourceKey == "UntranslatedKey")
            return translatable.Arguments[0];

        //Check if the translation is already requested, call the fetchTranslationText on first request
        let cachedValue = sessionStorage.getItem(uic.translation._cacheKey(translatable.ResourceKey));
        if (cachedValue == undefined) {
            cachedValue = (await uic.translation.fetchTranslationTexts([translatable]))[translatable.ResourceKey];

            sessionStorage.setItem(uic.translation._cacheKey(translatable.ResourceKey), cachedValue);
        }

        //Format the arguments in the text
        return (cachedValue || translatable.DefaultValue).format(translatable.Arguments);
    },

    //returns a object with Key / Values
    //This method can also be used to cache all required keys so the uic.translation.translate function does not require to fetch again
    translateMany: async function (translatables) {
        let missingTranslations = [];
        for (let i = 0; i < translatables.length; i++) {
            let translatable = translatables[i];
            if (translatable.ResourceKey == undefined)
                continue;

            //If the key is untranslatedKey, return the first argument without translations
            if (translatable.ResourceKey == "UntranslatedKey")
                continue;

            //Don't fetch the same key twice
            if (missingTranslations.filter(x => x.ResourceKey == translatable.ResourceKey).length)
                continue;

            //Check if the translation is already requested, call the fetchTranslationText on first request
            let cachedValue = sessionStorage.getItem(uic.translation._cacheKey(translatable.ResourceKey));
            if (cachedValue == null) {
                missingTranslations.push(translatable);
            }
        }
        if (missingTranslations.length) {
            let translatedValues = await uic.translation.fetchTranslationTexts(missingTranslations);
            for (let i = 0; i < missingTranslations.length; i++) {
                let key = missingTranslations[i].ResourceKey;
                let value = translatedValues[key];
                if (value == undefined)
                    value = missingTranslations[i].DefaultValue || key.split('.').slice(-1)[0];

                sessionStorage.setItem(uic.translation._cacheKey(key), value);
            }
        }
        

        //After caching the translations, run the translate method to format the arguments
        let results = {};
        for (let i = 0; i < translatables.length; i++) {
            let translatable = translatables[i];
            results[translatable.ResourceKey||`${i}`] = await uic.translation.translate(translatable);
        }
        return results;

    },
    //The function that requests the service to give the translations
    //This takes a array of translatables and should return a key / value dictionary.
    //The translated values should not have the arguments filled in yet! this should happen clienside
    fetchTranslationTexts: async function (translatables) {
        //This default implementation just takes the default value or last part of the key, and formats it with the arguments.
        //For real implementation this function should call a controller function.

        console.warn('requesting translations from uic.translation.fetchTranslationTexts without implementation! \r\n Returning a placeholder result until implemented correctly');

        let results = {};
        for (let i = 0; i < translatables.length; i++) {
            let translatable = translatables[i];
            let defaultValue = translatable.DefaultValue || translatable.ResourceKey.split('.').slice(-1)[0];
            results[translatable.ResourceKey] = defaultValue;
        }
        return results;
    },
    clearCache: function () {
        let stored = Object.getOwnPropertyNames(sessionStorage);
        for (let i = 0; i < stored.length; i++) {
            let x = stored[i];
            if (x.startsWith('uic.Translations.')) {
                sessionStorage.removeItem(x);
            }
        }
    },
    //The key that is used in the sessionStorage
    _cacheKey: function (resourceKey) {
        return `uic.Translations.${language}.${resourceKey}`
    }
};