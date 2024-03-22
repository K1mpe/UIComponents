# Required
## c#
In program.cs, add the this to the builder.
```c#
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
```c#
@await myComponent.InvokeAsync(Component)
```

Rendering a Task< IUIComponent > is also supported
```c#
@await _uic.CreateComponentAsync(testModel).InvokeAsync(Component)
```
### Rendering components from controller
```c#
public IActionResult RenderMyComponent(){
    var myComponent = ...
    return View("/UIComponents/ComponentViews/Render.cshtml", myComponent);
}
```

# Custom Generators
You can add custom generators in the builder config to add or change the behavior.
Each generator needs a Name, orderNumber and a function.
- Name: Might be usefull when a error occurs or logging
- Order: From low to high, default generators are around 1000.
- Function: gets arguments and the existing result from previous generators
```c#
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

```c#
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
```c#
public class BaseEntity: IDbEntity<long>
{
    public long Id { get; set; }
}

public class BaseEntity : IDbEntity<Guid>
{
    public Guid Guid { get; set; }

    // If the Id of the object is something else then 'Id', you can map it privately like this
    string IDbEntity.Id => Guid;
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
``` Javascript
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

# Available services

## IUIComponentService
This service can auto generate components based on the existing generators.

You can inject the **IUIComponentService** directly in a view, controller or service.
```c#
private readonly IUIComponentService _uic;

TestModel testModel = new TestModel();

var myComponent = await _uic.CreateComponentAsync(testModel);
```

You can change the options of the generators by providing a **UICOptions** object.
```c#
//Create
var componentWithOptions = await _uic.CreateComponentAsync(testModel, new UICOptions()
{
    ExcludedProperties = "Id, IsDeleted",
    ShowEditButton = false,
};

```
To change the default values of these options, these can be configured from the **UIComponents.Defaults.OptionDefaults**.
```c#
UIComponents.Defaults.OptionDefaults.ReverseButtonOrder = true;
```

You can also Inherit the UICOptions object and set default values in the constructor
```c#
public class UICCreateOptions : UICOptions
{
    public UICCreateOptions(){
        ReplaceSaveButtonWithCreateButton = true;
        InputGroupSingleRow = true;
        ShowEditButton = false;
        ShowDeleteButton = false;
        ShowCancelButton = true;
        IdHidden = true;
    }
}
```

You can create a single input field from a object
> :warning: This propertyselector can only be nested 1 level!
```c#
var dateInput = await _uic.CreateComponentAsync(testModel, x => x.Date);
var dateInputWithOptions = await _uic.CreateComponentAsync(testModel, x => x.Date, new(){
    InputGroupSingleRow = false;
});
```


## IUICValidationService
This service is available, and uses all implementations of **IUICPropertyValidationRule**.

If there are multiple implementations are conflicting, the most exact value will be used.
Validation Min value => one returns 0, other returns 10 => 10 will be set
Validation Max value => one returns 20, other returns 50 => 20 will be set

### Usage
This service is automatically used by the **IUIComponentService** to set properties as required, assign minimum and maximum values.
This service can also be used inside a **AbstractValidator< T >**. This will check all the availalble validationrules and handle the messages.


```c#
public class TestModelValidator : AbstractValidator<TestModel>
{
    private readonly IUICValidationService _validationService;
    private readonly IUICLanguageService _languageService;

    public TestModelValidator(IUICValidationService validationService, IUICLanguageService languageService)
    {
        _validationService = validationService;
        _languageService = languageService;

        //This method requires a AbstractValidator (this) and a implementations of the IUICLanguageService
        _validationService.ValidateModel(this, _languageService);
    }
}
```

### Available interfaces for IUICPropertyValidationRules
- IUICPropertyValidationRule< T > : ValidationRule that only works for this PropertyType (example: IUICPropertyValidationRule< int >)
- IUICPropertyValidationRuleRequired : Can check if a property is required
- IUICPropertyValidationRuleMinValue < TValue > : Assign a minimum value to a property
- IUICPropertyValidationRuleMaxValue < TValue > : Assign a maximum value to a property
- IUICPropertyValidationRuleMinLength : Assign a minimum length to a string
- IUICPropertyValidationRuleMaxLength : Assign a maximum length to a string
- IUICPropertyValidationRuleReadonly : Mark a property as readyonly

### Predefined validationRules
To include the predefined validators to the implementation, add the following line the configuration:
```c#
builder.Services.AddUIComponentWeb(config =>
{
    ...
    config.AddDefaultValidators(builder.Services);
});
```

#### UICValidatorRequired : IUICPropertyValidationRuleRequired
Set required if **Required** attribute is set.
Else, if property is not nullable, these check will run:
- Has ForeignKeyAttribute
- Has FakeForeignKeyAttribute with required on
- Check UICInheritAttribute

#### UICValidatorRangeAttribute< T > : IUICPropertyValidationRuleMinValue< T >, IUICPropertyValidationRuleMaxValue< T > where T : struct, IComparable
Check the **RangeAttribute** on the current property or inherit property
This implementation is type specific, and is currently only implemented for:
- short
- int
- long
- float
- double
- decimal
- DateOnly
- DateTime
- TimeOnly
- Timespan

#### UICValidatorEditPermission : IUICPropertyValidationRuleReadonly
Use the edit permission to check if the current user has permission to edit this property.
Also checks the Inherit property (if available).

### Adding custom ValidationRules


#### Createing service on interface
When adding custom validationRules, try using the most exact interface, as described above.
If you use **IUICPropertyValidationRule**, this will be used in server validation, but will not be checked for MinValue requirement.
Only these specific interfaces can check these options.

```c#
public class MyValidationRuleRequired : IUICPropertyValidationRuleRequired
{
    //The type this validation rule is used for, set as typeof(object) if used for any propertyType
    public Type? PropertyType => typeof(object);

    public async Task<bool> IsRequired(PropertyInfo propertyInfo, object obj)
    {
        //This function is used to mark a property with the validationrule when requesting the form
    }
```
If you want to return custom messages or handling of the response, add the **IUICPropertyValidationValidationResultsImplementation** interface

```c#
public class MyValidationRuleRequired : IUICPropertyValidationRuleRequired, IUICPropertyValidationValidationResultsImplementation
{
    //The type this validation rule is used for, set as typeof(object) if used for any propertyType
    public Type? PropertyType => typeof(object);

    public async Task<bool> IsRequired(PropertyInfo propertyInfo, object obj)
    {
        //This function is used to mark a property with the validationrule when requesting the form
    }
    public Task<ValidationRuleResult> CheckValidationErrors(PropertyInfo propertyInfo, object obj)
    {
        ...
    }
```

In the configuration, assign this validator
```c#
//Register this type to be used as a validator. Does not register service!
config.AddValidatorProperty<MyValidationRuleRequired>();

//This method assignes the type as a validator, but also register this service as scoped.
config.AddAndRegisterGenerator<MyValidationRuleRequired>(builder.Services);
```

#### create a validator without a service or interface
You can also create validators without creating a new class that implements the interface.
Use one of the **AddValidatorProperty..** methods in the configuration.
This way DOES NOT have support for dependency injection and always uses the default **DefaultValidationErrors** method.
```c#
config.AddValidatorPropertyRequired((propertyInfo, obj) =>
{
    ...
});
```




## IUICStoredComponents
You can use this interface if you want to send something to a user (F.e. a complex notification or popup).
This service is also used by the **IUICQuestionService**.

### Receiving notification on page load
When loading a page, you can check if there are any remaining notifications for the user, and display these notifications.
```c#
var allNotifications = _uicStoredComponents.GetComponentsByUser(currentUserId);
```
:warning: If a component is stored as single use, this will automatically be removed from the collection.
This means you should never call this method if you do not intent on sending these notifications to the user.
The .InvokeAsync(Component) method does support lists or null to render

## IUICQuestionService
You can use the IUICQuestionService if you want to ask questions to the client before continuing in code.
:warning: The questionservice does require the **IUICSignalRService** service to be implemented by the user!





# Register services by implementer

## IUICLanguageService
To enable translations, Implement the **IUICLanguageService** and make sure this is also registrated as this type.
```c#
builder.Services.AddScoped<IUICLanguageService, LanguageService>();
```
If you do not wish to use this service, disable the check in the builder configuration.

Without a languageService, all Translatables will take the last part of the key as defaultValue
```c#
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
```c#
builder.Services.AddScoped<IUICPermissionService, PermissionService>();
```
If you do not wish to use this service, disable the check in the builder configuration.
Without the permissionservice, all permissionchecks will result in true.
```c#
builder.Services.AddUIComponentWeb(config =>
{
    config.CheckPermissionServiceType = false;
    ...
});
```

## IUICDefaultCheckValidationErrors< IUICPropertyValidationRuleReadonly >
The UIComponents does not have a way to validate if a readonly property is changed.
This means that when trying to validate there will be logged a error that this is not yet available (Error only occurs first time on each build).
After this the error logging, the service will just return that there are no errors.

You can disable this errormessage in the config, or create a implementation of this interface.
```c#
builder.Services.AddUIComponentWeb(config =>
{
    config.CheckPropertyValidatorReadonly = false;
    ...
});
```

## IUICSignalRService
Implement the IUICSignalRService interface in your solution to enable IUICQuestionService. 
This is not required for the UICSignalR model.
```c#
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
```c#
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
```c#
UIComponents.Defaults.OptionDefaults.ReverseButtonOrder = true;
UIComponents.Defaults.ColorDefaults.ButtonSave = new Color("primary");
UIComponents.Defaults.TranslationDefaults.ButtonDelete = new Translatable("Button.Delete");
```


# Attributes
There are several attibutes available that influence the behavior of the generators.

## FakeForeignKey
This attribute can identify a property as foreignKey without influencing the database.
You can also mark this foreignKey as optional
```c#
[FakeForeignKey(typeof(User), false)]
public long UserId { get; set; }
```

## UICIgnoreAttribute
Properties with this attribute are always ignored by the generators and will not be visualised
```c#
[UICIgnoreAttribute]
public bool IsDeleted { get; set; }
```

## UICInheritAttribute
When creating view models, you can use this attribute to make the generators look at the other class attributes.
```c#
[UICInheritAttribute(typeof(User))]
public string EmailAddress { get; set; }
```
If the propertyname does not match the name of the inherite object, you can also give the source name.
```c#
[UICInheritAttribute(typeof(User), nameof(User.Email)]
public string EmailAddress { get; set; }
```

This attribute can also be placed on the class, This will inherit all properties with the same name.
You can also add multiple types. The first type match will have priority.

In this example, the LastName will not be rendered, and the GroupName will have a span text.
```c#
[UICInheritAttribute(typeof(User), typeof(UserGroup)]
public class UserViewModel
{
    public string EmailAddress { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public string GroupName { get; set; }
}
public class User
{
    public string EmailAddress { get; set; }
    public string FirstName { get; set; }

    [UICIgnoreAttribute]
    public string LastName { get; set; }
}
public class UserGroup
{
    [UICSpan('This is the name of the group')]
    public string GroupName { get; set; }
}
```

## UICPropertyTypeAttribute
Normally the generators will automatically detect what type of PropertyType to use, but you can also manually assign this.

:warning: If a property name contains "Color" and is a string input, this will be detected as a color input with a HEX value by default.
```c#
public class TestModel
{
    [UICPropertyType(UICPropertyType.String)]
    public string MySkinColor { get; set; }

    [UICPropertyType(UICPropertyType.MultilineText)]
    public string Description { get; set; }
}
```


## UICSpanAttribute / UICTooltipAttribute
Applying one of this attributes will add a info textbox and both work in a very simular way.

- UICSpanAttribute will generate a spantext under the input
- UICTooltipAttribute will generate a tooltip on the input and label. The label may also get a info icon to indicate there is a tooltip available.


The **first** parameter of this attribute is the **default text**, and the **second** paramater is the resourceKey.
If you do not provide the resourceKey, This key will automatically be generated.
You can overwrite these defaultKeys in the **UIComponents.Defaults.TranslationDefaults** namespace.
```c#
UIComponents.Defaults.TranslationDefaults.DefaultInfoSpanKey = (propInfo, propertyType) => { ... };
UIComponents.Defaults.TranslationDefaults.DefaultTooltipKey = (propInfo, propertyType) => { ... };
```