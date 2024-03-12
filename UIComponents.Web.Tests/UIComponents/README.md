# Required
## C#
In program.cs, add the this to the builder.
```C#
builder.Services.AddUIComponentWeb(config =>
{
    config.AddDefaultGenerators(builder.Services);
});
```

## Javascript
In _scripts.cshtml or the layout page, add the script.
Make sure that all overrides of this javascript code come after the importing of the script
```Javascript
<script src="~/uic/js/uic.js" asp-append-version="true"></script>
```

## scss
```css
@import '../uic/css/uic.scss';
```


# Creating components
Create items manually or use the IUIComponentService to generate components.

### Rendering components from view
```C#
@await myComponent.InvokeAsync(Component)
```
### Rendering components from controller
```C#
public IActionResult RenderMyComponent(){
    var myComponent = ...
    return View('/UIComponents/ComponentViews/Render.cshtml', myComponent);
}
```

# Custom Generators
You can add custom generators in the builder config to add or change the behavior.
Each generator needs a Name, orderNumber and a function.
- Name: Might be usefull when a error occurs or logging
- Order: From low to high, default generators are around 1000.
- Function: gets arguments and the existing result from previous generators
```C#
config.AddGenerator(GeneratorHelper.SelectListItems("DataBase SelectList", 1000, async (args, existing) =>
{
    //If there is a existing result, do not change anything
    if (existing != null)
        return GeneratorHelper.Next<List<SelectListItem>>();

    var selectlistItems = await  GetSelectListItemsAsync()

    //After the function get the required results, return with GeneratorHelper.Success(results, true/false)
    //the true value indicates that the other generators may change this result. False disables this.
    return GeneratorHelper.Success(selectListItems, true);

}));
```

```C#
config.AddGenerator(GeneratorHelper.ObjectGenerator(typeof(IDbEntity), "AddingSignalRRefresh", 2000, async (args, existing) =>
{
    await Task.Delay(0);
    //If no result is available, don't do anything
    if(existing == null)
        return GeneratorHelper.Next();

    if(existing is IUICHasChildren<IUIComponent> canHaveChildren)
    {
        //If the object can have child elements, create a signalR method that will be added to this object
        var signalR = new UICSignalR(nameof(IMainHub.ReceiveEntity), "entity", "type", "dbAction")
        {
            JoinGroups = args.PropertyType.Name,
            Action = new UICActionValidateObject()
            {
                ReferenceObjectName = "entity",
                MatchObject = new { Id = (args.PropertyValue as IDbEntity).Id },
                OnMatch = new UICActionMarkChanges(existing, "entity")
            }
        };
        canHaveChildren.Children.Add(signalR);
        return GeneratorHelper.Success(existing, true);
    }
    return GeneratorHelper.Next();
}));
```
# Database entities (IDbEntity)
Set the IDbEntity on your database classes, so these are recognised as database models and the Id can be mapped
```C#
public class BaseEntity: IDbEntity<long>
{
    public Long Id{get;set;}
}

public class BaseEntity : IDbEntity<string>
{
    public string Guid{get;set;}

    //If the Id of the object is something else then 'Id', you can map it privately like this
    string IDBEntity.Id => Guid;
}
```

# ClientSide responseHandling
If you want to add some clientside responsehandeling (example: success notification or errorbox),
this can be done by adding a handler to **uic.getpost.defaultHandlers**.
There are 3 categories for these handlers:
- get
- post
- both

After getting a response from the server, the client will loop through these methods, until one method returns a value.
If no handlers return a value, the entire response will be returned
```Javascript
<script src="~/uic/js/uic.js" asp-append-version="true"></script>

<script>
uic.getpost.defaultHandlers.push( (response) => {
    if (response.type == "Redirect") {

        if (!response.data.length)
            location.reload();
        else
            location.href = response.url;
        return true;
    }
});
</script>
```



# Register services

## IUICLanguageService
To enable translations, Implement the **IUICLanguageService** and make sure this is also registrated as this type.
```C#
builder.Services.AddScoped<IUICLanguageService, LanguageService>();
```
If you do not wish to use this service, disable the check in the builder configuration.

Without a languageService, all Translatables will take the last part of the key as defaultValue
```C#
builder.Services.AddUIComponentWeb(config =>
{
    config.CheckLanguageServiceType = false;
    ...
});
```
### Clientside translation
To get clientside translations, create a controller method that gets the translated value without formatting the variables, and make this call available from **uic.translation.fetchTranslationText**.

The entire clientside javascript can also be overwritten:
```Javascript
uic.translation = {
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
            cachedValue = await fetchTranslationText(translatable);
            uic.translation._defaultValues[translatable.ResourceKey] = cachedValue;
        }

        //Format the arguments in the text
        return cachedValue.format(translatable.Arguments);

        
    },
    //The function that requests the service to give the translation
    fetchTranslationText: async function (translatable) {
        let defaultValue = translatable.DefaultValue || translatable.ResourceKey.split('.').last();
        return defaultValue.format(translatable.Arguments);
    },

    //Local dictionary that store translation keys with value texts. these values do not have their variables replaced yet.
    //Content of this object remains until the page reloads.
    _defaultValues: {

    }
}
```


## IUICPermissionService
To enable permission checks, Implement the **IUICPermissionService** and make sure this is also registrated as this type.
```C#
builder.Services.AddScoped<IUICPermissionService, PermissionService>();
```
If you do not wish to use this service, disable the check in the builder configuration.
Without the permissionservice, all permissionchecks will result in true.
```C#
builder.Services.AddUIComponentWeb(config =>
{
    config.CheckPermissionServiceType = false;
    ...
});
```

## IUICQuestionService
This interface is implemented and does not need a custom implementation.
You can use the IUICQuestionService if you want to ask questions to the client before continuing in code.
The questionservice does require the IUICSignalRService service to be implemented by the user!

## IUICSignalRService
Implement the IUICSignalRService interface in your solution to enable IUICQuestionService. 
This is not required for the UICSignalR model.
```C#
public class SignalRService : IUICSignalRService
{
    #region Ctor
    public SignalRService(MainHub signalRHub)
    {
        SignalRHub = signalRHub;
    }

    #endregion

    #region Properties

    public MainHub SignalRHub { get; set; }

    #endregion

    public async Task RemoveUIComponentWithId(string id)
    {
        await SignalRHub.Clients.All.RemoveUIComponentWithId(id);
    }

    public async Task SendUIComponentToUser(FetchComponent fetchComponent, string userId)
    {
        await SignalRHub.Clients.All.SendUIComponentToUser(fetchComponent, userId);
    }
}
```
After implementing the implementation of the scripts, you also need to assign the userId clientSide.
```C#
<script src="~/uic/js/uic.js" asp-append-version="true"></script>
<script>
    uic.signalR.currentUserId = @UserId;
</script>
```

If you want to use a diffrent implementation of this interface than this example, you also need to change the javascript methods that receive these events.
```Javascript
uic.signalR = {
    handleUIComponentFetch: async ()=>{
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
    currentUserId: undefined
}
```

# Option defaults
If you want to set some default values, you can access them in the UIComponents.Defaults namespace.
It is recommended to only change these in the builder config to not change the behavior while the program is running.
```C#
UIComponents.Defaults.OptionDefaults.ReverseButtonOrder = true;
UIComponents.Defaults.ColorDefaults.ButtonSave = new Color("primary");
UIComponents.Defaults.TranslationDefaults.ButtonDelete = new Translatable("Button.Delete");
```