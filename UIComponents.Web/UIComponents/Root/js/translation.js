uic.translation = uic.translation || {
    translate: async function (translatable) {
        if (translatable.ResourceKey == undefined)
            return translatable;

        let defaultValue = translatable.DefaultValue || translatable.ResourceKey.split('.').last();
        return defaultValue.format(translatable.Arguments);
    }
}