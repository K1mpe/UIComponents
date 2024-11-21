# UICComponents Documentation


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
    - [IUIAction](#IUIAction)
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

    - [IUIComponentGenerator](#IUIComponentGenerator)
    - [IUICValidationService](#iuicvalidationservice)
    - [IUICStoredComponents](#iuicstoredcomponents)
    - [IUICQuestionService](#iuicquestionservice)

  </details>
  <details>
    <summary>Implementable Service-Interfaces</summary>

    - [IUICILanguageService](#iuiclanguageservice)
    - [IUICPermissionService](#iuicpermissionservice)
    - [IUICDefaultCheckValidationErrors&lt;IUICPropertyValidationRuleReadonly&gt;](#iuicdefaultcheckvalidationerrors)
    - [IUICSignalRService](#iuicsignalrservice)

  </details>
- [Attributes](#attributes)
- [Logging](#logging)

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
Create items manually or use the [IUIComponentGenerator](#IUIComponentGenerator) to generate components.

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
There is also a taghelper for rendering components. If your component supports [IUICHasAttributes](#IUICHasAttributes), attributes assigned to the component are applied to the component
```cshtml
<uic c="myComponent" class="my-custom-class"></uic>
```
If this component implements the [IUICSupportsTaghelperContent](#IUICSupportsTaghelperContent), you can also write content within the tags.
> For mor info, see [UICTaghelper](#uictaghelper)



## Rendering components from controller
```c#
public IActionResult RenderMyComponent()
{
	var myComponent = …
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

## Models
### UICCustom

This is a custom component that can be used as [IUIComponent](#iuicomponent), [IUICAction](#iuiaction) or [IDropdownItem](#idropdownitem).

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

To Translate these objects, you need to implement the [IUICILanguageService](#iuiclanguageservice) interface.

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

When creating a form, you can set the whole object as DefaultData and the [UICGetValue](#uicgetvalue) as **GetVariableData**. All properties that are not rendered will still be included in the post, but the rendered properties will overwrite these values.
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
