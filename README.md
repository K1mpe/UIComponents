# [https://github.com/K1mpe/UIComponents.git](https://github.com/K1mpe/UIComponents.git)


# UICComponents Documentation

## Quick reference for AI agents / LLMs

This library lets you build server-rendered ASP.NET Core UI out of small, composable C# model objects (`IUIComponent`) instead of hand-writing Razor markup. Two ways to produce a component tree:
1. **Manual** — `new UICCard(...)`, `.Add(...)`, wire `IUICAction`s (`OnClick`, `OnSuccess`, ...) onto it, then render with `@await component.InvokeAsync(Component)`.
2. **Generated** — pass a plain C# model/viewmodel to `IUIComponentGenerator.CreateComponentAsync(model, options?)` and let reflection + attributes build the form for you (see [How component generation works](#how-component-generation-works)).

Every concrete model type name is prefixed `UIC...` and lives in `UIComponents.Models.Models.*` (see [ViewImports](#viewimports) for the full using-list). Interfaces/attributes/enums/base data types live in `UIComponents.Abstractions.*`. The generator/validator pipeline lives in `UIComponents.Generators.*`. ASP.NET Core wiring (DI registration, controllers, taghelpers) lives in `UIComponents.Web.*`.

| I want to... | Go to |
|---|---|
| Auto-generate a form/detail view from a C# class | [IUIComponentGenerator](#iuicomponentgenerator), [How component generation works](#how-component-generation-works) |
| Build UI by hand | [Creating components](#creating-components), [Card](#card), [Buttons](#buttons), [Inputs](#inputs) |
| Run client-side/server-side logic on click/submit/etc | [Actions](#actions) |
| Show/edit a scalar or list property | [Inputs](#inputs) |
| Show a data grid (sortable/filterable/editable) | [Tables](#tables) |
| Let a user browse/upload/download files | [FileExplorer](#fileexplorer), [UICUpload](#uicupload) |
| Show a menu, right-click menu, or nav dropdown | [Dropdown](#dropdown), [ContextMenu](#contextmenu) |
| Show a hierarchical tree (ajax-loaded or static) | [Tree](#tree) |
| Plot a time series | [Graphs](#graphs) |
| Push a toast/notification/live update to a connected user | [Notifications & Realtime](#notifications--realtime) |
| Block server code until the user answers a question | [Questions](#questions) |
| Cache/share one ajax data source across multiple inputs | [Data Sources & Caching](#data-sources--caching) |
| Translate a string for the current user | [Translatable](#translatable) |
| Validate a property (required/min/max/custom) | [IUICValidationService](#iuicvalidationservice), [Attributes](#attributes) |
| Control how a property is rendered without writing a custom generator | [Attributes](#attributes) |
| Add a brand-new generator/validator/button | [Custom Generators](#custom-generators), [How component generation works](#how-component-generation-works) |
| Find a component inside another by type or property name | [Find... methods](#find-methods) |
| Model a schedule/recurrence rule | [DataTypes](#datatypes) (`RecurringDate`) |

> :bulb: Class names are unique and greppable — search this file for the exact `UICXxx` class name if you already know it. Every class has its own heading matching its name.

- [Implementation](#implementation)
- [Creating Components](#creating-components)
- [ClientSide methods](#clientside-methods)
- [Component Models](#component-models)
  <details>
    <summary>Extension Methods</summary>

    - [Add Methods](#add-methods)
    - [Find... Methods](#find-methods)
    - [Get or Set Attributes](#get-or-set-attributes)

  </details>
  <details>
    <summary>Interfaces</summary>

    - [IUIComponent](#iuicomponent)
    - [IUICAction](#iuicaction)
    - [UIComponentViewModel](#uicomponentviewmodel)

  </details>
  <details>
    <summary>Models</summary>

    - [UICCustom](#uiccustom)
    - [Translatable](#translatable)
    - [IUIComponentViewMode](#iuicomponentviewmodel)
    - [Actions](#actions)
      <details>
        <summary>Action Types</summary>

        - [UICActionCloseModal](#uicactionclosemodal)
        - [UICActionDelayedAction](#uicactiondelayedaction)
        - [UICActionDisableSaveButtonOnValidationErrors](#uicactiondisablesavebuttononvalidationerrors)
        - [UICActionGetPost](#uicactiongetpost)
            - [UICActionGet](#uicactiongetpost)
            - [UICActionPost](#uicactiongetpost)
        - [UICActionGetValue](#uicactiongetvalue)
        - [UICActionGoBack](#uicactiongoback)
        - [UICActionMarkChanges](#uicactionmarkchanges)
        - [UICActionNavigate](#uicactionnavigate)
        - [UICActionOpenResultAsModal](#uicactionopenresultasmodal)
        - [UICActionRefreshPartial](#uicactionrefreshpartial)
        - [UICActionServerResponse](#uicactionserverresponse)
        - [UICActionSetEdit](#uicactionsetedit)
        - [UICActionSetReadonly](#uicactionsetreadonly)
        - [UICActionSetValue](#uicactionsetvalue)
        - [UICActionTriggerSubmit](#uicactiontriggersubmit)
        - [UICActionValidateObject](#uicactionvalidateobject)

      </details>
    - [Buttons](#buttons)
      <details>
        <summary>Button Types</summary>

        - [UICButton](#uicbutton)
        - [UICButtonCancel](#uicbuttoncancel)
        - [UICButtonCollapseCard](#uicbuttoncollapsecard)
        - [UICButtonCreate](#uicbuttoncreate)
        - [UICButtonDelete](#uicbuttondelete)
        - [UICButtonEdit](#uicbuttonedit)
        - [UICButtonGroup](#uicbuttongroup)
        - [UICButtonRefreshPartial](#uicbuttonrefreshpartial)
        - [UICButtonSave](#uicbuttonsave)
        - [UICButtonToolbar](#uicbuttontoolbar)
        - [UICToggleButton](#uictogglebutton)

      </details>
    - [Card](#card)
      <details>
        <summary>Card Types</summary>

        - [UICCard](#uiccard)
        - [UICCardHeader](#uiccardheader)
        - [UICTabs](#uictabs)
        - [UICModal](#uicmodal)
        - [UICAccordion](#uicaccordion)

      </details>

  </details>
  <details>
	<summary>Taghelpers</summary>
		
	- [UICCustomTaghelper](#uiccustomtaghelper)
  </details>

- [Custom Generators](#custom-generators)
- [Database entities (IDbEntity)](#database-entities-idbentity)
- [ClientSide responseHandling](#clientside-responsehandling)
- [Services](#services)
  <details>
    <summary>Available Services</summary>

    - [IUIComponentGenerator](#iuicomponentgenerator)
    - [IUICValidationService](#iuicvalidationservice)
    - [IUICStoredComponents](#iuicstoredcomponents)
    - [IUICQuestionService](#iuicquestionservice)

  </details>
  <details>
    <summary>Implementable Service-Interfaces</summary>

    - [IUICILanguageService](#iuicilanguageservice)
    - [IUICPermissionService](#iuicpermissionservice)
    - [IUICDefaultCheckValidationErrors&lt;IUICPropertyValidationRuleReadonly&gt;](#iuicdefaultcheckvalidationerrorsltiuicpropertyvalidationrulereadonlygt)
    - [IUICSignalRService](#iuicsignalrservice)

  </details>
- [Attributes](#attributes)
- [DataTypes](#datatypes)
- [Web Extension Methods](#web-extension-methods)
- [Controllers](#controllers)
- [Logging](#logging)

<details>
  <summary>Additional Component Models (Inputs, Tables, Dropdown, Tree, FileExplorer, Graphs, Icons, Texts, ContextMenu, Carousel, Questions, Notifications, ...)</summary>

- [Inputs](#inputs)
- [Tables](#tables)
- [Dropdown](#dropdown)
- [Tree](#tree)
- [FileExplorer](#fileexplorer)
- [Graphs](#graphs)
- [Icons](#icons)
- [Texts](#texts)
- [ContextMenu](#contextmenu)
- [Carousel](#carousel)
- [Questions](#questions)
- [Notifications & Realtime](#notifications--realtime)
- [Data Sources & Caching](#data-sources--caching)
- [UICUpload](#uicupload)
- [UICSpaceSelector](#uicspaceselector)
- [UICPartial](#uicpartial)
- [UICEvent](#uicevent)
- [UICGroup](#uicgroup)

</details>

# Implementation

## C#
In program.cs, add the this to the builder.
```c#
builder.Services.AddUIComponentWeb(config =>
{
	config.AddDefaultGenerators(builder.Services);
	config.AddDefaultValidators(builder.Services);
});
```

## Javascript
In _scripts.cshtml or the layout page, add the partial containing all scripts.
This partial is automatically generated in /views/shared/_UicScripts.cshtml
```Javascript
<partial name="_Scripts.UIC" />
```
:warning: Make sure that all overrides of this javascript code come after the importing of the script

## scss
```css
<link rel="stylesheet" href="~/uic/css/uic.scss" asp-append-version="true" />
```
## ViewImports
These usings can also be added to GlobalUsings 
```
@using UIComponents.Abstractions;
@using UIComponents.Abstractions.Attributes;
@using UIComponents.Abstractions.Enums;
@using UIComponents.Abstractions.Extensions;
@using UIComponents.Abstractions.Interfaces;
@using UIComponents.Abstractions.Models;
@using UIComponents.Models.Models;
@using UIComponents.Models.Models.Actions;
@using UIComponents.Models.Models.Buttons;
@using UIComponents.Models.Models.Card;
@using UIComponents.Models.Models.Dropdown;
@using UIComponents.Models.Models.Icons;
@using UIComponents.Models.Models.Inputs;
@using UIComponents.Models.Models.Texts;
@using UIComponents.Models.Models.Tree;
@using UIComponents.Models.Extensions;
@using UIComponents.Generators.Interfaces;
@using UIComponents.Generators.Models;
@using UIComponents.Generators.Services;
@using UIComponents.Web.Extensions;
@using UIComponents.Web.Taghelpers;

//This line finds all taghelpers in current project, including those in the UIComponents folder
@addTagHelper *, <NameOfYourWebProject>
```

# Creating components
Create items manually or use the [IUIComponentGenerator](#iuicomponentgenerator) to generate components.

## Rendering components from view
```c#
@await myComponent.InvokeAsync(Component)
```
```c#
@await Component.InvokeAsync(myComponent)
```
Rendering a Task< IUIComponent > is also supported
```c#
@await _uic.CreateComponentAsync(testModel).InvokeAsync(Component)
```
There is also a taghelper for rendering components. If your component supports [IUICHasAttributes](#iuichasattributes), attributes assigned to the component are applied to the component
```cshtml
<uic c="myComponent" class="my-custom-class" invoke=true></uic>
<uic i="myComponent" class="my-custom-class"></uic>
```
If this component implements the [IUICSupportsTaghelperContent](#iuicsupportstaghelpercontent), you can also write content within the tags.
> For more info, see [UICTaghelper](#uictaghelper)



## Rendering components from controller
```c#
public IActionResult RenderMyComponent()
{
	var myComponent = �
	return View("/UIComponents/ComponentViews/Render.cshtml", myComponent);
}
```

### :bulb: Hint:
In the basecontroller you can create a overload that automatically goes to the correct view.
```c#
public IUIAction View(IUIComponent component) => View("/UIComponents/ComponentViews/Render.cshtml", component);
```

# ClientSide methods
Some components support clientside methods.
These often have Trigger... methods available to quickly generate code that triggers a clientside function.
When opening the console clientside, you can select a html element and run this line:
```javascript
$($0).trigger('uic-help');
```
This will look at all parent elements and display the available methods in console.
Most clientside .trigger('uic-...') methods stop propagation on the first matching element! 


## changeWatcher
You can use the uic.changeWatcher in javascript to check if properties are changed. more info can be found in the changeWatcher.js file.

# Component Models

## Extension Methods
[IUIComponent](#iuicomponent)
```c#
//Returns false if the component is null or if the component has a Render property set to false. Else returns true 
myComponent.HasValue()
```

```c#
IEnumerable<IUIComponent> multipleComponents;

mutlipleComponents.AnyHasValue()
```

### Add Methods
These methods only work on [IUICHasChildren](#iuichaschildren).
This is supported by components like cards, groups, modals, etc

All Add methods support fluent programming.

Normal Add method:
```c#
var myGroup = new UICGroup();
myGroup
	.Add(component1)
	.Add(component2)
	.Add(component3);
```

Add method with out parameter:
```c#
var myGroup = new UICGroup();

//Add a new card to the group and assign it to a new variable, the card property is available for later use.
myGroup.Add(out var card, new UICCard("TestCard"));

card.Add(out var button1, new UICButton());

button1.OnClick = ...;
```

Add method with action:
```c#
var myGroup = new UICGroup();

//Add a new card to the group and provide a action to configure the card further.
myGroup.Add(new UICCard("TestCard"), card =>{
	
	card.Add(new UICButton(), button =>{
		button.OnClick = ...;
	})
});
```

Mix And Match Add methods:
```c#
var myGroup = new UICGroup();
myGroup
	.Add(component1)
	.Add(out var card1, new UICCard("FirstCard"))
	.Add(new UICCard("SecondCard"), card2 =>{

	});
	//In this example, all 3 items are added to the group, but card1 is still available as variable and can still be edited before rendering.
	//The card2 is not available as variable, and only used in its own function
```


### Find... methods
The Find... methods can recursively find all IUIComponents that are assigned as properties. Supports nested search.
This does not work on properties that have the [UICIgnoreGetChildrenFunctionAttribute](#uicignoregetchildrenfunctionattribute).

Supported extentions: 
- FindAllOfType\<T\>()
- FindAllChildrenOfType\<T\>()
- FindFirstOfType\<T\>()
- FindFirstChildOfType\<T\>()
- FindInputByPropertyName\<T\>(string propertyName)
- FindInputGroupByPropertyName(string propertyName)
- TryFindFirstOfType\<T\>()
- TryFindFirstChildOfType\<T\>()
- TryFindInputByPropertyName\<T\>(string propertyName)
- TryFindInputGroupByPropertyName(string propertyName)




```c#
//Returns the first childProperty of the given type
var result = myComponent.FindFirstOfType<UICButtonToolbar>();

//If no UICButtonToolbar is found in myComponent, this will crash!
myComponent.FindFirstOfType<UICButtonToolbar>().ButtonDistance= Long;

myComponent.FindFirstChildOfType<UIButtonToolbar>(toolbar =>{
	//Run code here, only runs if there is a toolbar found
});
```
```c#
//Returns true or false if the component is found
bool result = myComponent.TryFindFirstOfType<UICButtonToolbar>();

myComponent.TryFindFirstOfType<UIButtonToolbar>(toolbar =>{
	//Run code here, only runs if there is a toolbar found
});
```

```c#
//Get a list of all buttons that are inside this component
var results = myComponent.FindAllOfType<UICButton>();
```

### Get or Set Attributes
These methods only work on [IUICHasAttributes](#iuichasattributes).
This is supported by most components, but may not work on many [IUICActions](#iuicaction).

```c#
//Returns the Id, if no Id exists, generate a random Id
string id = myComponent.GetId();

//Set the Id of a component
myComponent.SetId("MyCustomId");
```
> :warning: Will throw a exception if you set the id and there is already a id assigned!

Add Html Attributes:
```c#
myComponent
	.AddAttribute("data-value", "stringValue")
	.AddClass("my-testclass")
	.AddStyle("max-height:100%;");
```



## Interfaces
### IUIComponent
This is the base interface that is applied to all models.
This interfaces requires a path to a renderlocation. 
You can also place this interface on any model or viewmodel, and this object will be ignored by the generators and use the renderlocation with the current model.

> If you want to add custom Html or javascript as a IUIComponent, you can use a [UICCustom](#uiccustom).

> If you want to apply multiple components as a IUIComponent, you can use a [UICGroup](#uicgroup).

```c#
public interface IUIComponent
{
	public string RenderLocation { get; }
}
```
> :warning: If the RenderLocation does not have a '.' in the last 7 characters, .cshtml is automatically added to the RenderLocation

### IUICAction
This is a IUIComponent that is only used as action (f.e. Javascript code)
```c#
/// <summary>
/// This is a action for buttons or events
/// </summary>
public interface IUICAction : IUIComponent
{
}
```

### IUIComponentViewModel
A interface that wraps around a viewModel, for more info, check [UIComponentViewModel](#uicomponentviewmodel)

### IUICHasAttributes
Marker interface for components that can carry html attributes (`id`, `class`, `style`, `data-*`). Implementing it enables the `GetId()`/`SetId()`/`AddAttribute()`/`AddClass()`/`AddStyle()` extension methods described in [Get or Set Attributes](#get-or-set-attributes). Most components implement this; some `IUICAction`s (e.g. plain `UICCustom` javascript snippets) do not.

### IUICHasChildren
Marker interface for components that hold a collection of child `IUIComponent`s (e.g. `UICGroup`, `UICCard.Body`, `UICForm.Children`). Implementing it enables the [Add methods](#add-methods) (`.Add(...)`, `.Add(out var x, ...)`, `.Add(x, configure => {...})`) and makes the component traversable by the [Find... methods](#find-methods).

### IUICSupportsTaghelperContent
Marker interface for components whose content can be written as literal Razor markup inside the `<uic-custom>` taghelper instead of a C# string/`RazerBlock` — see [UICCustomTaghelper](#uiccustomtaghelper). Implemented by `UICCustom`, `UICForm`, `UICLabel`, `UICSpan`, `UICGroup`, `UICPartial`, `UICCarousel`, `UICSignalR`, `UICEvent`, and others.

### IDropdownItem
Empty marker interface (`UIComponents.Abstractions.Interfaces`) — implement it on a custom class to make that class usable inside `UICDropdown.DropdownItems` / `UICDropdownSubMenu.Items` (see [Dropdown](#dropdown)). Built-in implementations: [UICDropdownItem](#uicdropdownitem--idropdownitem-ihasiconuicicon), [UICDropdownHeader](#uicdropdownheader--idropdownitem-ihasiconuicicon), [UICDropdownDivider](#uicdropdowndivider--idropdownitem), [UICDropdownSubMenu](#uicdropdownsubmenu--uicdropdownitem-iuichaschildrenidropdownitem), and `UICCustom`.

## Models
### UICCustom

This is a custom component that can be used as [IUIComponent](#iuicomponent), [IUICAction](#iuicaction) or [IDropdownItem](#idropdownitem).

You can write the Html or javascript in the constructor as a string.
```c#
var button = new UICButton("test");
button.OnClick = new UICCustom("console.log('I have clicked the testButton');");
```

You can use the AddLine method on a UICCustom to keep adding a additional line to the string
```c#
var customOnClick = new UICCustom()
	.AddLine("let a = 2;")
	.AddLine("let b = 5;")
	.AddLine("let c = a * b;")
	.AddLine("alert(`The result is ${c}`)");
```
```c#
var customHtml = new UICCustom()
	.AddLine("<div class=\"row\">")
	.AddLine("  <div class=\"col\">")
	.AddLine("  </div>")
	.AddLine("  <div class=\"col\">")
	.AddLine("  </div>")
	.AddLine("</div>");
```

While in a csHtml page, you can also add razor code.
```javascript
var customOnClick = new UICCustom(@<text>
	let a = 2;
	let b = 5;
	let c = a * b;
	alert(`The result is ${c}`);
</text>);
```
```html
var customHtml = new UICCustom(@<text>
	<div class="row">
		<div class="col">
		</div>
		<div class="col">
		</div>
	</div>
</text>);
```

A UICCustom component without any content will always be ignored.


### Translatable
Translatable is a object that can be assigned and will translated just before sending to the client, This means you can create these keys without language context or send it to multiple users in diffrent languages.

To Translate these objects, you need to implement the [IUICILanguageService](#iuicilanguageservice) interface.

```c#
/// <summary>
/// The Key to Identify the translation
/// </summary>
public string ResourceKey { get; set; }

/// <summary>
/// Default value for key, if no translation exists
/// </summary>
public string DefaultValue { get; set; }

/// <summary>
/// Arguments to place inside de defaultValue. May include nested <see cref="Translatable"/>
/// </summary>
public object[] Arguments { get; set; }`
```

#### Untranslated / String
Any Component that needs a Translatable, you can also provide with a string. The string will be converted to a UnTranslatable in the background and will not be translated. 
```c#
var button1 = new UICButton(new("Button.Translate", "Translate this button"))
var button2 = new UICButton("Do not translate this button");
```
> :warning: If the string starts with "\[TRANSLATABLE]" this will be seen as a serialized translatable


#### Serialize translatable
If you want to store a translatable in a database as string, you can use the .Serialize() methods.
```c#
Translatable translatable = button1.ButtonText;
Translatable untranslated = button2.ButtonText;

//untranslatedText == "Do not translate this button"
string untranslatedText = untranslated.Serialize();

//Serialized format for this translatable
string serializedText = translatable.Serialize();

//x => Untranslated "Do not translate this button"
Translatable x = untranslatedText;

//y => Translatable with key "Button.Translate" and defaultValue "Translate this button"
Translatable y = serializedText;
```

### UIComponentViewModel
This component wraps around any viewmodel. Providing it with a renderlocation so you can use any ViewModel as a component.

This has the same result as returning a View or Partial view, but can be used as a component instead.
```c#
public class UIComponentViewModel<T> : IUIComponentViewModel
{
	public UIComponentViewModel(string renderlocation, T viewModel)
	{
		RenderLocation = renderlocation;
		ViewModel = viewModel;
	}

	public T ViewModel { get; set; }

	public string RenderLocation { get; set; }
	object IUIComponentViewModel.ViewModel => ViewModel;
}
```

Example:
```c#
var vm = await _factory.CreateViewModelAsync();

var modal = new UICModal("TestCard")
				.Add(new UIComponentViewModel("/Views/Shared/MyViewLocation", vm));

return ViewOrPartial(modal);
```



### Actions
#### UICActionCloseModal
```c#
/// <summary>
/// This function is called where there is no modal available to close
/// </summary>
public IUIAction OnFailed { get; set; } = new UICCustom();
```

#### UICActionDelayedAction
```c#
public UICActionDelayedAction()
{
			
}
public UICActionDelayedAction(int miliseconds, DelayedActionType delayType, IUICAction action)
{
	Miliseconds = miliseconds;
	Action = action;
	DelayType = delayType;
}


/// <summary>
/// The time used by the <see cref="DelayType"/>
/// </summary>
public int Miliseconds { get; set; }

/// <summary>
/// The type of the delay is configured here
/// </summary>
public DelayedActionType DelayType { get; set; }

/// <summary>
/// This action will trigger after the <see cref="Miliseconds"/> delay. Multiple triggers of this will only result in 1 trigger.
/// </summary>
public IUICAction Action { get; set; } = new UICCustom();

public enum DelayedActionType
{
	/// <summary>
	/// Waits for a period of inactivity, then execute the action
	/// </summary>
	Debounce,

	/// <summary>
	/// Waits for the delay, then execute once. All other triggers while waiting are ignored
	/// </summary>
	Delay,

	/// <summary>
	/// Trigger instantly, then block all triggers in the delay. After the delay is completed and one or more triggers are blocked, execute the action again one time
	/// </summary>
	Throttle
}
```

#### UICActionDisableSaveButtonOnValidationErrors
This component will automattically disable the button if there are any validation errors in the form
```c#
/// <summary>
/// This is the form or collection of properties that contains the validation errors.
/// </summary>
/// <remarks>
/// If null, the entire page cannot contain any validation errors
/// </remarks>
[UICIgnoreGetChildrenFunction]
public UIComponent Form { get; set; }


/// <summary>
/// This is the button that gets disabled on validation errors
/// </summary>
[UICIgnoreGetChildrenFunction]
public UIComponent SaveButton { get; set; }
```

#### UICActionGetPost
Make a Ajax request that uses [clientside responsehandling](#clientside-responsehandling).

This component has 3 levels of data.
- DefaultData
- GetVariableData
- FixedData

DefaultData and FixedData are 2 dictionaries that post their items as properties, while **GetVariableData** is a clientside function that is run when trigging the request. This data will overwrite matching properties of DefaultData, but FixedData will not be effected.

When creating a form, you can set the whole object as DefaultData and the [UICActionGetValue](#uicactiongetvalue) as **GetVariableData**. All properties that are not rendered will still be included in the post, but the rendered properties will overwrite these values.
```c#
public ActionTypeEnum ActionType { get; set; }

public string Controller { get; set; }

public string Action { get; set; }

/// <summary>
/// If not empty, use the url instead of <see cref="Controller"/> and <see cref="Action"/>
/// </summary>
public string Url { get; set; }

/// <summary>
/// This will be included on post, This takes lowest priority and can be overwritten by <see cref="GetVariableData"/> and <see cref="FixedData"/>
/// </summary>
public Dictionary<string, object> DefaultData { get; set; } = new();

/// <summary>
/// Before sending the request, this action is called client side to get additional properties.
/// <br>These properties have higher priority then <see cref="DefaultData"/> but lower than <see cref="FixedData"/></br>
/// </summary>
public IUIAction? GetVariableData { get; set; } = null;

/// <summary>
/// This will be included on post, and takes highest priority. This will overwrite all properties from <see cref="DefaultData"/> and <see cref="GetVariableData"/>
/// </summary>
public Dictionary<string, object> FixedData { get; set; } = new();

/// <summary>
/// When providing post options, these will overwrite the default options.
/// </summary>
public UICGetPostOptions? Options { get; set; } = null;

/// <summary>
/// A function that returns options. <see cref="Options"/> still takes priority over this
/// </summary>
public IUIAction? ClientSideOptions { get; set; } = null;

/// <summary>
/// This is the name of the responsevalue
/// </summary>
public string ResultName { get; set; } = "result";

/// <summary>
/// This will be triggered after posting.
/// </summary>
/// <remarks>
/// ResultName is available for this action
/// </remarks>
public IUIAction OnSuccess { get; set; }

/// <summary>
/// This will be triggered after failing the request
/// </summary>
public IUIAction OnFailed { get; set; }
```
```c#
#region Methods

public UICActionGetPost AddDefaultData(string key, object value)
{
	DefaultData[key] = value;
	return this;
}
public UICActionGetPost AddDefaultData(object data)
{
	foreach(var kvp in new RouteValueDictionary(data))
	{
		AddDefaultData(kvp.Key, kvp.Value);
	}
	return this;
}
public UICActionGetPost AddFixedData(string key, object value)
{
	FixedData[key] = value;
	return this;
}
public UICActionGetPost AddFixedData(object data)
{
	foreach (var kvp in new RouteValueDictionary(data))
	{
		AddFixedData(kvp.Key, kvp.Value);
	}
	return this;
}

#endregion
```

#### UICActionGetValue
Requires a component or a selector to create a action that gets the value.
You can set the value in a simular way by using [UICActionSetValue](#uicactionsetvalue)
```c#
[UICIgnoreGetChildrenFunction]
public IUIComponent Component { get; set; }

public string Selector { get; set; }
```

You can call this same method clientside in javascript
```javascript
var x = uic.getValue($($0));
```
When can also overwrite the **uic.getValue** method clientside for a specific selector to return a custom result.
```javascript
//The selected element will always return value '1'
$($0).on('uic-getValue', ()=>{
	return 1;
});
```

When calling the **uic.getValue** method on a element that is not a input, this will search for all child tags with a **name** attribute.

```html
<div id="myObject">
	<input type="text" name="String" value="foo"/>
	<div name="Group">
		<input type="number" name="Count" value="5"/>
		<checkbox name="Available" checked/>
		<input type="number" name="AlwaysOne" value="100">
	</div>
</div>
```
```javascript

$('#myObject input[name="AlwaysOne"]').on('uic-getValue', ()=>{
	return 1;
});
var x = uic.getValue('#myObject');

x = {
	String: "foo",
	Group: {
		Count: 5,
		Available: true,
		AlwaysOne: 1
	}
}
```

#### UICActionGoBack
```c#
public UICActionGoBack()
{
	Content = "history.back();";
}
```

#### UICActionMarkChanges
```c#
/// <summary>
/// The component to set the value too
/// </summary>
[UICIgnoreGetChildrenFunction]
public IUIComponent Component { get; set; }

/// <summary>
/// This is the name of the variable used to set the value
/// <br>Example: the name of the value received from signalR</br>
/// </summary>
public string ValueName { get; set; } = "referenceObject";
```


#### UICActionNavigate
```c#
public string Href { get; set; }
```


#### UICActionOpenResultAsModal
```c#
public string ResultPropertyName = "result";
```

#### UICActionRefreshPartial
```c#
/// <summary>
/// The partial that will be updated, if null, search the DOM tree for the closest partial
/// </summary>
[UICIgnoreGetChildrenFunction]
public UICPartial? Partial { get; set; } = null;


/// <summary>
/// When this partial is refreshing, this element will spin. Usefull for the icon of button
/// </summary>
/// <remarks>
/// If using a icon, be sure to generate a id for the icon, since a icon does not generate one itself.</remarks>
[UICIgnoreGetChildrenFunction]
public IUICHasAttributes? SpinElement { get; set; } = null;
```

#### UICActionServerResponse
This action takes a function that is triggered serverside each time the action is triggered
```c#

	public Func<Dictionary<string, string>, Task> Function { get; set; }
	public bool SingleUse { get; set; }

	/// <summary>
	/// This is the maximum time this connection can exist. Cleaning up the connection after this time.
	/// </summary>
	public TimeSpan Timeout { get; set; } = TimeSpan.FromHours(1);

	public object Data { get; set; }

	/// <summary>
	/// Before sending the request, this action is called client side to get additional properties.
	/// </summary>
	/// If this result has the same properties as <see cref="Data"/>, the <see cref="Data"/> takes priority.
	public IUIAction? GetVariableData { get; set; } = null;
```
Example:
```c#
var counter = 0;
var button = new UICButton("Test");
button.OnClick = new UICActionServerResponse(false, (data) => {
	counter ++;
	Console.WriteLine(counter);
});
```

#### UICActionSetEdit
Triggers the parent form to be editable

#### UICActionSetReadonly
Triggers the parent form to be readonly 
```c#
public bool ShowEmptyInReadonly { get; set; }
public bool ShowSpanInReadonly { get; set; }
public bool ShowDeleteButtonInReadonly { get; set; }
```

#### UICActionSetValue
Sets the value of a property or a container of multiple properties.
Works simularly as [UICActionGetValue](#uicactiongetvalue).
```c#
/// <summary>
/// The component to set the value too
/// </summary>
[UICIgnoreGetChildrenFunction]
public IUIComponent Component { get; set; }

/// <summary>
/// This is the name of the variable used to set the value
/// <br>Example: the name of the value received from signalR</br>
/// </summary>
public string ValueName { get; set; } = "referenceObject";
```
You can call this same method clientside in javascript
```javascript
uic.setValue($($0), {String: "blub"});
```
When can also overwrite the **uic.getValue** method clientside for a specific selector to return a custom result.
```javascript
//The selected element will always return value '1'
$($0).on('uic-setValue', (ev, value)=>{
	...
});
```

When calling the **uic.getValue** method on a element that is not a input, this will search for all child tags with a **name** attribute.

```html
<div id="myObject">
	<input type="text" name="String" value="foo"/>
	<div name="Group">
		<input type="number" name="Count" value="5"/>
		<checkbox name="Available" checked/>
	</div>
</div>
```
```javascript
uic.setValue('#myObject', {
	String: "blub",
	Group: {
		Count: 15,
		Available: false,
	}
});
```

#### UICActionTriggerSubmit
Triggers the submit from the parent form.

#### UICActionValidateObject
```c#
public string ReferenceObjectName { get; set; } = "referenceObject";

/// <summary>
/// Optional: The compared object must match all properties from this object
/// </summary>
/// <remarks>
/// foo* => objectProperty starts with foo
/// <br>*foo => objectProperty ends with foo</br>
/// <br>*foo* => objectProperty contains foo</br>
/// </remarks>
public object MatchObject { get; set; }

/// <summary>
/// Optional: The compared object may not match any of the properties from this object
/// </summary>
/// <remarks>
/// foo* => objectProperty starts with foo
/// <br>*foo => objectProperty ends with foo</br>
/// <br>*foo* => objectProperty contains foo</br>
/// </remarks>
public object NotMatchObject { get; set; }

/// <summary>
/// This action will be executed when the object has a match
/// </summary>
public IUIAction OnMatch { get; set; }

/// <summary>
/// This action will be executed when the object has no match
/// </summary>
public IUIAction OnMisMatch { get; set; }
```


### Buttons
Many buttons are just a override of [UICButton](#uicbutton) and have just the constructor defined.

#### UICButton
```c#
	public Translatable ButtonText { get; set; }

	public Translatable Tooltip { get; set; }

	public IColor? Color { get; set; } = ColorDefaults.ButtonDefault;

	/// <summary>
	/// Function triggered when clicking the button
	/// </summary>
	/// <remarks>
	/// ev => ClickEventArgs
	/// </remarks>
	public IUICAction OnClick { get; set; } = new UICCustom();

	public UICIcon PrependButtonIcon { get; set; }
	public UICIcon AppendButtonIcon { get; set; }

	public ButtonRenderer Renderer { get; set; } = ButtonRenderer.Default;
	public bool Disabled { get; set; }
```

#### UICButtonCancel
```c#
public UICButtonCancel() : base(TranslationDefaults.ButtonCancel)
{
	OnClick = new UICActionCloseModal()
	{
		OnFailed = new UICActionGoBack()
	};

	AddAttributeToDictionary("type", "reset", Attributes);
}
```
#### UICButtonCollapseCard
```c#
/// <summary>
/// The card that uses this button.
/// <br>If null, the closest parent is used.</br>
/// </summary>
public UICCard? Card { get; set; }
```

#### UICButtonCreate
```c#
public UICButtonCreate(Type type, bool modal = false)
{
	ButtonText = TranslationDefaults.ButtonCreate;

	if (type == null)
		return;

	Tooltip = TranslationDefaults.ButtonCreateTooltip(type);

	if (modal)
	{
		OnClick = new UICActionGetPost(UICActionGetPost.ActionTypeEnum.Get, type.Name, "Create", new { modalTitle = "" })
		{
			OnSuccess = new UICActionOpenResultAsModal()
		};
	}
	else
	{
		OnClick = new UICActionNavigate($"/{type.Name}/Create");
	}
}
```

#### UICButtonDelete
```c#
public UICButtonDelete()
{
	ButtonText = TranslationDefaults.ButtonDelete;
	Color = ColorDefaults.ButtonDelete;
	PrependButtonIcon = new UICIcon(UIComponents.Defaults.IconDefaults.Delete?.Icon ?? string.Empty);
	this.AddAttribute("class", "btn-delete");
}

public UICButtonDelete(Type type, object id) : this($"/{type.Name}/Delete", new { id = id })
{
}
public UICButtonDelete(string url, object data) : this()
{
	OnClick = new UICCustom($"await uic.form.delete('{url}', {JsonSerializer.Serialize(data)});");
}
```

#### UICButtonEdit
```c#
public UICButtonEdit() : base()
{
	ButtonSetEdit = new()
	{
		ButtonText = TranslationDefaults.ButtonEdit,
		OnClick = new UICActionSetEdit(),
	};
	ButtonSetReadonly = new()
	{
		ButtonText = TranslationDefaults.ButtonReadonly,
		OnClick = new UICActionSetReadonly(),
	};
}
```
```c#
public UICForm Form { get; set; }

public bool ReadonlyOnLoad { get; set; } = true;

public UICButton ButtonSetEdit { get; set; }
public UICButton ButtonSetReadonly { get; set; }
```

#### UICButtonGroup
 A buttongroup will combine multiple buttons and make them appear like one cohesive unit
 ```c#
public List<IUIComponent> Buttons { get; set; } = new();

public bool VerticalButtons { get; set; }
```

#### UICButtonRefreshPartial
```c#
public UICButtonRefreshPartial(UICPartial partial)
{
	PrependButtonIcon = new UICIcon(IconDefaults.RefreshIcon.Icon);
	ButtonText = TranslationDefaults.ButtonRefresh;
	PrependButtonIcon.GetId();
	OnClick = new UICActionRefreshPartial(partial, PrependButtonIcon);
}
```

#### UICButtonSave
```c#
public UICButtonSave()
{
	ButtonText = TranslationDefaults.ButtonSave;
	Color = ColorDefaults.ButtonSubmit?? ColorDefaults.ButtonDefault;
	this.AddAttribute("class", "btn-save");
}
```

#### UICButtonToolbar
```c#
public ButtonDistance Distance { get; set; } = UIComponents.Defaults.Models.Buttons.UICButtonToolbar.Distance;

public List<IUIComponent> Left { get; set; } = new();
public List<IUIComponent> Center { get; set; } = new();
public List<IUIComponent> Right { get; set; } = new();
```

> :warning: The [Add Extension methods](#add-methods) do not work on a buttonToolbar, since these can add Left, Center or Right.
The AddLeft() AddCenter() and AddRight() methods work in a simular way.


#### UICToggleButton
A Toggle button is 2 buttons that switch visibility.
```c#
public bool Value { get; set; }

/// <summary>
/// When true, you will not automatically change when clicking the button
/// </summary>
public bool DisableAutoChange { get; set; }

public UICButton ButtonTrue { get; set; }
public UICButton ButtonFalse { get; set; }
```

### Card
#### UICCard
```c#
/// <summary>
/// The header of the card, <see cref="UICCardHeader"/> is most used for this.
/// </summary>
public IHeader Header { get; set; }

/// <summary>
/// These are all the elements displayed in this card
/// </summary>
public UICGroup Body { get; set; } = new();

public UICGroup Footer { get; set; } = new();


/// <summary>
/// Do not display the header for this card. Header can still be used for tabs
/// </summary>
public bool HideHeader { get; set; }    

/// <summary>
/// If this card has a title, this property can set a card as closed by default.
/// </summary>
public bool DefaultClosed { get; set; }

public bool DisableClosing { get; set; }

/// <summary>
/// Store the collapsed state of a card in local storage. Next time the user visits this page it will remember if the card was collapsed or not
/// </summary>
/// <remarks>
/// This only works if the card has a Id Assigned => card.AddAttribute("id", "myId")
/// </remarks>
public bool RememberCollapsedState { get; set; } = true;

/// <summary>
/// If not empty, set this as the minimum width of the card
/// </summary>
public string MinWidth { get; set; } = "fit-content";

/// <summary>
/// If not empty, set this as the maximum width of the card
/// </summary>
public string MaxWidth { get; set; }
```

The Header is IHeader, you can create a custom header model if you want, but [UICCardHeader](#uiccardheader) is used by default.

**Add methods**
The [Add... Methods](#add-methods) will add components to its body.

The **AddFooter** works in a simular way, but adds the elements to the footer instead of the body.

**AddHeader**
Since the Header is IHeader, you cannot easily change header properties.
The AddHeader method can help solve this problem.

The AddHeader will create a new header of the requested type, but will trow a exception if there is already a header of a diffrent type.
```c#
var card = new UICCard();
card.AddHeader(out var header); //This header is UICCardHeader
card.AddHeader(header =>{
	//This header is also UICCardHeader.
})
card.AddHeader<MyCustomHeader>(out var header2) //This header is MyCustomHeader
```

**AddPartial**
Add a partial to the card. Using this method, this will also add a [UICButtonRefreshPartial](#uicbuttonrefreshpartial) to the card header.

#### UICCardHeader

A CardHeader can be used for [UICCard](#uiccard), [UICTab]

The buttons inside the List\<IUIComponent\> Buttons will use a diffrent renderer to better match the UICCardHeader.
```c#
public Translatable Title { get; set; }
public IColor? Color { get; set; } = ColorDefaults.CardHeaderDefault;

public List<IUIComponent> PrependTitle { get; set; } = new();
public List<IUIComponent> AppendTitle { get; set; } = new();

public List<IUIComponent> Buttons { get; set; } = new();

/// <summary>
/// If the card supports collapsing, Open or close it by clicking the header.
/// <br>Does not affect clickinig <see cref="Buttons"/></br>
/// <br>Can be disabled with ev.stopPropagation()</br>
/// </summary>
public bool CollapseCardOnClick { get; set; } = true;
```

#### UICTabs
```c#
public List<IUICTab> Tabs { get; set; } = new();

public List<IUIComponent> BeforeTabs { get; set; } = new();
public List<IUIComponent> AfterTabs { get; set; } = new();  

/// <summary>
/// Allow each tab button to have content
/// </summary>
public bool ColorTabs { get; set; }

/// <summary>
/// Remember what tab was last accessed. Requires <see cref="Id"/> to be assigned
/// </summary>
public bool RememberTabState { get; set; } = true;

/// <summary>
/// If only one tab is available, only render the content from that single tab
/// </summary>
/// <remarks>
/// Tabs can be added or removed based on permissions
/// </remarks>
public bool OnlyRenderSingleContent { get; set; }
```

#### UICModal
A modal works simular to a [UICCard](#uiccard) and has the same methods available.
```c#
 public IHeader Header { get; set; }

 public UICGroup Body { get; set; } = new();

 public UICGroup Footer { get; set; } = new();

 public bool ShowCloseButton { get; set; } = UIComponents.Defaults.Models.Card.UICModal.ShowCloseButton;

 /// <summary>
 /// Move the content out of its current location and place it on the body
 /// </summary>
 public bool MoveModalToBody { get; set; } = UIComponents.Defaults.Models.Card.UICModal.MoveModalToBody;

 public bool DisableCloseOnClickout { get; set; } = UIComponents.Defaults.Models.Card.UICModal.DisableCloseOnClickout;
 public bool DisableEscapeKeyToClickout { get; set; } = UIComponents.Defaults.Models.Card.UICModal.DisableEscapeKeyToClickout;

 /// <summary>
 /// Open the modal as soon as this is loaded on the page
 /// </summary>
 public bool OpenOnLoad { get; set; } = UIComponents.Defaults.Models.Card.UICModal.OpenOnLoad;

 /// <summary>
 /// When the modal is closed, remove the html from the page.
 /// </summary>
 public bool RemoveModalOnClose { get; set; } = UIComponents.Defaults.Models.Card.UICModal.RemoveModalOnClose;
```
**Trigger methods**
- TriggerOpen
- TriggerClose
- TriggerDestroy

#### UICAccordion
```c#
public List<UICCard> Children { get; set; } = new List<UICCard>();

/// <summary>
/// Default close all cards in accordion, the only exception is if the urlHash contains the card Id
/// </summary>
/// <remarks>
/// If <see cref="AllowOneCardOpen"/> is true, only first card is shown
/// </remarks>
public bool AllCardsClosedByDefault { get; set; }

/// <summary>
/// When opening a card, all other cards in the accordion will close
/// </summary>
public bool AllowOneCardOpen { get; set; }

public bool RemoveMarginBetweenCards { get; set; }
```

## Inputs

All input models inherit from the abstract `UICInput` (non-generic base) or `UICInput<T>` (typed value holder), both in `UIComponents.Abstractions.Models`. Namespace for the concrete input types: `UIComponents.Models.Models.Inputs`.

### Common base: `UICInput` / `UICInput<T>`
Every input below inherits these members — they are not repeated per type:
```c#
public string PropertyName { get; set; }      // C# property name, not the display label
public Translatable Placeholder { get; set; }
public Translatable Tooltip { get; set; }
public Translatable DisplayName { get; set; } // used in validation messages
public UICActions Actions { get; set; }       // clientside event hooks (on change, etc.)
public bool Readonly { get; set; }
public bool Disabled { get; set; }
public T Value { get; set; }                  // typed accessor over ValueObject (UICInput<T> only)
```
> :bulb: `TriggerGetValue()` returns an `IUICAction` that reads the current clientside value via `uic.getValue('#id')`.

Most inputs additionally expose their own `Validation...` properties (e.g. `ValidationRequired`, `ValidationMinLength`, `ValidationMinValue`, `ValidationMinimumDate`) which drive both clientside validation (`HasClientSideValidation`) and the generator/`IUICValidationService` pipeline described in [IUICValidationService](#iuicvalidationservice). These are plain settable properties — set them directly on the input after generation or construction, as shown in the `TestSubClass()` action of `HomeController`.

### UICInputGroup
The wrapper you almost always want around a `UICInput` — pairs a `UICLabel` with the input and optionally prepend/append content and a span. The generator always produces `UICInputGroup`s; you rarely construct a bare `UICInput` outside one.
```c#
public UICLabel Label { get; set; }
public UICInput Input { get; set; }
public List<UIComponent> PrependInput { get; set; } = new();
public List<UIComponent> AppendInput { get; set; } = new();
public UICSpan Span { get; set; }             // helper text under the input
public bool RenderWithoutInput { get; set; }  // render label/span even if Input has no value
public InputGroupRenderer Renderer { get; set; } = InputGroupRenderer.Default; // Default | Grid
```
```c#
public UICInputGroup(Translatable label, UICInput input);
```
Helper methods: `T InputAs<T>()` (casts, throws if wrong type), `InputAs<T>(Action<T>)`, `TryInputAs<T>(out T)`, `TryInputAs<T>(Action<T>)`.
```c#
group.Add(new UICInputGroup("Test", multiselect), inputgroup =>
{
    inputgroup.PrependInput.Add(new UICButton("Refresh") { OnClick = source.TriggerRefresh() });
    inputgroup.AppendInput.Add(new UICIcon("fas fa-user"));
});
```
> :bulb: Use [`FindInputGroupByPropertyName`](#find-methods) / `TryFindInputGroupByPropertyName` to locate a generated input group by the underlying C# property name, then swap or reconfigure its `.Input`:
```c#
component.TryFindInputGroupByPropertyName(nameof(TestModel.ObjectList), inputGroup =>
{
    if (inputGroup.Input is UICInputList list)
        inputGroup.Input = new UICInputTable(list.PropertyName, myTable) { Value = list.Value };
});
```

### UICForm
The root component required for `UICActionTriggerSubmit` / `ISubmitAction` to work. The generator normally wraps generated components in one automatically.
```c#
public UICForm(ISubmitAction submitAction);
public List<IUIComponent> Children { get; set; } = new();
public bool SetFocusOnFirstInput { get; set; } = true;
public bool DisablePostOnEnterClick { get; set; }
public bool DisablePostButtonsDuringPost { get; set; } = true;
public bool Readonly { get; set; }            // disables submit buttons
public ISubmitAction Submit { get; set; }
public IUICAction? TriggerSubmit();           // clientside: trigger('submit')
public IUICAction TriggerGetValue();
```
Supports the `<uic-custom>` taghelper content pattern (adds raw content as a child `UICCustom`).

### UICSingleRow
Renders a set of `IUIComponent` (usually `UICInputGroup`s) label+input pairs side-by-side in a single row instead of stacked. Custom components can opt in via `IUISingleRowSupport` (`TransformToSingleRow()` / `RendersInSingleRow()`).
```c#
public UICSingleRow(List<IUIComponent> components);
public List<IUIComponent> Components { get; set; } = new();
public string MinLabelWidth { get; set; }
public string MaxLabelWidth { get; set; }
public string MinInputWidth { get; set; }
public string MaxInputWidth { get; set; }
public string MarginBetweenRows { get; set; } = "0.5rem";
public string MarginBetweenColumns { get; set; }
public int? Columns { get; set; }
public UICGroup ConvertToGroup();   // back to a normal stacked UICGroup
public static List<IUIComponent> ConvertFromList(List<IUIComponent> components);
```
> :bulb: The generator option `InputGroupSingleRow` (see [IUIComponentGenerator](#iuicomponentgenerator)) uses this under the hood to lay out a whole generated form in single-row mode.

---

### Text-like inputs

#### UICInputText
`string` input. `Type` maps to an HTML5 `<input type>` via `System.ComponentModel.DataAnnotations.DataType` (default `DataType.Text`).
```c#
public DataType Type { get; set; } = DataType.Text;
public bool ValidationRequired { get; set; }
public int? ValidationMinLength { get; set; }
public int? ValidationMaxLength { get; set; }
public UICInputMultiline CovertToMultiline();
```

#### UICInputMultiline
`string` input rendered as a `<textarea>`.
```c#
public int? MinRows { get; set; }
public int? MaxRows { get; set; }   // beyond this, scrollbar appears
public bool ValidationRequired { get; set; }
public int? ValidationMinLength { get; set; }
public int? ValidationMaxLength { get; set; }
public UICInputText ConvertToSingleLine();
```

#### UICInputColor
`string` input storing a hex color or color name, rendered with the Coloris color picker.
```c#
public UICInputColorRenderer Renderer { get; set; } = UICInputColorRenderer.Coloris;
public bool AllowAlpha { get; set; } = true;
public bool OnlySystemColors { get; set; }   // restrict to UIComponents.Constants.Colors
public bool ValidationRequired { get; set; }
public bool ValidationValidColor { get; set; } = true;
```
> :warning: The generator auto-detects `UICInputColor` for any `string` property whose name contains "Color" — see [UICPropertyTypeAttribute](#uicpropertytypeattribute) to override.

#### UICInputCustom
Drops in raw HTML/Razor in place of a normal input; all other `UICInput` properties are ignored. Supports the `<uic-custom>` taghelper.
```c#
public UICInputCustom(RazerBlock razercode);
public string Content { get; set; }
```

---

### Numeric / boolean inputs

#### UICInputNumber
Backs any numeric C# type (`int`, `decimal`, `float`, etc. — generator maps them here); internally stored as `double?`.
```c#
public bool AllowDecimalValues { get; set; } = true;
public bool ValidationRequired { get; set; }
public decimal? ValidationMinValue { get; set; }
public decimal? ValidationMaxValue { get; set; }
```

#### UICInputCheckbox
`bool` input.
```c#
public IColor Color { get; set; }
public CheckboxRenderer Renderer { get; set; } = CheckboxRenderer.ToggleSwitch; // Checkbox | ToggleSwitch
```

#### UICInputCheckboxThreeState
`bool?` input (true / false / null / indeterminate), same shape as `UICInputCheckbox` but `Renderer` defaults to `CheckboxRenderer.Checkbox`.

---

### Date & time inputs

#### UICInputDatetime
`DateTime?` input.
```c#
public UICDatetimeStep Precision { get; set; } = UICDatetimeStep.Minute; // Date|Minute|Second|Millisecond
public bool ValidationRequired { get; set; }
public DateTime? ValidationMinimumDate { get; set; }
public DateTime? ValidationMaximumDate { get; set; }
```
```c#
component.TryFindInputByPropertyName<UICInputDatetime>(nameof(TestModel.MyDateTime), input =>
{
    input.ValidationMinimumDate = new DateTime(2024, 12, 1);
    input.ValidationMaximumDate = new DateTime(2025, 12, 31);
    input.ValidationRequired = true;
});
```

#### UICInputTime
`TimeOnly?` input.
```c#
public int Step { get; set; } = 1;                              // e.g. Step=15 with Precision=Minute -> 0/15/30/45 only
public UICTimeonlyEnum Precision { get; set; } = UICTimeonlyEnum.Minute; // Minute|Second|Milliseconds
public bool ValidationRequired { get; set; }
public TimeOnly? ValidationMinTime { get; set; }
public TimeOnly? ValidationMaxTime { get; set; }
public InputTimeRenderer Renderer { get; set; } = InputTimeRenderer.Default; // Default | SelectLists
```

#### UICInputTimespan
`TimeSpan?` input, rendered as separate day/hour/minute/second/millisecond fields — toggle which are visible.
```c#
public bool ShowDays { get; set; } = true;
public bool ShowHours { get; set; } = true;
public bool ShowMinutes { get; set; } = true;
public bool ShowSeconds { get; set; } = true;
public bool ShowMilliseconds { get; set; }
public bool ValidationRequired { get; set; }
public TimeSpan? ValidationMinValue { get; set; }
public TimeSpan? ValidationMaxValue { get; set; }
```

#### UICInputDateRange
Backs `IDateRangeInput` (`From`/`To`, `Duration` computed). Wraps daterangepicker.com.
```c#
public UICDatetimeStep Precision { get; set; } = UICDatetimeStep.Second;
public bool ForceFitInput { get; set; }
public bool ShowWeeknumbers { get; set; }
public bool AutoApply { get; set; }
public List<DateRangeSelector> RangeSelectors { get; set; } = new();  // presets, e.g. DateRangeSelector.Today()/.Last7Days()/.ThisMonth()
public bool AlwaysShowCalendar { get; set; }
public string DisplayFormat { get; set; }
public bool DisconnectCalendars { get; set; }
public Dictionary<string, object> Options { get; set; } = new();     // raw daterangepicker.com options
public bool ValidationRequired { get; set; }
public DateTime? ValidationMinimumDate { get; set; }
public DateTime? ValidationMaximumDate { get; set; }
public TimeSpan? ValidationMaxLength { get; set; }
```
```c#
var input = new UICInputDateRange(nameof(Model.Period))
{
    RangeSelectors = { DateRangeSelector.Today(), DateRangeSelector.Last7Days(), DateRangeSelector.ThisMonth() },
    Value = new DateRangeInput(DateTime.Today, DateTime.Today.AddDays(7))
};
```

#### UICInputRecurringDate
Backs `RecurringDate` (see [RecurringDate](#recurringdate)) — lets a user configure a weekly / monthly / custom-date recurrence rule in one widget.
```c#
public List<Type> AllowedTypes { get; set; } = new() { typeof(RecurringWeekly), typeof(RecurringMonthly), typeof(RecurringCustomDate) };
public bool ValidationRequired { get; set; }      // at least 1 selector required
public bool ValidationEndRequired { get; set; }   // end time required for selectors
public UICInputRecurringDate AddType(Type type);    // must implement IRecurringDateSelector
public UICInputRecurringDate RemoveType(Type type);
```

---

### Selection / list inputs

Three related-but-distinct concepts:
- **`UICInputSelectlist`** — single-value select (`<select>` / Select2).
- **`UICInputMultiSelect`** — multi-value tag-style select.
- **`UICInputList`** — a repeatable *complex* input: N copies of an arbitrary `UICInput`/sub-object, with add/remove/reorder buttons (e.g. a `List<SubObject>` property).
- **`UICInputTable`** — same repeatable-list use case as `UICInputList` but rendered as a [UICTable](#tables) instead of stacked cards — swap it in when you want a grid editor for a collection property (see the `TestSubClass()` example above).

#### UICInputSelectlist
```c#
public UICInputSelectList(string propertyName, List<SelectListItem> selectListItems); // MVC SelectListItem, auto-converted
public UICInputSelectList(string propertyName, List<UICSelectListItem> selectListItems = null);

public IColor? Color { get; set; }
public int SearchableIfMinimimResults { get; set; } = -1;  // 0 = always searchable, -1 = never
public List<UICSelectListItem> SelectListItems { get; set; } = new();
public bool ValidationRequired { get; set; }
public Translatable NoItemsText { get; set; }
public bool AllowDynamicOptions { get; set; }              // let user type a new option
public IUICAction? OnListOpen { get; set; }
public SelectListRenderer Renderer { get; set; } = SelectListRenderer.Select2; // Default | Select2
```

#### UICInputMultiSelect
```c#
public UICInputMultiSelect(string propertyName, List<SelectListItem> selectListItems);

public IColor? Color { get; set; }
public List<UICSelectListItem> SelectListItems { get; set; } = new();
public bool ClearInputAfterSelecting { get; set; } = true;
public bool CloseOnSelect { get; set; }
public bool AllowDynamicOptions { get; set; }
public Translatable NoItemsText { get; set; }
```

Both selectlist types share the same **3 ways to populate items**, and the same `AddSource(...)` fluent API (with `out` and `Action<>` overloads):

1. **Static items** — assign `SelectListItems` directly (see `UICSelectListItem` below).
2. **Remote endpoint** — `AddSource(UICActionGetPost)`, backed by `UICInputSelectListSource`:
   ```c#
   multiselect.AddSource(out var source, new UICActionGet("Home", "SelectListData"));
   // ...
   OnClick = source.TriggerRefresh()   // clientside: re-fetch the items
   ```
   `UICInputSelectListSource` properties: `GetSelectListItems` (the request), `Sorting` (`NoSorting|Text_Ascending|Text_Decending|Value_Ascending|Value_Decending`), `MapToSelectListItems` (optional clientside `IUICAction` to reshape raw response data into `{Value, Text}` pairs), `SkipInitialLoad`, `ReloadOnOpen`.
3. **Shared cache via `UICHtmlStorage`** — `AddSource(UICHtmlStorage)`, backed by `UICInputSelectListHtmlStorage` (same properties as above). Use this when multiple selectlists on the page should share one fetch — see [UICHtmlStorage](#uichtmlstorage).
   ```c#
   group.Add(out var storage, new UICHtmlStorage("SelectList",
       new UICActionGet("Home", "SelectListData"),
       new UICActionGet("Home", "GetSelectListTime")));   // 2nd action = cache-key/versioning check

   var selectStorage = new UICInputSelectList().AddSource(storage);
   var anotherSelect = new UICInputMultiSelect().AddSource(storage); // reuses the same fetched data
   ```

> :warning: Both selectlist server endpoints can return either `List<UICSelectListItem>` (full control incl. groups/icons/disabled) or plain MVC `List<SelectListItem>` — both are accepted by `Json(...)`.

#### UICSelectListItem / UICSelectListGroup
```c#
public UICSelectListItem(string text, object value);
public string Text { get; set; }
public object Value { get; set; }
public bool Selected { get; set; }
public bool Disabled { get; set; }
public UICSelectListGroup Group { get; set; }
public int SortOrder { get; set; }      // lower = higher in list
public string SearchTag { get; set; }   // extra text matched when searching
public List<IUIComponent> PrependText { get; set; } = new();  // e.g. icons
public List<IUIComponent> AppendText { get; set; } = new();
public string Tooltip { get; set; }
public UICSelectListItem AddPrepend(IUIComponent component);
public UICSelectListItem AddAppend(IUIComponent component);
```
`UICSelectListGroup` has `Name`, `Disabled`, `SortOrder`, `SearchTag`, `PrependText`/`AppendText`, `Tooltip`. Both implicitly convert from MVC's `SelectListItem` / `SelectListGroup`.
```c#
multiselect.SelectListItems.AddRange(new List<UICSelectListItem>
{
    new(){ Value="1", Text="one", Group=new(){Name="Group1"}, SearchTag="blub a b c" }.AddPrepend(UICIcon.Delete()),
    new(){ Value="3", Text="one", Group=new UICSelectListGroup(){Name="Group2", Disabled=true}.AddAppend(new UICIcon("fas fa-user")) },
    new(){ Value="5", Text="five", Disabled=true },
});
```

#### UICInputList
A repeatable set of instances of `SingleInstanceInput`, with per-row move/remove and an add button.
```c#
public UICInput SingleInstanceInput { get; set; }      // the template used for each row
public Type ItemType { get; set; }
public object DefaultValueAdd { get; set; }            // default values for a newly added row
public CreateNewInstanceEnum CreateNewInstanceMethod { get; set; } = CreateNewInstanceEnum.ReplaceIds; // ReplaceIds (clientside clone+regex) | GetRequest (serverside new instance)
public bool ShowMoveButtons { get; set; } = true;
public UICButton MoveUpButton { get; set; }
public UICButton MoveDownButton { get; set; }
public UICButton AddButton { get; set; }
public UICButton RemoveButton { get; set; }
public ButtonOrientationEnum ButtonOrientation { get; set; } // Auto | Horizontal | Vertical
public IUICAction TriggerAddInstance(object value = null);
public IUICAction TriggerRemoveInstance(string instanceSelector);
```
> :bulb: The generator produces a `UICInputList` automatically for `List<T>` properties where `T` is a complex object. Swap it for a `UICInputTable` (see example under [UICInputGroup](#uicinputgroup)) when a grid layout fits better.

#### UICInputTable
```c#
public UICInputTable(string propertyName, UICTable table);
public UICTable Table { get; set; }
public Type ItemType { get; set; }
```
See [UICTable](#tables) for configuring the table itself (columns, insert/update/delete).

---

### Complex / template inputs

#### UICInputObject
A container input for a nested/complex object — has `Children` like any `IUICHasChildren<IUIComponent>` and is what the generator produces for nested object properties (e.g. `SubModel`, `SubClass` on `TestModel`).
```c#
public List<IUIComponent> Children { get; set; } = new();
```

#### UICInputEditorTemplate
Delegates rendering to a plain MVC `EditorTemplate` (`~/Views/Shared/EditorTemplates/{TemplateFor}.cshtml`) instead of a UIComponents renderer — an escape hatch for existing Razor editor templates.
```c#
public UICInputEditorTemplate(string propertyName, string templateFor, object data = null);
public string Expression { get; set; }
public string TemplateFor { get; set; }
public object AdditionalData { get; set; }
```

## Tables

The Tables model category renders data grids (jsGrid-based) that can display static data or load data dynamically from a server endpoint, optionally support inline insert/update/delete, and can even be embedded as an editable input for `List<T>` properties.

### UICTable / UICTable\<T\>

`UICTable` is the non-generic base class; `UICTable<T>` is the strongly-typed entry point you use in views, giving you expression-based column selectors (`x => x.Prop`) instead of raw `PropertyInfo`.

```c#
public class UICTable : UIComponent, IUICSupportsTaghelperContent
{
    public UICTable();
    public UICTable(List<object> data);

    public string PropertyName { get; set; }
    public string Width { get; set; }
    public string Height { get; set; }
    public bool Resizable { get; set; }
    public List<IUICTableColumn> Columns { get; set; }
    public bool Filtering { get; set; }
    public bool Selecting { get; set; }
    public bool Sorting { get; set; }
    public bool AutoAddControlColumn { get; set; }
    public bool EnableInsert { get; set; }
    public bool EnableUpdate { get; set; }
    public bool EnableDelete { get; set; }
    public bool CanInsert { get; }   // EnableInsert && (OnInsertItem or OnInsertButtonClick set)
    public bool CanUpdate { get; }   // EnableUpdate && OnUpdateItem set
    public bool CanDelete { get; }   // EnableDelete && OnDeleteItem set
    public bool EnableHeaderAsTooltip { get; set; }
    public bool EnableSpansAndTooltips { get; set; }
    public bool InfinitePaging { get; set; }
    public List<object> Data { get; set; }
    public UICActionDelayedAction DelayedAction { get; set; }
    public GridSorter Sorter { get; set; }
    public int PageSize { get; set; }
    public string PagingSelector { get; set; }
    public bool Minimal { get; set; }
    public bool ReplaceLoadingIndicator { get; set; }
    public bool SaveFiltersInLocalStorage { get; set; }
    public bool SaveSortingInLocalStorage { get; set; }
    public bool ShowAllSelectFilters { get; set; }
    public bool FilterClientSize { get; set; }
    public bool SaveOnBlur { get; set; }
    public bool SaveOnEnter { get; set; }
    public List<UICSignalR> SignalRRefreshTriggers { get; set; }

    // Events (all IUICAction, wired to client-side callbacks)
    public IUICAction OnInit { get; set; }
    public IUICAction OnDataLoaded { get; set; }     // args.grid, args.data
    public IUICAction OnDataEditing { get; set; }    // args.grid, args.row, args.item, args.itemIndex
    public IUICAction OnItemDeleting { get; set; }   // args.grid, args.row, args.item, args.itemIndex; set args.cancel=true to abort
    public bool EditOnRowClick { get; set; }
    public IUICAction OnRowClick { get; set; }       // args.event, args.item, args.itemIndex
    public IUICAction LoadData { get; set; }         // args => filter arguments
    public IUICAction OnInsertItem { get; set; }     // args => item
    public IUICAction OnUpdateItem { get; set; }     // args => item
    public IUICAction OnDeleteItem { get; set; }     // args => item
    public IUICAction OnInsertButtonClick { get; set; }
    public IUICAction AdditionalConfig { get; set; }
    public IUICAction RowRenderer { get; set; }      // args => item, index

    public UICTable AddColumn(PropertyInfo propInfo, Action<UICTableColumn> config = null);
    public UICTable AddColumn(out UICTableColumn column, PropertyInfo propInfo);
    public UICTable AddColumn<T>(T column, Action<T> config = null) where T : class, IUICTableColumn;
    public UICTable AddColumn<T>(out T addedColumn, T column) where T : class, IUICTableColumn;
    public UICTable AddControlColumn(out UICTableColumnControl controlColumn);
    public UICTable AddControlColumn(Action<UICTableColumnControl> config = null);
    public UICTable InsertColumn(int index, PropertyInfo propInfo, Action<UICTableColumn> config = null);
    public UICTable RemoveColumn(PropertyInfo propInfo);
    public UICTable AddSignalR(UICSignalR signalR);
    public IUICAction TriggerReload();
}

public class UICTable<T> : UICTable where T : class
{
    public UICTable();                                  // wires OnInsertItem/OnUpdateItem/OnDeleteItem to POST {TypeName}/Insert|Update|Delete
    public UICTable(string id);
    public UICTable(List<T> data);
    public UICTable(UICActionGetPost loadDataFunc);      // sets LoadData

    public new IEnumerable<T> Data { get; set; }

    public UICTable<T> AddColumn(Expression<Func<T, object>> propExpression, Action<UICTableColumn> action = null);
    public UICTable<T> AddColumn(out UICTableColumn column, Expression<Func<T, object>> propExpression);
    public UICTableColumn GetColumn(Expression<Func<T, object>> expression, bool allowNew = true);
    public UICTable<T> AddColumns(string columnNames, bool caseSensitive = true);
    public UICTable<T> AddColumns(params Expression<Func<T, object>>[] propExpressions);
    public UICTable<T> ConfigureColumns(Action<UICTableColumn> action, params Expression<Func<T, object>>[] propExpressions);
    public UICTable<T> AddAllUndefinedColumns(bool includeId = false, bool includeIsDeleted = false);
    public UICTable<T> InsertColumn(int index, Expression<Func<T, object>> propExpression, Action<UICTableColumn> action = null);
    public UICTable<T> InsertColumn(int index, out UICTableColumn column, Expression<Func<T, object>> propExpression);
    public UICTable<T> RemoveColumn(Expression<Func<T, object>> propExpression);
    public UICTable<T> RemoveColumns(string columnNames, bool caseSensitive = false);
    public UICTable<T> OrderBy(Expression<Func<T, object>> expression, SortOrder sortOrder = SortOrder.Asc);
}
```

**Constructing a `UICTable<T>`:** the `UICTable<T>()` default constructor automatically wires `OnInsertItem`, `OnUpdateItem` and `OnDeleteItem` to `POST {typeof(T).Name}/Insert|Update|Delete` (with `GetVariableData = "item"`). Passing a `UICActionGetPost` (typically `new UICActionGet(controller, action)`) to the constructor sets `LoadData` for server-driven paging/filtering/sorting; passing a `List<T>` instead sets static `Data` (client-side only, no `LoadData` needed).

```c#
var table = new UICTable<TestModel>(new UICActionGet("testmodel", "LoadData"))
{
    EnableInsert = true,
    EnableUpdate = true,
    EnableDelete = true,
    SaveOnBlur = false,
    InfinitePaging = true,
    PageSize = 20,
    Sorter = new(nameof(TestModel.Number), SortOrder.Desc),
    SaveSortingInLocalStorage = false
}.SetId("testTable");

table.AddColumn(x => x.Description, config =>
{
    config.ColumnVisibility = UICTableColumnVisibility.HideSmallerThenLg;
});
table.AddAllUndefinedColumns().RemoveColumns("SubModel, RecurringDate, IntList, objectlist");
table.AddControlColumn(out var controlColumn);

@await table.InvokeAsync(Component)
```

**The `LoadData` endpoint contract:** `LoadData` is set to a `UICActionGet`/`UICActionPost` pointing at a controller action (the jsGrid client posts/gets to it every time data needs to (re)load — initial load, paging, sorting, or filtering). The client-side jsGrid grid engine (`jsGrid.js`) calls this endpoint with jsGrid's standard load parameters (page index, page size, per-column filter values, and sort field/order when `Sorting`/`Sorter` are in play) and expects back either a raw array of row objects, or — when paging is server-side (`InfinitePaging = false` and not client-paged) — an object shaped like `{ data: T[], itemsCount: number }` so the grid can compute total pages.

> :bulb: When `InfinitePaging` is true, there is no page selector — the grid keeps requesting subsequent pages as the user scrolls near the bottom.

> :warning: If you enable Sorting/Filtering without also handling the incoming sort/filter arguments server-side, the server must do so itself — client only re-sorts/filters automatically for static `Data`, not for `LoadData` results (see `Sorting`/`FilterClientSize` remarks below).

> :warning: `CanInsert`/`CanUpdate`/`CanDelete` are computed, read-only convenience flags — setting `EnableInsert = true` alone does **not** make the table insertable; you also need a working `OnInsertItem` (or `OnInsertButtonClick`) action, which `UICTable<T>`'s constructor provides by default.

> :bulb: `Sorting` only auto-sorts client-side `Data`. When using `LoadData`, your server endpoint must honor the incoming sort request. Likewise `FilterClientSize` re-runs filtering on the client for small static datasets rather than requiring server-side filter support.

> :bulb: `SaveFiltersInLocalStorage` / `SaveSortingInLocalStorage` persist state per browser, but require the table to have a stable `Id` (set via `.SetId(...)`) — otherwise there's nothing consistent to key the stored state on.

### UICTableColumn

The default, property-bound column type. Created via `AddColumn(x => x.Prop, config => ...)`; most of its fields (`Title`, `Type`, `SelectListItems`, min/max, tooltips, …) are auto-populated by `UICGridColumnGenerator` based on the property's type/attributes unless already set, or unless `IgnoreGenerators` is true.

```c#
public class UICTableColumn : IUIComponent, IUICTableColumn, IUICConditionalRender, IUICSortableTableColumn
{
    public UICTableColumn(PropertyInfo propInfo = null);

    public UICTable ParentTable { get; set; }
    public PropertyInfo PropertyInfo { get; set; }
    public string ColumnName { get; set; }             // falls back to PropertyInfo.Name
    public Translatable Title { get; set; }
    public Translatable Tooltip { get; set; }
    public SortOrder? SortOrder { get; set; }
    public string Type { get; set; }
    public object DefaultFilter { get; set; }
    public Dictionary<string, object> Options { get; }
    public bool AutoSearch { get; set; } = true;
    public UICIcon Icon { get; set; }
    public bool IgnoreTooltipAndSpanAttributes { get; set; }
    public bool AddTooltipIconInHeader { get; set; } = true;
    public string Width { get; set; }
    public string Css { get; set; }
    public bool Editing { get; set; } = true;
    public bool Filtering { get; set; } = true;
    public bool Render { get; set; } = true;
    public string Format { get; set; }
    public string Step { get; set; }
    public object? MinValue { get; set; }
    public object? MaxValue { get; set; }
    public bool CheckViewPermission { get; set; } = true;
    public bool CheckEditPermission { get; set; } = true;
    public bool IgnoreGenerators { get; set; }
    public List<SelectListItem> SelectListItems { get; set; }
    public bool Nullable { get; set; }
    public UICHorizontalAlignment? HorizontalAlignment { get; set; }
    public UICVerticalAlignment? VerticalAlignment { get; set; }
    public virtual IUICAction CellRenderer { get; set; }    // args: value, item
    public virtual IUICAction ItemTemplate { get; set; }    // args: value, item — returns markup/DomNode/jQuery element
    public virtual IUICAction EditTemplate { get; set; }    // args: value, item
    public virtual IUICAction FilterTemplate { get; set; }
    public virtual IUIComponent HeaderTemplate { get; set; }
    public UICTableColumnVisibility ColumnVisibility { get; set; } = UICTableColumnVisibility.VisibleOnAll;
    public UICTableColumnVisibility TextVisibility { get; set; } = UICTableColumnVisibility.VisibleOnAll;
    public UICTableColumnVisibility IconVisibility { get; set; } = UICTableColumnVisibility.VisibleOnAll;

    public UICTableColumn OrderBy(SortOrder order);
    public static string VisibilityClass(UICTableColumnVisibility visibility);
}
```

```c#
table.AddColumn(x => x.Number, config =>
{
    config.DefaultFilter = 10;
    config.Icon = new("fas fa-dollar");
    config.IconVisibility = UICTableColumnVisibility.VisibleSmallerThenLg;
    config.TextVisibility = UICTableColumnVisibility.HideSmallerThenLg;
});
```

You can also retrieve an already-added column later (e.g. from a `uic-custom` tag helper block) with `table.GetColumn(x => x.Decimal)`:

```cshtml
<uic-custom uic="table.GetColumn(x=>x.Decimal).ItemTemplate">
    return '€'+value;
</uic-custom>
```

> :warning: `GetColumn` requires the column to already exist (added via `AddColumn`/`AddAllUndefinedColumns`) — it does not create one. If not found and `allowNew` is left `true` (the default), it silently returns a throwaway, disconnected `UICTableColumn` instead of throwing, purely so a `uic-custom` block targeting it doesn't crash the page; check your column names carefully since this failure mode is silent.

> :bulb: `Icon`/`IconVisibility` and the column's text/`TextVisibility` are independent — you can show the icon on small screens and the text label on larger ones (or vice-versa) by combining `IconVisibility` and `TextVisibility` with different breakpoints, as shown above.

### UICTableColumnButton

A non-data column that renders a `UICButton` (or a custom click handler) per row — used for row-level actions like "view details" or "open in new tab" that aren't the standard edit/delete controls.

```c#
public class UICTableColumnButton : IUICTableColumn, IUICHasScriptCollection
{
    public UICTableColumnButton();
    public UICTableColumnButton(UICButton button);   // moves button.OnClick onto column.OnClick unless it's a UICActionNavigate

    public string Width { get; set; } = "auto";
    public Translatable Title { get; set; }
    public UICHorizontalAlignment Alignment { get; set; } = UICHorizontalAlignment.Center;
    public UICButton Button { get; set; }
    public IUICAction OnClick { get; set; }
    public IUICScriptCollection ScriptCollection { get; set; }
    public IUICAction Validation { get; set; }   // args: value, item — return false to hide button on that row
    public bool Render { get; set; } = true;
}
```

```c#
var testButton = new UICButton()
{
    PrependButtonIcon = new("fas fa-user"),
    OnClick = new UICActionNavigate("/table/details/?teststring=${item.TestString}")
};
table.AddColumn(new UICTableColumnButton(testButton)
{
    Width = "36px",
    Validation = new UICCustom("return item.Number < 120;")
});
```

> :bulb: Use `Validation` to conditionally show/hide the button per row (e.g. only allow an action while a numeric field is below a threshold) — it runs client-side against `value`/`item` for every rendered row.

> :warning: The `UICTableColumnButton(UICButton button)` constructor only reassigns `OnClick` from the button when it is *not* a `UICActionNavigate`. Navigate actions stay on `Button.OnClick` itself (this is how the real-world example wires `/table/details/?teststring=${item.TestString}` — the navigate action is set on `testButton.OnClick` after construction, not passed into the constructor's initial `OnClick`).

### UICTableColumnControl

The special column that hosts the built-in Insert/Edit/Delete controls. It's a `UICTableColumn` subclass, so it inherits all base column properties, plus:

```c#
public class UICTableColumnControl : UICTableColumn, IUICInitializeAsync
{
    public UICTableColumnControl();   // sets Type = "control", clears HeaderTemplate, appends "control-column" to Css

    public bool? Inserting { get; set; }     // null = auto-derived from EnableInsert / OnInsertButtonClick
    public bool? EditButton { get; set; }    // null = auto-derived from EnableUpdate / OnUpdateItem
    public bool? DeleteButton { get; set; }  // null = auto-derived from EnableDelete / OnDeleteItem
    public IUICAction BeforeButtons { get; set; }         // args: item — inject markup before default buttons
    public IUICAction AfterButtons { get; set; }          // args: item — inject markup after default buttons
    public IUICAction EditButtonCondition { get; set; }   // args: item — sync function, return true/false to show edit button
    public IUICAction DeleteButtonCondition;              // args: item — sync function (public field, not a property)
}
```

It is normally added automatically (`AutoAddControlColumn`, default `true`) whenever `EnableInsert`/`EnableUpdate`/`EnableDelete` is set, but you can add/retrieve it explicitly with `table.AddControlColumn(out var controlColumn)` to customize it (e.g. inject an extra icon via `BeforeButtons`):

```c#
table.AddControlColumn(out var controlColumn);
```

```cshtml
<uic-custom uic="controlColumn.BeforeButtons">
    return '<i class="fas fa-user"></i>';
</uic-custom>
```

> :warning: `DeleteButtonCondition` is a plain **field**, not a property — that's how it's declared in source (unlike its sibling `EditButtonCondition`, which is a property). Functionally you can still assign to it the same way, but be aware of the asymmetry if you're reflecting over the class.

> :bulb: `Inserting`/`EditButton`/`DeleteButton` default to `null`, meaning "decide automatically from the table's `Enable*`/`On*Item` settings." Only set them explicitly when you need to override that automatic behavior (e.g. hide the edit button in the control column while still allowing programmatic updates).

### UICTableColumnPartial

A column that renders an expandable/collapsible row detail panel, lazily loaded from a server partial when the row is expanded — useful for showing extra detail without a full page navigation or a dedicated modal.

```c#
public class UICTableColumnPartial : IUICTableColumn
{
    public UICTableColumnPartial();
    public UICTableColumnPartial(UICActionGetPost getPost);   // defaults GetVariableData to "item" if not already set

    public UICActionGetPost GetPost { get; set; }
    public string Identifier { get; set; } = "Id";     // property used to get a unique value per row
    public IUICAction Validation { get; set; }         // args: value, item — return false to not render for that row
    public bool Multiple { get; set; }                 // allow multiple expanded rows at once vs. auto-collapsing others
    public bool Render { get; set; } = true;
    public UICTableColumnVisibility ColumnVisibility { get; set; } = UICTableColumnVisibility.VisibleOnAll;
}
```

```c#
table.AddColumn(new UICTableColumnPartial(new UICActionGet("/table/details"))
{
    Identifier = "TestString",
    Multiple = true,
    ColumnVisibility = UICTableColumnVisibility.VisibleSmallerThenXl
});
```

> :warning: `Identifier` defaults to `"Id"` — if your model's key property is named something else (as in the example, `"TestString"`), you must set it explicitly or the partial's row-identification will look for a non-existent `Id` field.

> :bulb: Set `Multiple = true` when several row-detail panels should be able to stay open simultaneously; leave it `false` (default) so opening one panel collapses any other open panel — handy for detail views that are expensive to keep rendered.

### UICInputTable

Lets you use an entire `UICTable` as the editing surface for a `List<T>` property, instead of the default `UICInputList`. This is a drop-in replacement you typically swap in after the form/component has already generated a default list input.

```c#
public class UICInputTable : UICInput<object[]>
{
    public UICInputTable(string propertyName, UICTable table);
    public UICInputTable();

    public UICTable Table { get; set; }
    public Type ItemType { get; set; }
    public override bool HasClientSideValidation => false;   // inherited override
}
```

```c#
component.TryFindInputGroupByPropertyName(nameof(TestModel.ObjectList), inputGroup =>
{
    if (inputGroup.Input is UICInputList list)
    {
        var table = new UICTable<TestModel2>().AddAllUndefinedColumns();
        table.EnableDelete = true;
        table.EnableInsert = true;
        table.EnableUpdate = true;
        table.OnInsertItem = null;
        table.OnUpdateItem = null;
        table.OnDeleteItem = null;

        var tableInput = new UICInputTable(list.PropertyName, table) { Value = list.Value };
        inputGroup.Input = tableInput;
    }
});
```

> :warning: `HasClientSideValidation` is always `false` for `UICInputTable` — row-level validation (required fields, ranges, etc.) is not enforced client-side the way a normal scalar input would be; validate server-side if this matters.

> :warning: When embedding a table this way, explicitly null out `OnInsertItem`/`OnUpdateItem`/`OnDeleteItem` (as in the example) if you don't want the default `UICTable<T>` constructor's auto-wired `POST {Type}/Insert|Update|Delete` calls firing — since here the list is edited entirely in memory as part of the parent form/object and posted together with it, not persisted row-by-row via its own endpoints.

> :bulb: Set `Table.PropertyName` (or pass it via the constructor's `propertyName` parameter) so the table's rows serialize back as the array value of the correct property when the parent form is submitted.

### UICTableColumnVisibility

Controls per-column responsive visibility using Bootstrap's breakpoint display utility classes. `ColumnVisibility` applies to the whole column; `IconVisibility`/`TextVisibility` let a single column show its icon and its text label at different breakpoints independently.

```c#
public enum UICTableColumnVisibility
{
    VisibleOnAll = 0,          // d-table-cell (always shown)
    HiddenOnAll = 1,           // d-none (never shown)
    HideSmallerThanSm = 2,     // d-none d-sm-table-cell
    HideSmallerThanMd = 3,     // d-none d-md-table-cell
    HideSmallerThenLg = 4,     // d-none d-lg-table-cell
    HideSmallerThenXl = 5,     // d-none d-xl-table-cell
    VisibleSmallerThenSm = 6,  // d-table-cell d-sm-none
    VisibleSmallerThenMd = 7,  // d-sm-table-cell d-md-none
    VisibleSmallerThenLg = 8,  // d-sm-table-cell d-md-table-cell d-lg-none
    VisibleSmallerThenXl = 9,  // d-sm-table-cell d-md-table-cell d-lg-table-cell d-xl-none
}
```

In practice:
- **`HideSmallerThen*`** ("Hide smaller than Lg/Xl/Md/Sm") = the column is hidden below that breakpoint and shown at/above it — i.e. it's a "desktop-only, wide-screen" column.
- **`VisibleSmallerThen*`** ("Visible smaller than Lg/Xl/...") = the inverse: the column shows only below that breakpoint and is hidden at/above it — i.e. it's a "mobile-only, condensed" column.
- A common pattern (seen in the real-world example) is to pair one column configured `HideSmallerThenLg` (full text, desktop) with an icon/compact rendering on the same or another column configured `VisibleSmallerThenLg` (icon-only, mobile) so users always see *something* meaningful regardless of viewport.

> :bulb: The naming is breakpoint-relative, not viewport-absolute: "HideSmallerThenLg" hides below `lg` — i.e. it's still hidden on `sm`/`md` and only appears from `lg` upward. Read `HideSmallerThenX` as "requires at least X to show" and `VisibleSmallerThenX` as "requires smaller than X to show."

> :warning: `RemoveColumns` (on `UICTable<T>`) takes a **comma-separated string of property names** (`table.RemoveColumns("SubModel, RecurringDate, IntList, objectlist")`, case-insensitive by default), whereas `AddColumn` takes a **strongly-typed expression** (`table.AddColumn(x => x.Description, config => ...)`). Don't confuse this with `RemoveColumn` (singular), which does accept an expression (`Expression<Func<T, object>>`) — only the plural, bulk `RemoveColumns` uses the string form, mirroring `AddColumns(string columnNames, ...)`.

## Dropdown

### UICDropdown
A button that opens a menu of `IDropdownItem` entries.
```c#
public UICDropdown(IUIComponent button, List<IDropdownItem> dropdownItems = null);
public UICDropdown(Translatable dropdownText, List<IDropdownItem> dropdownItems = null); // wraps a UICButton automatically

public IUIComponent Button { get; set; }
public List<IDropdownItem> DropdownItems { get; set; } = new();

/// Ignored if none of the dropdownItems has an icon
public IconPositionEnum IconPosition { get; set; } = IconPositionEnum.Left; // Left, Right, None

/// If only 1 item is in DropdownItems, render that item's button directly instead of a dropdown
public bool ReplaceDropdownByButtonIfSingleDropdownItem { get; set; } = true;
```
Has `Add(item)`, `Add(out var item, item)`, `Add(item, configure => {...})` like any `IUICHasChildren` ([Add methods](#add-methods)).
```c#
var dropdown = new UICDropdown("Actions");
dropdown.Add(new UICDropdownItem("Edit", new UICActionSetEdit()){ Icon = UICIcon.Edit() });
dropdown.Add(new UICDropdownDivider());
dropdown.Add(new UICDropdownItem("Delete", new UICActionNavigate("/Delete")){ Icon = UICIcon.Delete() });
```
> :bulb: `ConvertToSubMenu()` converts this `UICDropdown` into a `UICDropdownSubMenu` (e.g. to nest it inside another dropdown).

### UICDropdownItem : IDropdownItem, IHasIcon\<UICIcon\>
A single clickable entry in a dropdown.
```c#
public UICDropdownItem(Translatable content, IUICAction onClick = null);

public Translatable Content { get; set; }
public Translatable Tooltip { get; set; }

/// Available args: e => eventArgs
public IUICAction OnClick { get; set; } = new UICCustom();
public UICIcon Icon { get; set; }

public IUIComponent BeforeContent { get; set; } = new UICCustom();
public IUIComponent AfterContent { get; set; } = new UICCustom();
```
> :bulb: `ConvertToButton(UICDropdown dropdown = null)` turns the item into a standalone `UICButton` (used internally when `ReplaceDropdownByButtonIfSingleDropdownItem` collapses a dropdown).

### UICDropdownSubMenu : UICDropdownItem, IUICHasChildren\<IDropdownItem\>
A dropdown item that opens a nested submenu.
```c#
public UICDropdownSubMenu(Translatable content, List<IDropdownItem> items);

public List<IDropdownItem> Items { get; set; } = new();

/// Render is always false if Items is empty
public override bool Render { get; set; }

/// If Items contains exactly 1 entry, render that single item instead of a submenu
public bool ReplaceBySingleItem { get; set; } = true;
```
Supports the same `Add(...)` overloads as `UICDropdown`. `ConvertToDropdown(parentDropdown)` converts it back into a standalone `UICDropdown`.

### UICDropdownHeader : IDropdownItem, IHasIcon\<UICIcon\>
A non-clickable title/section label inside a dropdown list.
```c#
public UICDropdownHeader(string content);
public Translatable Content { get; set; }
public Translatable Tooltip { get; set; }
public UICIcon Icon { get; set; }
```

### UICDropdownDivider : IDropdownItem
A plain horizontal divider line — no properties beyond the base component.
```c#
new UICDropdownDivider()
```

> :warning: `IDropdownItem` (`UIComponents.Abstractions.Interfaces`) is an empty marker interface — implement it on custom classes to make them usable inside `UICDropdown.DropdownItems` / `UICDropdownSubMenu.Items`.

---

## Tree

### UICTree
Renders a jsTree-based hierarchical tree. Supports either a static list of items or ajax-loaded children (not both at once — pick one per node via `HasAjaxChildren`).
```c#
public UICTree(string id, UICTreeLoadAjaxData GetdataFunc);   // ajax-driven tree
public UICTree(string id, List<UICTreeItem> items);           // static tree

public string Id { get; set; }

/// Requires Id to remain constant between page loads to restore state
public bool SaveState { get; set; } = true;

#region Checkbox
public bool EnableCheckbox { get; set; }
/// Ignored while CheckboxThreeState is enabled
public bool CheckboxCascadeUp { get; set; }
public bool CheckboxCascadeDown { get; set; } = true;
public bool CheckboxThreeState { get; set; } = true;
#endregion

public bool EnableDragAndDrop { get; set; }
/// Available args: event, data
public IUIComponent OnMove { get; set; }

/// A function that fetches child elements through ajax. Available args: obj, callback
public IUIComponent GetData { get; set; }

public List<UICTreeItem> TreeItems { get; set; } = new();
public List<IUIComponent> CustomComponents { get; set; } = new();
```

### UICTreeItem : IUICHasChildren\<UICTreeItem\>
A single node.
```c#
public UICTreeItem(Translatable text);

public string Id { get; set; }
public Translatable Text { get; set; }
public Translatable Tooltip { get; set; }
public string Icon { get; set; }
public JsTreeItemState State { get; set; } = new();   // { Opened, Disabled, Selected }
public List<UICTreeItem> Children { get; set; } = new();

/// If Children is empty, assumes this node has children fetched via the ajax call configured on UICTree
public bool HasAjaxChildren { get; set; } = true;

public Dictionary<string, string> Li_Attr { get; set; } = new();
public Dictionary<string, string> A_Attr { get; set; } = new();
```
> :warning: Static children win: only set `HasAjaxChildren = false` on leaf nodes (nodes with no real children), otherwise jsTree will keep showing an expand arrow that requests ajax data.

### UICTreeItems : IUICHasChildren\<UICTreeItem\>
A plain wrapper list of `UICTreeItem`, returned as JSON from the ajax endpoint referenced by `UICTreeLoadAjaxData`.
```c#
public List<UICTreeItem> Items { get; set; } = new();
```
Real ajax-endpoint example (from `HomeController.JsTreeItems`):
```c#
public async Task<IActionResult> JsTreeItems(string id, bool initial)
{
    var treeItems = new UICTreeItems();
    treeItems.Add(new("ajax1"));
    treeItems.Add(new("ajax2"), (item) =>
    {
        item.HasAjaxChildren = false; // leaf node, no further ajax calls
    });
    return ViewOrPartial(treeItems);
}
```

### UICTreeLoadAjaxData : IUICAction
Configures the ajax call jsTree uses to fetch node children.
```c#
public UICTreeLoadAjaxData(UICActionGetPost getPostAction, string initialId);

/// Sent on the very first request to get the initial (root) items
public string InitialId { get; set; }

/// A function producing a 'result' property containing the data jsTree parses (normally a UICActionGetPost)
public IUICAction Result { get; set; }
```
Server side, the target action just needs to return a `UICTreeItems` as JSON:
```c#
public async Task<IActionResult> GetData(string id)
{
    var items = new UICTreeItems();
    items.Items = await GetItemsAsync(id);
    return Json(items);
}
```

---

## FileExplorer

### UICFileExplorer
A full file-browser UI (Windows-explorer style) backed by pluggable services — never touches the filesystem directly from the model; all IO goes through `IUICFileExplorerService`/`IUICFileExplorerExecuteActions`.
```c#
/// The base directory the explorer is confined to
public string RootDirectory { get; set; }

/// Starting directory on page load (must be inside RootDirectory). If null, RootDirectory is used.
public string StartDirectory { get; set; }

/// Name of the controller implementing IUICFileExplorerController that serves this explorer's requests
public string ControllerName { get; set; } = "UICFileExplorer";

public string RenderMethod { get; set; } = Renderers.Details; // Renderers.Details | Renderers.JsGrid

#region Additional content (slots around the browser chrome)
public UICGroup TopContainer { get; set; }    // above everything
public UICGroup TopMain { get; set; }         // above the main window, between Left/Right
public UICGroup Left { get; set; }            // sidebar
public UICGroup Right { get; set; }           // e.g. preview pane
public UICGroup BottomMain { get; set; }
public UICGroup BottomContainer { get; set; } // e.g. context menu definitions
#endregion
```
`.AddAllAddons()` (extension method) wires up the standard UI in one call: a jsTree sidebar (`Left`), a preview pane (`Right`), a toolbar with "go up"/toggle-tree/toggle-preview buttons (`TopContainer`), and the full context menu (create folder, upload, open, download, cut/copy/paste, delete, rename) into `BottomContainer`.
```c#
var fileBrowser = new UICFileExplorer()
{
    RootDirectory = "C:\\",
    RenderMethod = UICFileExplorer.Renderers.JsGrid
}.AddAllAddons();
_pathMapper.RegisterPath("C:");   // REQUIRED — see below
return ViewOrPartial(fileBrowser);
```

#### Why `IUICFileExplorerPathMapper.RegisterPath` is required
Real filesystem paths are **never** sent to the client. `RegisterPath(basePath)` maps an absolute root (e.g. `"C:\"`) to an opaque `AbsolutePathReference` token; every path that crosses the wire is a `RelativePathModel { AbsolutePathReference, RelativePath }` built from that token, base64-encoded. This prevents path-traversal/disclosure: the client can only ever address paths under roots the server explicitly registered.
```c#
public interface IUICFileExplorerPathMapper
{
    string RegisterPath(string basePath);
    string GetAbsolutePath(IRelativePath relativePath);          // throws if reference unknown
    T GetRelativePath<T>(string absolutePath, string? preferredRoot = null) where T : class, IRelativePath;
    string ReplaceRoot(string path, string sourceRoot, string targetRoot);
}
```
> :warning: If you don't call `RegisterPath` for a root before rendering a `UICFileExplorer` using it, `GetAbsolutePath` will throw when the client requests it.

#### Pluggable services (DI)
Registered by `config.AddDefaultFileExplorerServices(builder.Services)` (called from `UICRegistrator`), each overridable via normal DI replacement:
```c#
services.TryAddSingleton<IUICFileExplorerPathMapper, UICFileExplorerPathMapper>();
services.TryAddScoped<IUICFileExplorerExecuteActions, UICFileExplorerExecuteActions>();
services.TryAddScoped<IUICFileExplorerService, UICFileExplorerService>();
services.TryAddScoped<IUICFileExplorerPermissionService, UICFileExplorerPermissionService>();
// plus 4 default IUICFileExplorerFileInfoManipulator implementations (icons/thumbnails), see below
```
| Interface | Responsibility |
|---|---|
| `IUICFileExplorerService` | Orchestrates listing/copy/move/delete/rename/preview/download; calls into `IUICFileExplorerExecuteActions` + runs all `IUICFileExplorerFileInfoManipulator`s over results |
| `IUICFileExplorerExecuteActions` | The actual raw IO: `AddFile(path, stream)`, `CreateDirectoryAsync`, `CopyFileAsync`, `DeleteFileAsync`, `MoveFileAsync`, `RenameFileAsync`, `RenameDirectoryAsync` — override this to target e.g. blob storage instead of local disk |
| `IUICFileExplorerPermissionService` | Per-path boolean checks: `CurrentUserCanViewFileOrDirectory`, `CanOpenFileOrDirectory`, `CanDownloadFileOrDirectory`, `CanCreateFileInThisDirectory`, `CanCreateFolderInThisDirectory`, `CanCreateOrEditFile`, `CanCreateDirectory`, `CanMoveFileOrDirectory`, `CanRenameFileOrDirectory(path, newFileName)`, `CanDeleteFileOrDirectory` — optional (constructor param defaults to `null` ⇒ everything allowed) |
| `IUICFileExplorerFileInfoManipulator` | Post-processes each `UICFileInfo` after listing (`AllowFiles`/`AllowDirectories` gates, `Priority` for ordering, `Initialize(filterModel, allFiles)` once per request, `ManipulateFileInfo(fileInfo)` per item, `Destroy()` cleanup). Built-ins: `ExtensionIconFileManipulator`, `ImageThumbnailsFileManipulator`, `PhotoIconFileInfoManipulator`, `UnknownFileIconFileManipulator` — register more with `services.AddScoped<IUICFileExplorerFileInfoManipulator, MyManipulator>()` (multi-registration, all run) |

`UICFileInfo : IRelativePath` is the DTO sent to the client per file/folder: `Thumbnail`, `Icon`, `FileName`/`Extension` (derived from `RelativePath`, `"folder"` extension for directories), `Created`, `LastModified`, `Size`/`SizeValue`, `CanOpen`/`CanDownload`/`CanMove`/`CanDelete`/`CanRename`, `IsFolder`, `DirectoryHasSubdirectories` (avoids a wasted ajax call for empty folders), `Data`/`Options` (arbitrary extra `data-*` attributes). `FileInfo`/`DirectoryInfo` (raw .NET `System.IO` handles) are cleared before serializing to the client to avoid leaking absolute paths.

#### Controller
Implement `IUICFileExplorerController` (see `UICFileExplorerController` in the tests project for the reference implementation) exposing: `CopyFiles`, `CreateDirectory`, `DeleteFiles`, `Download` (streams a single file or zips multiple via `UICFileExplorerHelper.DownloadFileOrZipStream`), `GetFilesForDirectoryPartial`/`GetFilesForDirectoryJson`, `MoveFiles`, `OpenFile`, `OpenImage`, `Preview`, `Rename`, `UploadPartial`/`UploadFiles` (chunked upload via `UICFileExplorerHelper.UploadFilesFromDropzoneStream`, dropzone-compatible). `UICFileExplorer.ControllerName` must match this controller's route name.

> :bulb: `RelativePathModel.FromBase64String(...)` / `.ToBase64String()` are how relative paths travel in query strings (e.g. `OpenFile(string base64)`).

---

## Graphs

### UICTimeLineGraph
A Chart.js-based time-series line chart with client-side pan/zoom, live updates, and server-fed data per line.
```c#
public List<LineGraph> LineGraphs { get; set; } = new();

public DateTime Start { get; set; } = DateTime.Now.AddDays(-5);
public DateTime End { get; set; } = DateTime.Now;
public DateTime? MinStart { get; set; }
public DateTime? MaxEnd { get; set; }

public string Width { get; set; } = "100%";
public string Height { get; set; } = "1000px";
public string MaxHeight { get; set; } = "85vh";

#region Legend
public bool ShowLegend { get; set; } = true;
public Position LegendPosition { get; set; } = Position.Top; // Top,Bottom,Left,Right
#endregion

#region DisplayFormats (moment.js format strings)
public string ColorMajor { get; set; } = "#FF0000";   // hex color for "major" date ticks; null disables the feature
public string DisplayFormatDay { get; set; } = "DD/MM";
public string DisplayFormatHour { get; set; } = "DD/MM - HH:mm";
public string DisplayFormatMinute { get; set; } = "HH:mm";
public string DisplayFormatSecond { get; set; } = "HH:mm:ss";
#endregion

#region Live data
public bool EnableNowIndicator { get; set; } = true;
public string NowIndicatorColor { get; set; }
public bool EnableLiveData { get; set; } = true;
/// Only applies to LineGraphs with Source set AND DisableFutureLoading == true
public TimeSpan LoadLiveDataMinInterval { get; set; } = TimeSpan.FromSeconds(3);
public TimeSpan? MoveGraphAfterLiveUpdate { get; set; } = TimeSpan.FromSeconds(1);
#endregion

#region Pan / Zoom
public bool EnablePanning { get; set; } = true;
public bool EnableZoom { get; set; } = true;
public TimeSpan? ZoomInLimit { get; set; } = TimeSpan.FromMinutes(1);
public TimeSpan? ZoomOutLimit { get; set; } = TimeSpan.FromDays(366);
#endregion

/// Fallback color palette (cycles) for LineGraphs without an explicit LineColor/BackgroundColor
public List<(string LineColor, string BackgroundColor)> LineColors { get; set; }
```
`.Add(new LineGraph(...))` / `.Add(out var lg, new LineGraph(...))` adds a series.

#### `UICTimeLineGraph.LineGraph`
```c#
public LineGraph(Translatable label, List<LineGraphPoint> points);       // static/pre-fetched data
public LineGraph(Translatable label, string id, string source);          // server-fed data

public Translatable Label { get; set; }
public List<LineGraphPoint> Points { get; set; } = new();

/// URL the client posts a RequestLineGraphDataModel to, to (re)fetch/extend data
public string Source { get; set; }
public string LineGraphId { get; set; }              // matches RequestLineGraphDataModel.LineGraphId

public bool CacheData { get; set; } = true;          // client-side cache to avoid re-processing
public bool RemoveCacheOnZoom { get; set; } = true;  // recommended when using LargestTriangleThreeBuckets decimation
public bool DisableFutureLoading { get; set; } = true; // never request data beyond "now"; enables minimal-diff live updates
public bool Enabled { get; set; } = true;            // active by default
public bool Stepped { get; set; }
public bool Fill { get; set; }
public string BackgroundColor { get; set; }
public string LineColor { get; set; }
public double BorderWidth { get; set; } = 1;
public int PointRadius { get; set; }
public int PointHitRadius { get; set; } = 100;       // tooltip proximity
public double Tension { get; set; }                  // curve smoothing

/// Round-trips to the server in RequestLineGraphDataModel.AdditionalPostData
public object AdditionalPostData { get; set; }
```
`LineGraphPoint(DateTime dateTime, double value, string id = null)` — serializes as `{ v, d, Id }` (`v`=Value, `d`=ISO date string), `DateTime` itself is `[JsonIgnore]`.

#### Client → server data flow
When a `LineGraph.Source` is set, the client posts a `RequestLineGraphDataModel` to that URL whenever it needs more data (initial load, pan, zoom, live-update tick):
```c#
public class RequestLineGraphDataModel
{
    public DateTime StartUTC { get; set; }
    public DateTime EndUTC { get; set; }
    public DateTime StartLocal => StartUTC.ToLocalTime();
    public DateTime EndLocal => EndUTC.ToLocalTime();
    public TimeSpan Scale { get; }                 // pixel-to-time resolution; use to decide bucket size for averaging
    public string LineGraphId { get; set; }        // which LineGraph this request is for
    public Dictionary<string, string> AdditionalPostData { get; set; } = new();

    public IEnumerable<LineGraphPoint> ReducePoints(List<LineGraphPoint> points, int maxPointsCount); // LargestTriangleThreeBuckets decimation
    public List<LineGraphPoint> AveragePerTimespan(List<LineGraphPoint> points, TimeSpan? timeSpan = null); // averages per Scale bucket, better for live data
}
```
Server action just returns `List<LineGraphPoint>` as JSON — real example from the tests project:
```c#
[HttpPost]
public IActionResult GetTimelineChartData(RequestLineGraphDataModel request)
{
    var data = (request.LineGraphId == "blub")
        ? TimelineDataFactory.GetPoint1(request.StartLocal, request.EndLocal)
        : TimelineDataFactory.GetPoint2(request.StartLocal, request.EndLocal);
    var reduced = request.AveragePerTimespan(data);
    return Json(reduced);
}
```
`DataDecimation` (static helper) provides the two reduction algorithms directly if you want to call them outside `RequestLineGraphDataModel`: `LargestTriangleThreeBuckets(points, threshold)` (visually-representative downsampling, best for a fixed historical range) and `AveragePerTimespan(points, timeSpan)` (bucket-averaging, better suited to live/streaming updates since it doesn't need the whole dataset).

> :bulb: Requires including Chart.js + the moment adapter + zoom plugin + hammer.js scripts — paths are configurable via `Script_ChartJs`, `Script_ChartJs_Adapter_Moment`, `Script_ChartJs_Plugin_Zoom`, `Script_HammerJs` (defaults point at `/lib/...`, so these libs must be present via libman/npm).

---

## Icons

### UICIcon : IUICIcon, IHasIcon
Renders a FontAwesome `<i>` tag.
```c#
public UICIcon(string icon); // e.g. "fa-solid fa-hashtag" — component does not render if Icon is empty

public string Icon { get; set; }
public IColor? Color { get; set; }
public Translatable Tooltip { get; set; }
```
Built-in presets: `UICIcon.Add()`, `.Checkmark()`, `.Close()`, `.Delete()`, `.Edit()`, `.Pin()`.

`UIComponents.Defaults.IconDefaults` holds overridable `Func<UICIcon>` factories used throughout the library's own buttons/components — override at startup to reskin all icons at once:
```c#
TooltipIcon, RefreshIcon, Add, Create, Edit, CancelEdit, Delete, Save,
ButtonCardCollapse, ButtonCardExpend, ButtonClose,
Upload, Download, Rename, CreateFolder, Cut, Copy, Paste,
OpenFolder, OpenFile, DirectoryUp
```
> :warning: `Create`, `Edit`, `CancelEdit`, `Delete`, `Save` default to `null` (no icon) unless you assign them.

---

## Texts

### UICLabel : IUICSupportsTaghelperContent
The label used by `UICInputGroup`, but usable standalone.
```c#
public UICLabel(Translatable labelText);

public Translatable LabelText { get; set; }
public Translatable Tooltip { get; set; }
public List<IUIComponent> PrependLabel { get; set; } = new();
public List<IUIComponent> AppendLabel { get; set; } = new();

/// Displayed only when Tooltip is not empty
public UICIcon TooltipIcon { get; set; } = IconDefaults.TooltipIcon?.Invoke();

/// Adds a required-marker next to the label text
public bool Required { get; set; }
```
Supports the `<uic-custom>` taghelper content pattern (setting `LabelText` from tag body).

### UICSpan : IUICSupportsTaghelperContent
A plain `<span>` of text — used e.g. as the info text under an input (`[UICSpanAttribute]`), or freely as its own component.
```c#
public UICSpan(Translatable text);
public Translatable Text { get; set; }
```

---

## ContextMenu

A declarative, selector-based right-click menu system (`uic.contextMenu` clientside) — you register `UICContextMenuItem`s once (often in a shared partial/layout) rather than attaching menus per-element; the correct item(s) are resolved at click-time by matching `Selector` against the clicked target.

### UICContextMenuItem
```c#
public UICContextMenuItem(string selector, IDropdownItem dropdownItem, IUICAction onClick = null);
public UICContextMenuItem(Func<string> selector, IDropdownItem dropdownItem, IUICAction onClick = null);

/// CSS selector defining which elements this item applies to, e.g. "#id", ".class", "button"
public Func<string> Selector { get; set; }

/// The visual row rendered in the menu (icon + text)
public IDropdownItem DropdownItem { get; set; }

public int? Position { get; set; }

/// Only one item may use a given Id; if several selectors match, the most specific Id wins (lets you override a generic item for a specific selector)
public string Id { get; set; }

/// Groups this item under a UICContextMenuCategory.CategoryId
public string Category { get; set; }

/// If ALL matching items are Optional, the menu doesn't open at all
public bool Optional { get; set; }

/// Args available: target (matched element), clickedElement (element actually right-clicked), event
public IUICAction OnClick { get; set; }
public IUICAction Text { get; set; }        // dynamic label override, same args
public IUICAction Title { get; set; }       // dynamic tooltip, same args
public new IUICAction Attributes { get; set; } // dynamic extra html attributes, same args
```
Real example (from `UICFileExplorerAddons.ContextMenu`):
```c#
var openFile = new UICContextMenuItem(
    $"#{fileExplorer.GetId()} .explorer-item:not(.explorer-folder):not(.cannot-open)",
    new UICDropdownItem("Open") { Icon = IconDefaults.OpenFile?.Invoke() },
    new UICCustom("uic.fileExplorer.openItem(target);"))
{
    Id = "FileExplorer.Actions.OpenFile",
    Category = "FileExplorer.FileActions"
};
```

### UICContextMenuCategory
Groups multiple `UICContextMenuItem`s together and controls how the group renders.
```c#
public string CategoryId { get; set; }         // matched by UICContextMenuItem.Category
public UICContextMenuItem MenuItem { get; set; } // used when the category itself renders as a sub-dropdown

/// (category, totalMenuItems) => html string
public IUICAction CategoryRenderer { get; set; }

public static UICCustom CategoryRendererGroup(bool addDividers);      // flat group, optional divider lines around it
public static UICCustom CategoryRendererIconsOnly(bool addDividers);  // icons-only row
public static UICCustom CategoryRendererSubMenu(bool addDividers);    // nested submenu
```

---

## Carousel

### UICCarousel : IUICHasAttributesAndChildren, IUICSupportsTaghelperContent
A Bootstrap-carousel-style rotating container — each child (e.g. `UICCard`, `UICTabs` page) becomes one slide.
```c#
public List<IUIComponent> Children { get; set; } = new();

public UICHoverVisibility ArrowIndicatorsVisibility { get; set; } = UICHoverVisibility.Visible;
public UICHoverVisibility TabIndicatorsVisibility { get; set; } = UICHoverVisibility.Visible;

/// If a child implements IUICHasColor, use that color for its indicator dot
public bool ColorTabIndicators { get; set; } = true;

/// null = no auto-advance
public TimeSpan? NextPageInterval { get; set; } = TimeSpan.FromSeconds(5);

/// Render the carousel shell even if there are no renderable children
public bool RenderEmpty { get; set; }

/// If only 1 child would render, skip the carousel chrome and render just that child
public bool OnlyRenderSingleContent { get; set; } = true;

public bool Loop { get; set; } = true;
public bool FadeAnimation { get; set; }

/// args: direction ("left"/"right"), relatedTarget, from, to
public IUICAction OnSlideStart { get; set; } = new UICCustom();
public IUICAction OnSlideFinished { get; set; } = new UICCustom();
```
Trigger methods: `TriggerPause()`, `TriggerResume()`, `TriggerGoToPage(int pageIndex)`, `TriggerPrevious()`, `TriggerNext()` — each returns a `UICCustom` js snippet (`$('#Id').carousel(...)`) usable as an `OnClick`, etc.
Supports the `<uic-custom>`/`<uic>` taghelper content pattern by wrapping raw tag content in a `UICCustom` child.

## Questions
Use `IUICQuestionService` to block server-side code and wait for a client's answer (e.g. "Are you sure?" before continuing a workflow). This requires **IUICSignalRService** to be implemented (see Services section) because the question is pushed to the client over SignalR and the answer comes back the same way.

All question types derive from `UICQuestionBase<TResponse>` and are built via a static `.Create(...)` factory rather than `new`, because `Create` also wires up the submit/cancel click handlers to the question service (`AssignClickEvents`).

```c#
public abstract class UICQuestionBase : IUIQuestionComponent, IUIComponent
{
    public Translatable Title { get; set; }
    public Translatable Message { get; set; }

    public bool InvertButtons { get; set; }

    /// <summary>
    /// If false, these options are ignored: <see cref="ShowClosebutton"/>, <see cref="ButtonCancel"/>, <see cref="CanClickOutSideModalToClose"/>
    /// </summary>
    public bool CanCancel { get; set; } = true;
    public bool ShowClosebutton { get; set; } = true;
    public UICButton ButtonSubmit { get; set; }
    public UICButton ButtonCancel { get; set; }

    public QuestionIconType? Icon { get; set; } // Success, Error, Warning, Info, Question

    public TimeSpan? RemoveAfterTimeout { get; set; }
    public bool CanClickOutSideModalToClose { get; set; } = true;

    public UICQuestionRenderer Renderer { get; set; } = UICQuestionRenderer.Modal;
}
```

### Asking a question (server side)
```c#
private readonly IUICQuestionService _uicQuestionService;

var yesNo = UICQuestionYesNo.Create("Test Ja / nee", "Wilt u deze vraag beantwoorden?", _uicQuestionService, question => question.Icon = QuestionIconType.Warning);

// TryAskQuestion(question, timeout, userId) targets a SPECIFIC user (or List<object> userIds — first response wins)
var answered = await _uicQuestionService.TryAskQuestion(yesNo, TimeSpan.FromMinutes(10), 1);
if (answered.IsValid && answered.Result)
{
    var dayOfWeek = UICQuestionSelectEnum<DayOfWeek>.Create("Favorite day", "What is your favorite day?", _uicQuestionService, question =>
    {
        question.Icon = QuestionIconType.Info;
        question.CanCancel = false;
    });

    // TryAskQuestionToCurrentUser requires an IUICGetCurrentUserId implementation and asks the user of the current http context
    var answered2 = await _uicQuestionService.TryAskQuestion(dayOfWeek, TimeSpan.FromMinutes(1), 1);
    if (answered2.IsValid && (answered2.Result == DayOfWeek.Saturday || answered2.Result == DayOfWeek.Sunday))
    {
        Console.WriteLine("In weekend");
    }
}
```

> :bulb: `TryAskQuestion` blocks the executing thread/task until the client responds, the timeout expires, or the user cancels — it does not require the caller to be inside a request for that specific user, since it targets an explicit `userId`/`userIds`. `TryAskQuestionToCurrentUser` is a convenience overload that resolves the current user via `IUICGetCurrentUserId` and instantly fails if no current user is found.

`UICQuestionResult<T>` is always returned, check `IsValid` before using `Result`:
```c#
public class UICQuestionResult<T>
{
    public bool IsValid { get; init; }
    public object? AnsweredByUserId { get; init; }
    public T? Result { get; set; }
    public bool IsCanceled { get; init; }
    public bool TimeoutExpired { get; init; }
}
```

### Question types
- **UICQuestionYesNo** — `UICQuestionYesNo.Create(title, message, service, configure?)`. Adds a `ButtonNo` next to the inherited `ButtonYes` (alias for `ButtonSubmit`). Result is `bool`.
- **UICQuestionText** — `UICQuestionText.Create(title, message, service)`. `Multiline`, `DefaultValue`, `ValidateRequired` (default `true`), `ValidateMinLength`, `ValidateMaxLength`, and a client-side `TextValidation` action (`IUICAction`, receives `value`, return a string to show as a validation error). Result is `string`.
- **UICQuestionSelectEnum\<TEnum\>** — `UICQuestionSelectEnum<DayOfWeek>.Create(title, message, service, configure?)`. Auto-populates `SelectListItems` from `Enum.GetNames(typeof(T))`, translated via `TranslationDefaults.TranslateEnums`. Result is `TEnum`.
- **UICQuestionSelectList** — `UICQuestionSelectList.Create(title, message, items, service)` (accepts `List<SelectItem>` or `List<SelectListItem>`). `EmptyText` translatable placeholder. Result is `string`.

> :warning: `UICQuestionSelectEnum<T>` inherits `UICQuestionSelectList`, so build a custom enum-like list by using `UICQuestionSelectList` directly if the values shouldn't come from a real `enum`.

### Translation workflow via Questions
`IUICAskUserToTranslate.AskCurrentUserToTranslate(TranslatableSaver.UICTranslationFilePath)` walks all `[Translatable]` keys with a missing translation and, for each, asks the current user (via `UICQuestionText` + `IUICQuestionService`) to fill it in — tying the [Translatable](#translatable) system to this same question infrastructure.

## Notifications & Realtime

### UICToastr
A popup notification (wraps the [Toastr](https://github.com/CodeSeven/toastr) JS library) that is pushed to a user through SignalR rather than rendered inline in a view.
```c#
public class UICToastr : IUIComponent
{
    public UICToastr(ToastType type, Translatable message, Translatable title = null);

    public Translatable Title { get; set; }
    public Translatable Message { get; set; }
    public ToastType Type { get; set; } // Success, Info, Warning, Error

    public ToastPosition Position { get; set; } = ToastPosition.TopRight;
        // TopRight, TopLeft, BottomRight, BottomLeft, TopCenter, BottomCenter, TopFullWidth, BottomFullWidth

    public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(5);       // 0 = persist
    public TimeSpan ExtendDuration { get; set; } = TimeSpan.FromSeconds(1); // extra time after hover-away, 0 = persist after hover
    public bool CloseButton { get; set; }
    public bool ProgressBar { get; set; } = true;
    public bool PreventDuplicates { get; set; } = true;
    public bool NewestOnTop { get; set; } = true;
    public IUICAction OnClick { get; set; }
    public Dictionary<string, object> Options { get; set; } = new(); // raw passthrough to toastr.js options

    public Task SendToUser(IUICStoredComponents storedComponents, object userId);
    public Task SendToUsers(IUICStoredComponents storedComponents, IEnumerable<object> userIds);
}
```
```c#
await new UICToastr(IUICToastNotification.ToastType.Success, "message", "title")
{
    Position = UICToastr.ToastPosition.BottomFullWidth
}.SendToUser(_storedComponents, 1);
```
> :bulb: `SendToUser`/`SendToUsers` are thin wrappers over `IUICStoredComponents.SendComponentToUserSignalR`, so **any** `IUIComponent` can be pushed to a connected user the same way — Toastr is just the most common case.

### IUICStoredComponents
A server-side store that lets you push a rendered component to a specific user, either immediately (SignalR, requires `IUICSignalRService`) or for later pickup on next page load.
```c#
public interface IUICStoredComponents
{
    bool TryGetComponent(string key, out IUIComponent component);
    IUIComponent GetComponent(string key);
    List<IUIComponent> GetComponentsByUser(object userId);

    string StoreComponentForUsers(IUIComponent component, IEnumerable<object> userIds, bool singleUse);

    void RemoveStoredComponent(string key);
    void RemoveStoredComponent(string key, object userId);

    Task SendComponentToUserSignalR(IUIComponent component, object userId, string appendTo = "body");
    Task SendComponentToUsersSignalR(IUIComponent component, IEnumerable<object> userIds, string appendTo = "body");
}
```
> :warning: If a component is stored as `singleUse`, it is automatically removed the first time it's retrieved via `GetComponentsByUser`/`GetComponent` — only call these when you actually intend to deliver the notifications to the user (see also the root README's existing note under [IUICStoredComponents](#iuicstoredcomponents)).

### UICSignalR
A component that listens for a raw SignalR broadcast and runs a client-side action when it arrives. Lower-level than `UICToastr`/`IUICStoredComponents` — used to build custom live-update behavior (see [UICEvent](#uicevent) below, and the custom-generator example in the root README that adds a `UICSignalR` to auto-refresh a `IDbEntity`-backed component).
```c#
public class UICSignalR : UIComponent, IUICSupportsTaghelperContent
{
    public UICSignalR(string subscriptionName, params string[] incomingArgs);

    public string SubscriptionName { get; set; }     // the signalR hub method to subscribe to
    public string Group { get; set; }                // signalR group to join
    public List<string> SubscriptionArguments { get; set; } = new(); // named args, in order, exposed to Action/Condition

    /// SignalR will not fire while the parent is hidden (e.g. closed card, hidden tab)
    public bool DisableOnHidden { get; set; } = true;

    public IUICAction Action { get; set; } = new UICCustom();     // has access to SubscriptionArguments
    public IUICAction Condition { get; set; } = new UICCustom();  // must return true/false; has access to SubscriptionArguments

    public bool Debug { get; set; } = UIComponents.Defaults.Models.UICSignalR.Debug;
}
```
Supports the `<uic-custom>` taghelper directly on `Action`:
```html
<uic-custom uic="mySignalR">
    console.log(sender, args);
</uic-custom>
```
Requires implementing `IUICSignalRService` (see root README's [IUICSignalRService](#iuicsignalrservice)) for the server push half.

### UICEvent
Bridges a plain C# `event` on a server-side service to a client-side SignalR trigger, without you needing to hand-roll a `UICSignalR` + group per event. It is an `IUICAction`, so it gets resolved (via `IUICGetComponent`) into a `UICSignalR` at render time, subscribing to the C# event through `IUICStoredEvents` and generating a unique group per subscription.
```c#
public class UICEvent<TArgs> : IUICAction, IUICConditionalRender, IUICSupportsTaghelperContent, IUICGetComponent where TArgs : EventArgs
{
    public UICEvent(Action<EventHandler<TArgs>> subscribeOnEvent, Action<EventHandler<TArgs>> unsubscribeFromEvent, IUICAction action = null);
    // or from reflection:
    public UICEvent(object service, EventInfo eventInfo, IUICAction action = null);

    public Action<EventHandler<TArgs>> SubscribeOnEvent { get; set; }
    public Action<EventHandler<TArgs>> UnsubscribeOnEvent { get; set; }

    /// Available args: sender, args
    public IUICAction Action { get; set; } = new UICCustom();
}
```
```c#
var evt = UICEvent<PriceChangedEventArgs>.Create(
    handler => _priceService.PriceChanged += handler,
    handler => _priceService.PriceChanged -= handler,
    new UICCustom("console.log('price changed', args);"));
```
> :warning: Requires `IUICStoredEvents` to be resolvable from DI (registered as part of `AddUIComponentWeb`). Only renders (`Render == true`) once `Action` itself has a value.

### UICSidePanel
A collapsible/pinnable/overlay sidebar layout wrapped around a main content area — implements `IUICSupportsTaghelperContentPassThrough` so `<uic-custom>` content targets `MainContent`.
```c#
public class UICSidePanel : UIComponent, IUICSupportsTaghelperContentPassThrough
{
    public UICSidePanel(IUIComponent mainContent, IUIComponent sidepanelContent);
    public UICSidePanel(IUIComponent sidepanelContent);

    public IUIComponent MainContent { get; set; } = new UICCustom();
    public IUIComponent SidePanelContent { get; set; } = new UICCustom();

    /// Set this to remember the panel state (Collapsed/Overlay/Fixed) across reloads for this user
    public string SidePanelIdentifier { get; set; }
    public UICSidePanelState DefaultState { get; set; } = UICSidePanelState.Collapsed; // Collapsed, Overlay, Fixed
    public UICSidePanelPosition Position { get; set; } = UICSidePanelPosition.Left;    // Left, Top, Right, Bottom

    public UICButtonToolbar ButtonToolbar { get; set; } = new(); // adding buttons here also adds them to the sidebar
    public UICButton SetFixedButton { get; set; }
    public UICButton OpenSidebarButton { get; set; }
    public UICButton CloseSidebarButton { get; set; }

    public IUICAction BeforeOverlay { get; set; } = new UICCustom();
    public IUICAction AfterOverlay { get; set; } = new UICCustom();
    public IUICAction BeforeCollapsed { get; set; } = new UICCustom();
    public IUICAction AfterCollapsed { get; set; } = new UICCustom();
    public IUICAction BeforePinned { get; set; } = new UICCustom();
    public IUICAction AfterPinned { get; set; } = new UICCustom();
}
```
> :warning: A `UICSidePanel` only renders if `MainContent` has a value — it will silently disappear if `MainContent` is null/empty, even if `SidePanelContent` is set.

## Data Sources & Caching

### UICHtmlStorage
A **named, client-side-cached AJAX data source**. Instead of re-fetching the same lookup data (e.g. a select list) for every input on the page, register it once and let multiple inputs read from the same cached value.
```c#
public class UICHtmlStorage : IUIComponent
{
    public UICHtmlStorage(string id, IUICAction getValue, IUICAction validateTimestamp);

    public string Id { get; set; }

    /// Result is compared against the previously stored value; when different, GetValue is invoked again
    public IUICAction ValidateTimestamp { get; set; } = new UICCustom();

    /// Fetches the actual data when empty or ValidateTimestamp indicates staleness
    public IUICAction GetValue { get; set; } = new UICCustom();

    public IUICAction TriggerGetValue();     // await uic.htmlStorage.getValueAsync('{Id}')
    public IUIComponent LoadHere(bool loadOutdatedFirst);
    public IUICAction TriggerExpired();      // uic.htmlStorage.triggerExpired('{Id}')
}
```
```c#
group.Add(out var storage, new UICHtmlStorage("SelectList",
    new UICActionGet("Home", "SelectListData"),     // GetValue: fetches the actual list
    new UICActionGet("Home", "GetSelectListTime"))); // ValidateTimestamp: a cheap call returning e.g. "yyyyMMddHH" used as a cache-busting version stamp

// Reuse the exact same cached source across unrelated inputs:
var selectStorage = new UICInputSelectList { Color = new UICColor("orange") }.AddSource(storage);
group.Add(selectStorage);
group.Add(new UICInputMultiSelect().AddSource(storage));
```
> :bulb: `ValidateTimestamp` is meant to be cheap (e.g. return a `DateTime` tick count or hash) so the client can decide, on every page load, whether it needs to re-fetch `GetValue` at all — avoiding a full refetch when nothing changed.

### UICCached
Wraps a component factory function so the **rendered HTML** (not just the underlying data) is generated once and reused on subsequent reloads — including translations, permission checks, and input values as they were at render time — until explicitly cleared.
```c#
public class UICCached : IUIComponent
{
    public UICCached(Func<Task<IUIComponent>> component);

    public Func<Task<IUIComponent>> Component { get; set; }
    public bool HasCachedValue { get; protected set; }
    public string CachedHtml { get; protected set; }

    public UICCached ClearCache();
    public void SetCachedValue(RazerBlock block);
    public void SetCachedValue(string value);
}
```
> :warning: Because the cached HTML is frozen at first render, per-user data (translations, permissions, values) will "leak" to the next reload if the underlying `Component` factory would otherwise have produced different output for a different user/state. Call `ClearCache()` explicitly whenever the source data changes.

## UICUpload
A drag-and-drop / click-to-upload widget (wraps [Dropzone.js](https://www.dropzone.dev/)) that posts files to a controller action.
```c#
public class UICUpload : UIComponent
{
    public UICUpload(string postUrl);

    /// MVC controller location to post the file, e.g. "/Upload/UploadFile"
    public string PostUrl { get; set; }
    public Dictionary<string, object> PostData { get; set; } = new();

    public long MaxFileSize { get; set; }     // MB
    public int MaxFileCount { get; set; } = 1;
    public string AcceptedFiles { get; set; } // e.g. ".jpg,.png"
    public Translatable Text { get; set; }    // dropzone placeholder text

    /// Called per file; available arg: file
    public IUICAction OnSuccess { get; set; } = new UICCustom();
    /// Called once, after all files finish
    public IUICAction OnSuccessAll { get; set; } = new UICCustom();
    /// Available args: file, message
    public IUICAction OnError { get; set; } = new UICCustom();

    public bool DisplayFileCountMessage { get; set; } = true;
    public bool AllowChunking { get; set; }
    public int ChunkSizeMB { get; set; } = 2;
    public int ParallelUploads { get; set; }
}
```
Matching controller action:
```c#
[HttpPost]
public async Task<IActionResult> Upload()
{
    var file = Request.Form.Files[0];
    var fileName = Path.GetFileName(file.FileName);
    using var fileStream = new FileStream($"...\\{fileName}", FileMode.Create, FileAccess.Write);
    await file.CopyToAsync(fileStream);
    return Json("Ok");
}
```
Static defaults for all uploads (theme-wide) live in `UIComponents.Defaults.Models.UICUpload` (`PostUrl`, `MaxFileSize`, `MaxFileCount`, `AcceptedFiles`, `Text`, `DropzoneCss`, `DropzoneScript`, chunking options).

## UICSpaceSelector
Renders whichever of several alternative representations of the *same content* best fits the available width, and re-evaluates automatically when the container resizes.
```c#
public class UICSpaceSelector : UIComponent, IUICSupportsTaghelperContent
{
    public UICSpaceSelector(IUIComponent bigElement, IUIComponent smallElement);

    /// Convenience constructor: shows the button list if it fits, otherwise collapses everything into a single dropdown
    public UICSpaceSelector(IEnumerable<IUIComponent> buttons, UICButton dropdownButton);

    /// Candidates, ordered largest-to-smallest; the largest one that fits is shown
    public List<IUIComponent> Elements { get; set; } = new();

    /// Selector for the closest resize-aware parent, default: ".card, .card-body, .modal, .content"
    public Func<string> WatcherSelector { get; set; } = () => ".card, .card-body, .modal, .content";
}
```
```c#
// Collapse a row of toolbar buttons into a dropdown when the toolbar is too narrow
var selector = new UICSpaceSelector(new List<IUIComponent> { button1, button2, button3 }, new UICButton { PrependButtonIcon = new UICIcon("fas fa-bars") });
```

## UICPartial
An AJAX-loaded content region — the model behind `UICButtonRefreshPartial` / `UICActionRefreshPartial` (see root README's Actions section) and `UICCard.AddPartial`.
```c#
public class UICPartial : UIComponent, IUICHasChildren<IUIComponent>, IUICSupportsTaghelperContent
{
    public UICPartial(string controller, string action, object data = null, ActionTypeEnum getPost = ActionTypeEnum.Get);
    public UICPartial(string url, object data = null, ActionTypeEnum getPost = ActionTypeEnum.Get);
    public UICPartial(UICActionGetPost getHtml);

    public UICActionGetPost GetHtml { get; set; }

    /// Placeholder content shown until the first AJAX load completes
    public List<IUIComponent> Children { get; set; } = new();

    public bool SkipInitialLoad { get; set; }
    public bool ShowLoadingOverlay { get; set; } = true;

    /// Debounce multiple near-simultaneous 'uic-reload' triggers (e.g. several signalR events) into one
    public TimeSpan? ReloadDelay { get; set; }

    /// If false, a reload triggered while hidden (closed card/hidden tab) is skipped entirely
    public bool ReloadIfHidden { get; set; } = true;
    /// If true, refresh every time the parent card/tab opens
    public bool ReloadIfParentOpens { get; set; } = false;

    /// Only for content that itself starts with a new partial container
    public bool ReplaceSelf { get; set; }

    public IUICAction? BeforeFetch { get; set; }
    public IUICAction? AfterFetch { get; set; }

    public IUICAction TriggerReload(); // => new UICActionRefreshPartial(this)
}
```
> :bulb: `ReloadIfHidden = true` (default) means a partial inside a closed card that receives a reload trigger while closed will still refresh once when that card opens (`ReloadIfParentOpens` controls whether it refreshes on *every* subsequent opening, not just the pending one).

## UICGroup
The generic container used throughout the library to treat a list of components as one `IUIComponent` (e.g. `UICCard.Body`, `UICCard.Footer`). This is what the root README's [Add methods](#add-methods) operate on.
```c#
public class UICGroup : UIComponent, IUICAction, IUICHasChildren<IUIComponent>, IUICHasAttributesAndChildren, IUICSupportsTaghelperContent
{
    public UICGroup(IEnumerable<IUIComponent> components);

    public UICGroupRenderer Renderer { get; set; } = UICGroupRenderer.Div; // Div | ContentOnly (no wrapping element)
    public List<IUIComponent> Components { get; set; } = new(); // same list as Children

    /// If no children render, don't render the wrapping div either
    public bool RenderWithoutContent { get; set; } = false;
    /// If exactly one child ends up rendering, skip the wrapper and render just that child
    public bool RenderSingleItem { get; set; } = false;
}
```
> :bulb: `UICGroup` also implements `IUICAction`, so it can be used anywhere an `IUICAction` is expected (e.g. `OnClick`) to run a *sequence* of other actions/components together.

## Taghelpers

### UICTaghelper

### UICCustomTaghelper
Using the razorCode inside the UICCustom will result in loss of intellisence. You can also use the **\<uic-custom\>** taghelper instead.

Before you can use the taghelper, you first need to add this to _ViewImports.cshtml:
```
@addTagHelper *, UIComponents.Web.Tests
```
Usage examples for html:
```html
@{
	var card = new UICCard("My Card");
	card.Add(out var customContent, new UICCustom())
}
<uic-custom uic="customContent">
	<h1>This is my custom card content</h1>
</uic-custom>

@await card.InvokeAsync(Component)
```
Usage examples for javascript:
```html
@{
	var button = new UICButton("Test");
}
<uic-custom uic="button.OnClick">
	alert('I have clicked on the button');
</uic-custom>

@await button.InvokeAsync(Component)
```
> :warning: The taghelper needs to be placed **before** the component is invoked.

> :warning: the taghelper accept all [IUIComponent](#iuicomponent) in compilation, but will throw a exception if the component is null or not a [UICCustom](#uiccustom)



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
			Group = args.PropertyType.Name,
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

## How component generation works

This section expands the existing **Custom Generators** docs with the internal pipeline contract, the full `UICOptions` surface, config toggles, the built-in property→input mapping, form-button conditions, global defaults, and additional validators.

### The generator pipeline contract
Every generator implements `IUICGenerator<TArgs, TResult>` (most concretely as `UICGeneratorBase<TArgs,TResult>` or via `UICCustomGenerator<TArgs,TResult>` created through `GeneratorHelper`). For a given request, **all matching generators run in ascending `Priority` order** (lowest first; built-ins sit around `1000`), each receiving the same `args` and the `existing` result produced so far:

```c#
Task<IUICGeneratorResponse<TResult>> GetResponseAsync(TArgs args, TResult? existingResult);
```

A generator must return one of:
- **`GeneratorHelper.Next<T>()`** (or non-generic `Next()`) — "I have no opinion, keep `existing` unchanged and call the next generator." `Success = false`, `Continue = true`.
- **`GeneratorHelper.Success(result, allowContinue)`** — "Here is a result." If `allowContinue` is `true`, later (higher-priority) generators still run and may replace/build on this result; if `false`, the pipeline **stops immediately** after this generator (e.g. permission checks return `Success(null, false)` to hard-stop button creation).

The pipeline itself lives in `UICConfig.GetGeneratedResultAsync<TArgs,TResult,TConverted>`: it collects generators registered as DI services (`configOptions.AddAndRegisterGenerator<T>()`) plus ad-hoc ones passed via `UICOptions.Generators` (`config.AddGenerator(...)` at call time), orders them by `Priority`, and folds the result. Any property carrying `[UICIgnore]` (including inherited via `UICInheritAttribute`) short-circuits the whole property pipeline before any generator runs.

Different "kinds" of generation (rendering the object, a property group, a single input, a span, a tooltip, select-list items, or one of the 5 form buttons) are distinguished by `args.CallCollection.CurrentCallType` (`UICGeneratorPropertyCallType`: `ClassObject`, `PropertyGroup`, `PropertyInput`, `PropertyGroupSpan`, `PropertyTooltip`, `SelectListItems`, `ButtonCreate`, `ButtonSave`, `ButtonDelete`, `ButtonEditReadonly`, `ButtonCancel`, `ButtonToolbar`, ...). `GeneratorHelper`'s factory methods (`PropertyGenerator`, `PropertyGroupGenerator`, `PropertyInputGenerator`, `ObjectGenerator`, `ButtonGenerator`, `SelectListItems`, `PropertyTypeGenerator`, `ForeignKeyTypeGenerator`, `PropertyToolTip`, `PropertySpanField`) each pre-filter on the right call type / property name / property type / declaring type so your custom function only fires when relevant.

### `UICOptions` — full surface
Passed per-call to `IUIComponentGenerator.CreateComponentAsync(model, options)`. Every property defaults from the static `UIComponents.Defaults / UICComponents.Generators.Defaults.OptionDefaults` class (override those once at startup to change the default for the whole app):

**Overrides**
- `Generators: List<IUICGenerator>` — extra generators used only for this call (via `.AddGenerator(...)`).
- `OptionsDictionary: Dictionary<string,object>` — free-form bag your own generators can read (`.AddDictionaryOption(key, value)`).
- `Partial: UICPartial?` — wrap the generated component in a partial (auto-skips initial load, adds a refresh button to the first card).

**Properties**
- `HideId` — hide any property literally named `Id`.
- `HideReadonlyProperties` — omit readonly properties entirely instead of rendering them disabled.
- `IncludedProperties` (comma string) — explicit, ordered property allow-list.
- `IncludedUndefinedProperties` — after `IncludedProperties`, also render any properties not mentioned (always true if `IncludedProperties` is empty).
- `ExcludedProperties` (comma string) — deny-list.

**Form**
- `NoForm` — skip wrapping in a `<form>` (for embedding read-only/sub content).
- `FormReadonly` — render entirely readonly.
- `HideEmptyInReadonly` — in readonly mode, hide inputs with no value.
- `PostForm: ISubmitAction` — the submit action (typically a `UICActionGetPost`).
- `OnSuccessfullSubmit: IUICAction` — action run after a successful post (`result` variable available).
- `PostObjectAsDefault` — post the whole model as `DefaultData` so un-rendered properties still round-trip.
- `PostIdAsFixed` — post `Id` as `FixedData` (client can't tamper); requires `IDbEntity`.
- `ReplaceSaveButtonWithCreateButton` — swap the Save button's generator for the Create button's.
- `FormToolbarInCardFooter` — wrap the form in a card and move the toolbar to the card footer.
- `DisableSaveButtonOnValidationErrors` — wire a `UICActionDisableSaveButtonOnValidationErrors` automatically.

**Buttons**
- `ToolbarPosition: AboveForm | BelowForm`.
- `ButtonOrder` (comma string, e.g. `"delete, cancel, edit, save"`) — any of `delete/cancel/edit/save` omitted are appended at the end; unknown keys must exist in `ButtonGenerators`.
- `ButtonGenerators: Dictionary<string, Func<UICButtonToolbar, UICPropertyArgs, Task>>` — register/override how a named toolbar slot builds its button(s); built-in keys are `delete`, `cancel`, `edit`, `save`.
- `ButtonPosition`, `EditButtonPosition`, `DeleteButtonPosition`, `CancelButtonPosition`, `SaveButtonPosition: ButtonPosition?` — per-button placement override (falls back to `ButtonPosition`, then the toolbar's own default).
- `ButtonDistance` — spacing between toolbar buttons.
- `ShowEditButton` — add the readonly/edit toggle button.
- `ShowDeleteButton` — add the delete button (default generator only fires for `IDbEntity`).
- `PostFullModelOnDelete` — delete button posts the whole model instead of just `Id` (also enables delete on non-`IDbEntity` types).
- `ShowCancelButton` — add the cancel button (closes modal, else browser back).

**Card**
- `ShowCardHeaders` — show/hide headers on auto-generated cards.
- `StartInCard: UICCard?` — put the generated component inside this card.
- `SubClassesInCard: UICCard?` — nested object/sub-model properties get their own card using this as a template.
- `SubCardTitleOverride: CardTitleOverride` — `NoOverride | ClassTranslatedNameOrTostring | ClassToString | ClassType | PropertyName`.

**Checkbox**
- `CheckboxColor: IColor?` — default color for all checkboxes/toggle switches.
- `CheckboxRenderer: CheckboxRenderer` — e.g. `ToggleSwitch` vs. plain checkbox.

**SelectList**
- `SelectlistAddEmptyItem` — prepend a blank "unselected" option.
- `SelectlistSearchableForItems: int` — item-count threshold above which a select becomes searchable.
- `SelectListShowAddButtonIfAllowed` — show an inline "add new" button when the user has create-permission for the referenced type.

**Misc**
- `DatetimePrecision: UICDatetimeStep`, `TimeOnlyPrecision: UICTimeonlyEnum` — default rounding for date/time inputs.
- `InputGroupSingleRow` — label + input on one row vs. stacked.
- `MarkLabelsAsRequired` — append a red `*` to labels of required inputs.
- `CheckReadPermissions` / `CheckWritePermissions` — enforce `IUICPermissionService` per property.
- `CheckClientSideValidation` — auto-populate `Validation...` properties on inputs from `IUICValidationService`.

### `UicConfigOptions` (the builder config object) & registration helpers
Passed as the lambda argument to `builder.Services.AddUIComponentWeb(config => { ... })` / `AddUIComponent`.

- `AddDefaultGenerators(services)` — registers **everything**: `UICPropTypeGenerator`, `UICGeneratorInitialPartial`, `UICGeneratorCard`, `UICGeneratorForm`, `UICGeneratorGroup`, `UICGridColumnGenerator`, all default property/input generators (see table below), all 6 form-button generators, `UICGeneratorRequired`, the default File Explorer services, and the default validation-error handlers for every `IUICDefaultCheckValidationErrors<...>` (Required/MinLength/MaxLength/Readonly/MinValue`<T>`/MaxValue`<T>` for every numeric + date/time type).
- `AddDefaultPropertyGenerators(services)` / `AddDefaultButtons(services)` — the two generator subsets `AddDefaultGenerators` composes from, callable independently if you want the buttons but a custom property pipeline (or vice versa).
- `AddDefaultValidators(services)` — registers `UICValidatorRequired`, `UICValidatorReadonlyAttribute`, `UICValidatorEditPermission`, one `UICValidatorRangeAttribute<T>` per numeric/date/time type, and one `UICValidatorMinMaxOfType<T>` per integer/decimal type (clamps to the CLR type's own min/max, e.g. `byte` → 0–255).
- `AddGenerator(type)` / `AddGenerator<T>()` — register a generator type for DI resolution *without* adding it to the container yourself (you must register it elsewhere, or use the `AndRegister` variant).
- `AddAndRegisterGenerator(type, services)` / `AddAndRegisterGenerator<T>(services)` — the usual path: DI-registers the type as scoped **and** adds it to the pipeline.
- `AddCustomGenerator(IUICGenerator<TArgs,TResult>)` / `AddGenerator<TArgs,TResult>(name, priority, func)` — add an already-built generator instance (typically produced by `GeneratorHelper.*`) directly, no DI involved.
- `AddPropertyGenerator(...)` / `AddObjectGenerator(type/T, ...)` — shorthand instance-based registration scoped to `UICPropertyArgs`→`IUIComponent`, with `AddObjectGenerator` pre-filtering by `ClassObject`/`PropertyType` assignability.
- `AddValidatorProperty<T>()` / `AddPropertyValidator(type)` — register a validation-rule **type** to be resolved from DI (needs separate `services.AddScoped<T>()`, or use `AddAndRegisterValidator<T>(services)` to do both).
- `AddValidatorPropertyRequired/MinValue<T>/MaxValue<T>/MinLength/MaxLength/Readonly(func)` — register a validation rule as a plain delegate, no class/DI required (always uses the built-in `DefaultCheckValidationErrors` result formatting).
- `AddUpdateMonitor(action)` — register a custom `IUICUpdateMonitor` (only takes effect if none is already registered).
- Toggles: `CheckLanguageServiceType`, `CheckPermissionServiceType` (both default `true` — disable if you don't implement those services), `CheckPropertyValidatorReadonly` (default `false` — suppresses the one-time "no readonly validator" error log), plus package-update toggles `OnlyReplaceNewerVersion`, `ReplaceCss`, `ReplaceComponents`, `ReplaceScripts`, `ReplaceTaghelpers`, `AddReadMe`, `AddChangeLog`, `AddFileExplorerImgs`, `AddTranslationFile` (control what the nuget package overwrites in your project's `/UIComponents` and `wwwroot/uic` folders on build).

### Built-in property → input mapping
`UICPropTypeGenerator` picks a `UICPropertyType` per property (attribute override via `[UICPropertyType(...)]` always wins), then a matching `UICGeneratorInput*` turns that into a concrete input:

| `UICPropertyType` | Detected by default for | Generated input |
|---|---|---|
| `String` | `string` | `UICInputText` |
| `MultilineText` | *(opt-in via attribute only)* | `UICInputMultiline` |
| `HexColor` | `string` property whose name ends in `"color"` | `UICInputColor` |
| `SelectList` | enums, `[ForeignKey]`, `[FakeForeignKey]`, or a resolved foreign-key type | `UICInputSelectlist` (+ `UICGeneratorEnumSelectListItems` supplies enum options) |
| `Number` | `sbyte/byte/short/ushort/int/uint/long/ulong` | `UICInputNumber` |
| `Decimal` | `float/double/decimal` | `UICInputNumber` (decimal mode) |
| `DateOnly` | `DateOnly`, or `[DataType(DataType.Date)]` | `UICInputDateTime` (date-only mode) |
| `DateTime` | `DateTime` | `UICInputDateTime` |
| `TimeOnly` | `TimeOnly`, or `[DataType(DataType.Time)]` | `UICInputDateTime` (time-only mode) |
| `TimeSpan` | `TimeSpan`, or `[DataType(DataType.Duration)]` | `UICInputTimespan` |
| `Boolean` | non-nullable `bool` | `UICInputCheckbox` |
| `ThreeStateBoolean` | nullable `bool?` | `UICInputCheckboxThreeState` |
| *(class/list types)* | classes, `IEnumerable<T>` (not `string`) | `UICGeneratorInputClassObject` (nested object) / `UICGeneratorInputList` (list) |
| `RecurringDate` | `RecurringDate` type | `UICInputRecurringDate` |

Other Property-stage generators regardless of type: `UICGeneratorLabel` (label text), `UICGeneratorInputGroup`/`UICGeneratorInputGroupSpan` (wraps label+input, adds span text), `UICGeneratorTooltip`/`UICGeneratorInputTooltip` (tooltip from `[UICTooltip]`), `UICGeneratorHtmlAttributes` (applies `[UICHtmlInput]`/`[UICHtmlInputGroup]`/`[UICHtmlLabel]`), `UICGeneratorPropertySetReadonly` and `UICGeneratorPropertyViewPermission` (readonly/visibility from validators & `IUICPermissionService`), `UICGeneratorDataAnnotationValidators` (wires `[Required]`, `[MinLength]`, etc. into `Validation...` properties when `CheckClientSideValidation` is on), `UICFakeForeignKeyTypeGenerator` (resolves `[FakeForeignKey]` target type), `UICGeneratorInputEditorTemplate` (routes to a Razor `EditorTemplates/*.cshtml` when one exists for the type).

### Form-button generation conditions
Each button generator only runs for its matching `CallType` and is skipped entirely if a higher-priority generator already produced a non-null result:

- **Create** (`UICGeneratorButtonCreate`) — requires `IUICPermissionService.CanCreateType(type)` to pass (if a permission service exists) **and** a `UICForm` with a submit action to be found in the ancestor chain; produces a `UICButtonSave`-styled button labelled "Create" with class `btn-create`.
- **Save** (`UICGeneratorButtonSave`) — requires `CanEditObject(obj)` permission **and** a submit-capable form; labelled "Save", class `btn-update`.
- **Delete** (`UICGeneratorButtonDelete`) — if `ClassObject` is null, always creates a generic `UICButtonDelete`; otherwise requires `CanDeleteObject(obj)` permission. Uses `PostFullModelOnDelete` to decide between posting the full object or just `IDbEntity.Id` (falls through to `Next()`/no button if the object isn't an `IDbEntity` and full-model posting is off).
- **EditReadonly** (`UICGeneratorButtonEditReadonly`) — always produces a `UICButtonEdit`; wires its readonly-toggle click to respect `HideEmptyInReadonly`.
- **Cancel** (`UICGeneratorButtonCancel`) — always produces a plain `UICButtonCancel`.
- **Toolbar** (`UICGeneratorButtonToolbar`) — only runs if a `UICForm` exists among the rendered components. Builds the ordered button list from `Options.ButtonOrder` (defaulting missing `delete/cancel/edit/save` to the end), resolves each via `Options.ButtonGenerators[key]`, and places each per its `...ButtonPosition` (falling back to `ButtonPosition`, then the toolbar default). The `edit` slot additionally hooks `uic-afterSubmit` to re-apply readonly after a successful save; the `save` slot wires `UICActionDisableSaveButtonOnValidationErrors` when `DisableSaveButtonOnValidationErrors` is set.

`ShowEditButton`(default `true`), `ShowDeleteButton`(default `true`), `ShowCancelButton`(default `false`) gate whether each slot's function even attempts generation.

### `OptionDefaults` — global startup overrides
Static fields on `UIComponents.Defaults.OptionDefaults` (or the generator-side `UIComponents.Generators.Defaults.OptionDefaults`, mirrored 1:1 with `UICOptions`) that seed every new `UICOptions` instance. Set these once during app startup to change behavior app-wide instead of passing `UICOptions` on every call: `CheckReadPermissions`/`CheckWritePermissions` (both `true`), `CheckClientSideValidation` (`true`), `DisableSaveButtonOnValidationErrors` (`true`), `FormReadonly`/`FormToolbarInCardFooter`/`HideEmptyInReadonly`/`HideReadonlyProperties` (all `false`), `HideId` (`true`), `ExcludedProperties` (empty), `IncludedUndefinedProperties` (`false`), `InputGroupSingleRow` (`true`), `MarkLabelsAsRequired` (`true`), `NoForm`/`PostIdAsFixed`/`PostObjectAsDefault`/`ReplaceSaveButtonWithCreateButton`/`SelectlistAddEmptyItem`/`SelectListShowAddButtonIfAllowed`/`PostFullModelOnDelete` (all `false`), `SelectlistSearchableForItems` (`10`), `ShowCancelButton` (`false`), `ShowCardHeaders` (`false`), `ShowDeleteButton`/`ShowEditButton` (`true`), `ButtonDistance` (`Medium`), `ButtonOrder`/`ButtonPosition`/`EditButtonPosition`/`DeleteButtonPosition`/`CancelButtonPosition`/`SaveButtonPosition` (`null`), `ButtonGenerators` (empty dictionary), `ToolbarPosition` (`BelowForm`), `CheckboxColor` (`null` factory), `OnSuccessfullSubmit` (empty `UICCustom` factory), `StartInCard` (`null`), `SubClassesInCard` (a fresh default `UICCard`).

### Additional built-in validators
Beyond `UICValidatorRequired`, `UICValidatorRangeAttribute<T>`, and `UICValidatorEditPermission` already documented:
- **`UICValidatorReadonlyAttribute`** (`IUICPropertyValidationRuleReadonly`) — marks a property readonly if `[ReadOnly(true)]` is present.
- **`UICValidatorMinMaxOfType<T>`** (`IUICPropertyValidationRuleMinValue<T>`, `IUICPropertyValidationRuleMaxValue<T>`) — registered per integer/decimal type (`byte, short, int, long, ushort, uint, ulong, decimal`); clamps the input's min/max to that CLR type's own `MinValue`/`MaxValue` even with no `[Range]` attribute present (e.g. a bare `byte` property automatically gets client-side max `255`).
- **Custom delegate-based validators** (`CustomValidatorPropertyRequired/MinValue<T>/MaxValue<T>/MinLength/MaxLength/Readonly`) — the plain-function implementations backing `AddValidatorProperty*` config calls; each wraps a `Func<PropertyInfo, object, CancellationToken, Task<...>>` you supply inline, no class needed.
- **`DefaultCheckValidationErrors<TRule>`** family — the fallback `IUICDefaultCheckValidationErrors<...>` implementations registered by `AddDefaultValidationErrorHandlers`, providing the actual server-side error-message generation (localized via `IUICLanguageService`) for every rule interface, so a bare `IUICPropertyValidationRuleRequired` implementation (with no `IUICPropertyValidationValidationResultsImplementation`) still produces a sensible validation message.

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

# Services

## Available services
ready-to-use, pre-made, impleme,ntet, ... services

### IUIComponentGenerator
This service can auto generate components based on the existing generators.

You can inject the **IUIComponentGenerator** directly in a view, controller or service.
```c#
private readonly IUIComponentGenerator _uic;

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


### IUICValidationService
This service is available, and uses all implementations of **IUICPropertyValidationRule**.

If there are multiple implementations are conflicting, the most exact value will be used.
Validation Min value => one returns 0, other returns 10 => 10 will be set
Validation Max value => one returns 20, other returns 50 => 20 will be set

#### Usage
This service is automatically used by the **IUIComponentGenerator** to set properties as required, assign minimum and maximum values.
This service can also be used inside a **AbstractValidator< T >**. This will check all the availalble validationrules and handle the messages.


```c#
public class TestModelValidator : AbstractValidator<TestModel>
{
	private readonly IUICValidationService _validationService;
	private readonly IUICILanguageService _languageService;

	public TestModelValidator(IUICValidationService validationService, IUICILanguageService languageService)
	{
		_validationService = validationService;
		_languageService = languageService;

		//This method requires a AbstractValidator (this) and a implementations of the IUICILanguageService
		_validationService.ValidateModel(this, _languageService);
	}
}
```

#### Available interfaces for IUICPropertyValidationRules
- IUICPropertyValidationRule< T > : ValidationRule that only works for this PropertyType (example: IUICPropertyValidationRule< int >)
- IUICPropertyValidationRuleRequired : Can check if a property is required
- IUICPropertyValidationRuleMinValue < TValue > : Assign a minimum value to a property
- IUICPropertyValidationRuleMaxValue < TValue > : Assign a maximum value to a property
- IUICPropertyValidationRuleMinLength : Assign a minimum length to a string
- IUICPropertyValidationRuleMaxLength : Assign a maximum length to a string
- IUICPropertyValidationRuleReadonly : Mark a property as readyonly

#### Predefined validationRules
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

#### UICValidatorRequired : IUICPropertyValidationRuleRequired
Set readonly if **Readonly** attribute is set and has value true.

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

#### Adding custom ValidationRules


#### Creating service on interface
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

#### Create a validator without a service or interface
You can also create validators without creating a new class that implements the interface.
Use one of the **AddValidatorProperty..** methods in the configuration.
This way DOES NOT have support for dependency injection and always uses the default **DefaultValidationErrors** method.
```c#
config.AddValidatorPropertyRequired((propertyInfo, obj) =>
{
	...
});
```

#### Example for implementing custom attributes for validation
```c#
config.AddValidatorPropertyMinLength((propinfo, obj) =>
	{
		var minLengthAttr = propinfo.GetInheritAttribute<MinLengthAttribute>();
		if (minLengthAttr == null)
			return Task.FromResult<int?>(null);
		return Task.FromResult<int?>(minLengthAttr.Length);
	});
```




### IUICStoredComponents
You can use this interface if you want to send something to a user (F.e. a complex notification or popup).
This service is also used by the **IUICQuestionService**.

#### Receiving notification on page load
When loading a page, you can check if there are any remaining notifications for the user, and display these notifications.
```c#
var allNotifications = _uicStoredComponents.GetComponentsByUser(currentUserId);
```
:warning: If a component is stored as single use, this will automatically be removed from the collection.
This means you should never call this method if you do not intent on sending these notifications to the user.
The .InvokeAsync(Component) method does support lists or null to render

### IUICQuestionService
You can use the IUICQuestionService if you want to ask questions to the client before continuing in code.
:warning: The questionservice does require the **IUICSignalRService** service to be implemented by the user!





## Register services developer
optional, todo, unimplemented, ... services

### IUICILanguageService
To enable translations, Implement the **IUICILanguageService** and make sure this is also registrated as this type.
```c#
builder.Services.AddScoped<IUICILanguageService, ILanguageService>();
```
If you do not wish to use this service, disable the check in the builder configuration.

Without a languageService, all [Translatables](#translatable) will take the last part of the key as defaultValue (split by ".")
```c#
builder.Services.AddUIComponentWeb(config =>
{
	config.CheckILanguageServiceType = false;
	...
});
```
#### Clientside translation
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


### IUICPermissionService
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

### IUICDefaultCheckValidationErrors&lt;IUICPropertyValidationRuleReadonly&gt;
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

### IUICSignalRService
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

# Web Extension Methods

## UICBuilderExtensions

Startup/DI wiring for the web layer, on top of what `UIComponents.Generators` provides.

```c#
public static IServiceCollection AddUIComponentWeb(this IServiceCollection services, Action<UicConfigOptions> config);
public static IApplicationBuilder MapUIC(this IApplicationBuilder app, string localPath);
```

`AddUIComponentWeb` calls `AddUIComponent` internally, registers `RecurringDateModelBinderProvider` as an MVC model binder, and — depending on the `UicConfigOptions` passed in (`ReplaceScripts`, `ReplaceCss`, `ReplaceComponents`, `ReplaceTaghelpers`, `AddReadMe`, `AddChangeLog`, `AddFileExplorerImgs`, `AddTranslationFile`, `OnlyReplaceNewerVersion`) — unpacks the library's embedded scripts, styles, `.cshtml` component views, tag helpers, README, changelog, file-explorer images and translation file into the host project's `wwwroot`/`UIComponents` folders on startup. It also writes a `UIComponents/Version.md` marker used to skip re-copying files when `OnlyReplaceNewerVersion` is set and the version hasn't changed.

`MapUIC` maps an embedded static file provider (the `Root` manifest resources) so UIComponents' own assets can be served directly from the assembly.

Usage:

```c#
builder.Services.AddUIComponentWeb(options =>
{
    options.ReplaceScripts = true;
    options.ReplaceCss = true;
    options.ReplaceComponents = true;
});
```

> :warning: `AddUIComponentWeb` performs file-system writes (creating/overwriting files under the app's working directory) as a side effect of DI registration — call it once at startup, not per-request.

## UICExtensions

Rendering helpers used from Razor views/`.cshtml`, mostly around invoking `IUIComponent`s from an `IViewComponentHelper` and working with html attribute dictionaries.

```c#
public static Task<IHtmlContent> InvokeAsync(this IUIComponent? UIC, IViewComponentHelper component);
public static Task<IHtmlContent> InvokeAsync<T>(this Task<T> UIC, IViewComponentHelper component) where T : IUIComponent;
public static Task<IHtmlContent> InvokeAsync<T>(this IEnumerable<T> UIC, IViewComponentHelper component) where T : IUIComponent;
public static Task<IHtmlContent> InvokeAsync<T>(this Task<List<T>> UIC, IViewComponentHelper component) where T : IUIComponent;
public static Task<IHtmlContent> InvokeAsync(this IViewComponentHelper component, params IUIComponent[] UIC);
public static Task<IHtmlContent> InvokeAsync(this IViewComponentHelper component, IEnumerable<IUIComponent> UIC);

public static string GetHtmlAttributes(this Dictionary<string, string> dictionary);
public static string GetHtmlAttributes(this IUICHasAttributes component);
public static IHtmlContent GetAttributesFromDictionary(this IHtmlHelper htmlHelper, Dictionary<string, string> dictionary);

public static void AddAttribute(this IEnumerable<IUIComponent> actions, string attribute, string value);
public static void SetIdentifier(this IUICHasAttributes action, string identifier);
```

Usage (typical Razor view pattern):

```cshtml
@await myComponent.InvokeAsync(Component)
```

> :bulb: The `InvokeAsync` overloads exist for a single component, an awaited task of a component, a collection of components, and an awaited task of a list — so you rarely need to manually loop or await before invoking; call `.InvokeAsync(Component)` directly on whatever you have.

Also included: `AssignCollectionForChildren` / `RenderStylesAndScripts` overloads, which propagate an `IUICScriptCollection` down to child components and render the combined `<style>`/`<script>` tags collected from a component tree. These are invoked internally by component views to flush per-request styles/scripts once at the root of a render.

## TranslateExtensions

Helpers for safely emitting translated text into JavaScript or HTML contexts from a Razor view.

```c#
public static string Encode(string text, char? brackets = null);
public static IHtmlContent JsEncode(this IHtmlHelper htmlHelper, string text, string? brackets = null);
public static Task<IHtmlContent> TranslateJs(this IHtmlHelper htmlHelper, IUICLanguageService languageService, Translatable translateable, string brackets = "'");
public static Task<IHtmlContent> TranslateHtml(this IHtmlHelper htmlHelper, IUICLanguageService languageService, Translatable translatable, string brackets = null);
```

`Encode` runs text through `JavaScriptEncoder` so it is safe to embed inside a JS string literal, optionally wrapping it in the given bracket character (e.g. `'`). `TranslateJs` combines a language-service lookup with that JS-safe encoding; `TranslateHtml` does the same but writes raw (unencoded) HTML instead, for translations that legitimately contain markup.

Usage:

```cshtml
var message = @await Html.TranslateJs(LanguageService, myTranslatable);
```

> :warning: Use `TranslateJs` for anything interpolated into inline `<script>` blocks, and `TranslateHtml` only for trusted translation content — `TranslateHtml` deliberately skips HTML-encoding.

## WebExtensions

Small Razor/JS interop helpers.

```c#
public static IHtmlContent ToMoment(this DateTime dateTime, IHtmlHelper htmlHelper);
public static IHtmlContent UICHelp(this IHtmlHelper htmlHelper, UIComponent model, string content);
public static IHtmlContent Conditional(this IHtmlHelper htmlHelper, bool condition, RazerBlock razerBlock);
public static IHtmlContent Conditional(this IHtmlHelper htmlHelper, bool condition, string content);
```

`ToMoment` renders a `DateTime` as a `moment("yyyy-MM-dd HH:mm:ss", moment.ISO_8601)` JS expression for use with moment.js. `UICHelp` wires up a `uic-help` jQuery event on a component's element that logs debug info to the browser console when triggered. `Conditional` renders either the given content/`RazerBlock` or nothing, based on a boolean — a compact alternative to an `@if` block inline in an expression chain.

## IUICValidatorExtensions

Bridges the library's own `IUICValidationService`/attribute-based validation into FluentValidation, and turns validation results (from either ModelState or FluentValidation) into the wire format the client-side script expects.

```c#
public static void ValidateModel<T>(this IUICValidationService validationService, AbstractValidator<T> validator, IUICLanguageService languageService);
public static void ValidateModelAsync<T>(this IUICValidationService validationService, AbstractValidator<T> validator, IUICLanguageService languageService);

public static IActionResult ValidationErrors(this Controller controller, FluentValidation.Results.ValidationResult validationResult = null);
public static UICValidationErrors ValidationErrors(this ModelStateDictionary ModelState);
public static UICValidationErrors ValidationErrors(this FluentValidation.Results.ValidationResult ModelState);
```

`ValidateModel`/`ValidateModelAsync` walk every property of `T` and add a FluentValidation `RuleFor(...)` that delegates to `IUICValidationService.ValidateObjectProperty`, so any `[UICValidate...]`/`IUICPropertyValidationRule` attributes on the model are enforced through the same `AbstractValidator<T>` pipeline. `ValidationErrors` converts either a FluentValidation `ValidationResult` or a controller's `ModelState` into a `UICValidationErrors` payload (a `PropertyName`/`Error` list plus the request `Url`) and — when called on a `Controller` — directly returns it as a `Json` `IActionResult`.

Usage (matches `HomeController.Post()`):

```c#
var validation = await _validator.ValidateAsync(post);
if (!validation.IsValid)
    return this.ValidationErrors(validation);
```

> :bulb: `controller.ValidationErrors()` with no argument falls back to `controller.ModelState`, so it also works for plain data-annotation/model-binding failures without a FluentValidation result in hand.

# Controllers

## UICController

The generic AJAX endpoint used by rendered components to talk back to the server for stored components/events. Route follows default MVC conventions (controller name `UIC`).

```c#
public class UICController : Controller
{
    [HttpPost]
    public Task<IActionResult> PostEvent(string key, Dictionary<string, string> values, bool ignoreKeyNotFound);

    [HttpGet]
    public IActionResult GetComponent(string key);

    public IActionResult ViewOrPartial(IUIComponent component);
}
```

- `PostEvent` (`POST /UIC/PostEvent`) forwards a client-triggered event, identified by `key` plus a payload dictionary, to `IUICStoredEvents.IncommingSignalRTrigger`.
- `GetComponent` (`GET /UIC/GetComponent`) looks up a previously stored `IUIComponent` by `key` via `IUICStoredComponents` and renders it through `/UIComponents/ComponentViews/Render.cshtml`.
- `ViewOrPartial` picks between a full `View` and a `PartialView` of `Render.cshtml` depending on whether the incoming request is an AJAX request (detected via the `X-Requested-With: XMLHttpRequest` header).

> :bulb: `GetComponent`/`PostEvent` are what backs components that need to round-trip state to the server (e.g. re-render after a server-side event) without a full page reload — the `key` correlates the client-side element to a server-stored `IUIComponent`.

## UICFileExplorerController

`UICFileExplorerController` (in `UIComponents.Web.Tests.Controllers`, implementing `IUICFileExplorerController`) is the backing controller for the File Explorer component, exposing actions such as `CopyFiles`, `CreateDirectory`, `DeleteFiles`, `Download`, `GetFilesForDirectoryPartial`, `GetFilesForDirectoryJson`, `MoveFiles`, `OpenFile`, `OpenImage`, `Preview`, `Rename`, `UploadPartial`, and `UploadFiles`. Full behavior of the File Explorer component and this controller is covered in its own dedicated section elsewhere in this README (see [FileExplorer](#fileexplorer)).

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

## UICIgnoreGetChildrenFunctionAttribute
Using this attribute will make the [Find... extension methods](#find-methods) ignore this property.
This is useful for properties that are a reference to a parent object, which would otherwise create a circular search.

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


## UICHtmlInputAttribute

Adds a raw HTML attribute directly on the rendered `<input>` element for a property. You can stack multiple instances on the same property since `AllowMultiple` is enabled.

```c#
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
public class UICHtmlInputAttribute : Attribute
{
    public UICHtmlInputAttribute(string attributeName, object attributeValue)
    public string AttributeName { get; set; }
    public object AttributeValue { get; set; }
}
```

Usage:

```c#
[UICHtmlInput("id", "myId")]
[UICHtmlInput("class", "blub")]
public bool Checkbox { get; set; }
```

> :bulb: Stack as many `UICHtmlInputAttribute` instances as you need — each one becomes a separate `name="value"` pair on the input.

## UICHtmlInputGroupAttribute

Same concept as `UICHtmlInputAttribute`, but the attribute is placed on the surrounding input-group wrapper element instead of the `<input>` itself. Useful for `data-*` attributes that a script needs to read from the group container rather than the input.

```c#
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
public class UICHtmlInputGroupAttribute : Attribute
{
    public UICHtmlInputGroupAttribute(string attributeName, object attributeValue)
    public string AttributeName { get; set; }
    public object AttributeValue { get; set; }
}
```

Usage:

```c#
[UICHtmlInputGroup("data-abc", 2)]
public bool Checkbox { get; set; }
```

## UICHtmlLabelAttribute

Same pattern again, this time targeting the `<label>` element generated for the property.

```c#
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
public class UICHtmlLabelAttribute : Attribute
{
    public UICHtmlLabelAttribute(string attributeName, object attributeValue)
    public string AttributeName { get; set; }
    public object AttributeValue { get; set; }
}
```

Usage:

```c#
[UICHtmlLabel("class", "my-label")]
public bool Checkbox { get; set; }
```

> :bulb: `UICHtmlInputAttribute`, `UICHtmlInputGroupAttribute`, and `UICHtmlLabelAttribute` all share the same constructor shape `(attributeName, attributeValue)` — they only differ in which rendered element they target (input, input-group wrapper, or label).

## PrependAppendInputGroupClass

A class-level attribute that overrides the default Bootstrap `input-group-text` class used for prepend/append text in input groups. Put it on the model class that contains the property being wrapped in an input-group.

```c#
[AttributeUsage(AttributeTargets.Class)]
public class PrependAppendInputGroupClass : Attribute
{
    public PrependAppendInputGroupClass(string setClass)
    public PrependAppendInputGroupClass(string prependClass, string appendClass)

    public string PrependClass { get; set; } = "input-group-text";
    public string AppendClass { get; set; } = "input-group-text";
}
```

Usage:

```c#
[PrependAppendInputGroupClass("my-custom-text-class")]
public class MyModel
{
    // properties with prepend/append input-groups use "my-custom-text-class"
    // instead of the default "input-group-text"
}

// Or set prepend/append independently:
[PrependAppendInputGroupClass("prepend-class", "append-class")]
public class MyOtherModel { }
```

## UICTooltipIconAttribute

Overrides the default tooltip icon used alongside `UICTooltipAttribute`. On its own it has no effect — it only changes the icon rendered when a tooltip is also present.

```c#
public class UICTooltipIconAttribute : Attribute
{
    public UICTooltipIconAttribute(string iconClass)
    public string IconClass { get; set; }
}
```

Usage:

```c#
[UICTooltip("Dit is een test")]
[UICTooltipIcon("fas fa-warning text-warning")]
public string TestString { get; set; }
```

> :warning: `UICTooltipIconAttribute` must be combined with `UICTooltipAttribute` on the same property — it customizes the icon but does not create a tooltip by itself.

## UICPrecisionDateAttribute

Controls how precise a `DateTime` or `DateOnly` property is rendered/edited — for example, restricting the date picker to date-only, or including minutes/seconds/milliseconds.

```c#
public class UICPrecisionDateAttribute : Attribute
{
    public UICPrecisionDateAttribute(UICDatetimeStep precision)
    public UICDatetimeStep Precision { get; set; }
}
```

`UICDatetimeStep` values: `Date`, `Minute`, `Second`, `Millisecond`.

Usage:

```c#
[UICPrecisionDate(UICDatetimeStep.Minute)]
public DateTime? MyDateTime { get; set; } = DateTime.Now;
```

## UICPrecisionTimeAttribute

The time-of-day counterpart to `UICPrecisionDateAttribute`. Controls the precision used when a `DateTime` or `TimeOnly` property is rendered as a time value — typically combined with `UICPropertyTypeAttribute(UICPropertyType.TimeOnly)` to force a `DateTime` property to render as a time input.

```c#
public class UICPrecisionTimeAttribute : Attribute
{
    public UICPrecisionTimeAttribute(UICTimeonlyEnum precision)
    public UICTimeonlyEnum Precision { get; set; }
}
```

`UICTimeonlyEnum` values: `Minute`, `Second`, `Milliseconds`.

Usage:

```c#
[UICPropertyType(UICPropertyType.TimeOnly)]
[UICPrecisionTime(UICTimeonlyEnum.Second)]
public DateTime? TimeOnly { get; set; }
```

# DataTypes

## RecurringDate

`RecurringDate` models a schedule made up of **included** and **excluded** date rules — think "every Monday, every 2 weeks, except this list of dates." It lives under `UIComponents.Abstractions.DataTypes.RecurringDates`.

```c#
public class RecurringDate
{
    public List<RecurringDateItem> Included { get; set; } = new();
    public List<RecurringDateItem> Excluded { get; set; } = new();

    public bool IsValidDate(DateTime date);
    public DateOnly? GetNextDate(DateTime? startPoint = null);
    public List<DateOnly> GetNextDates(int maxCount, DateTime? startPoint = null);

    public string Serialize();
    public static RecurringDate Deserialize(string serialized);
}
```

Each `RecurringDateItem` (in `RecurringDateItem.cs`) wraps a single rule with an `Enabled` flag, a `StartDate`/optional `EndDate` window, and a `Pattern` that implements `IRecurringDateSelector`:

```c#
public partial class RecurringDateItem
{
    public bool Enabled { get; set; } = true;
    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly? EndDate { get; set; }
    public IRecurringDateSelector Pattern { get; set; }

    public bool DateInRange(DateOnly date);
}
```

A date is considered valid overall when it matches at least one `Included` item and no `Excluded` item.

**Selector types** — the concrete implementations of `IRecurringDateSelector` under `DataTypes/RecurringDates/Selectors/`:

- **`RecurringWeekly`** — recurs every *N* weeks (`EveryXWeeks`) on any combination of `Monday` … `Sunday` boolean flags. `IsInvalid` is true when no weekday is selected.
- **`RecurringMonthly`** — recurs every *N* months (`EveryXMonths`) on a given `RecurringStyle` (day-of-week 0–6, or 10=Day, 11=WeekDay, 12=Weekend) combined with a `MonthlyInstance` (`First`, `Second`, `Third`, `Forth`, `Last`) describing which week of the month, e.g. "the last Friday of every month."
- **`RecurringCustomDate`** — an explicit list of `Days`, `Months`, and `Years` (any of which can be left empty to mean "any"), for one-off or fixed calendar patterns.

Every selector implements the same contract:

```c#
public interface IRecurringDateSelector : IUIComponent
{
    [UICIgnore]
    public bool IsInvalid { get; }

    public DateOnly? GetNextDate(RecurringDateItem dateItem, DateOnly start);
    public bool IsValidDate(RecurringDateItem dateItem, DateOnly date);

    public string Serialize();
    public IRecurringDateSelector Deserialize(string serialised);
}
```

`RecurringDate` itself is also serializable to a flat string (for storing in a single database column) and back:

```c#
var nextOccurences = post.RecurringDate.GetNextDates(30);
var serialized = post.RecurringDate.Serialize();
var deserialized = RecurringDate.Deserialize(serialized);
var nextOccurences2 = deserialized.GetNextDates(15);
```

> :bulb: `GetNextDates`/`GetNextDate` treat the given start date as inclusive — if `startPoint` itself matches a rule, it is returned as the first result.

> :warning: Invalid or disabled `Included` items (no pattern, or a selector whose `IsInvalid` is true) are silently skipped rather than throwing — `GetNextDates` can return fewer dates than `maxCount` (or an empty list) if the rules run out or none are valid.

### Model binding from form posts

Because `RecurringDate` is a nested collection of polymorphic selectors, the default MVC model binder cannot construct it from `Included[0][Pattern][...]`-style form fields. `UIComponents.Web` registers a dedicated `RecurringDateModelBinder`/`RecurringDateModelBinderProvider` (via `AddUIComponentWeb`) that reconstructs `Included`/`Excluded` `RecurringDateItem`s — including resolving the correct `Pattern` type from the posted `PatternType` field — directly from the submitted form collection. This means a `RecurringDate` property on an action parameter binds automatically from a standard form post; no manual parsing is required in the controller.

## ValueRange<T>

A simple generic "from/to" range container, e.g. for numeric or date range filters/inputs.

```c#
public class ValueRange<T> : IValueRange<T> where T : IComparable
{
    public ValueRange() { }
    public ValueRange(T from, T to)

    public T From { get; set; }
    public T To { get; set; }

    // Start/End are aliases for From/To
    public T Start { get; set; }
    public T End { get; set; }
}
```

Usage:

```c#
var range = new ValueRange<int>(1, 10);
var alsoRange = new ValueRange<DateOnly> { Start = DateOnly.MinValue, End = DateOnly.MaxValue };
```

> :bulb: `Start`/`End` and `From`/`To` are the same underlying values under different names — use whichever reads better at the call site.

## UICReferenceValues<T>

`UICReferenceValues<T>` is a strongly-typed bag that holds a subset of *another* model's property values, addressed by name, without requiring a hard reference to an actual instance of `T`. This is useful when a view model needs to remember "these particular properties of `TestModel2`" (for display, comparison, or partial patching) without carrying the whole object around.

```c#
public class UICReferenceValues
{
    public IReadOnlyDictionary<string, object> PropertyValues { get; }

    public UICReferenceValues AssignProperties(params string[] properties);
    public object GetPropertyValue(string propertyName);
    public UICReferenceValues SetPropertyValue(string propertyName, object value);
    public virtual UICReferenceValues SetValueInReference(object sourceObject);
    public virtual UICReferenceValues SetValueInSource(ref object sourceObject);
}

public class UICReferenceValues<T> : UICReferenceValues where T : class
{
    public UICReferenceValues<T> AssignProperties(params Expression<Func<T, object>>[] expressions);
    public TValue GetPropertyValue<TValue>(Expression<Func<T, TValue>> expression);
    public UICReferenceValues<T> SetPropertyValue<TValue>(Expression<Func<T, TValue>> expression, TValue value);
    public virtual UICReferenceValues<T> SetValueInReference(T sourceObject);
}
```

`AssignProperties` is the fluent entry point: it registers which properties of `T` you want to track (values start out `null`). `SetValueInReference` later reads those property values (via reflection) from a real instance of `T` into the bag; `SetValueInSource` writes them back onto a target instance.

Usage:

```c#
public UICReferenceValues<TestModel2> References { get; set; } =
    new UICReferenceValues<TestModel2>().AssignProperties(x => x.TestModel2Bool);

// later, capture the live value from a real TestModel2 instance:
References.SetValueInReference(someTestModel2Instance);
bool value = References.GetPropertyValue(x => x.TestModel2Bool);
```

> :bulb: Use the generic `Expression<Func<T, object>>` overload of `AssignProperties`/`GetPropertyValue`/`SetPropertyValue` for compile-time-checked property names instead of the base class's raw-string overloads.

> :warning: `GetPropertyValue<TValue>` throws if the value was never assigned (still `null`) and cannot be cast — check `PropertyValues` or catch accordingly if the reference hasn't been populated yet.

# Logging
The services use ILogger for logging.
You can set the minimum loglevel in appsettings.
Loglevels can be seperated on namespaces.
Used logLevels:
- Trace : Very low loglevel, logs every generator or validator that is used
- Debug : Usefull to locate why generators or validators do something in a certain way
- Info : Usefull logs outside of debug, example: log the responses to **IUICQuestionService**
- Error : All errors and exceptions that may occur. Recommend to always see these errors
```json
"Logging": {
	"LogLevel": {
	  "UIComponents" : "Information",
	  "UIComponents.Generators.Generators": "Information",
	  "UIComponents.Generators.Services": "Information",
	}
  },
```
