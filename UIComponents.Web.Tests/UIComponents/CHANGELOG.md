# 2025-09-02 (V1.4.0)
- UICQuestions in modals instead of SweetAlert
- FileExplorer permissions split for creating folder & uploading file
- FileExplorer formatting DateTime
- FileExplorer validate folder exists on folder create

# 2025-08-08 (V1.3.29)
- Fix contextmenu in FileExplorer
- QuestionText supports multiline text & clientside validation

# 2025-07-09 (V1.3.28)
- Implementing events sidepanel

# 2025-06-03 (V1.3.27)
- IUICSupportsTaghelperContentPassThrough
- UICSidepanel now supports TaghelperContent
- uic.getValue on select2 parses numbers if possible
- Propwatcher performance

# 2025-05-19 (V1.3.26)
- UICSignalRHub
- IFormattedException
- ValidationService gebruikt huidige scope
- Fix UICTable
- Fix nowIndicator in TimelineGraph
- Catch in getpost.js

# 2025-04-30 (V1.3.25)
- table localDelete for firstItem fix
- fix uic.markChanges

# 2025-04-29 (V1.3.24)
- uic.parse supports arrays
- minor css fix buttontoolbar
- form can now block submitbuttons (blocks by default)
- getting data from table will return valid value before jsgrid is completely loaded
- Recurringdate does crash when deserialising from empty string

# 2025-04-11 (V1.3.23)
- ToString op TaskResult
- BugFix Table ReloadCondition

# 2025-04-01 (V1.3.22)
- RecurringDate can explicit convert from and to string
- UICInputTable -> use a table as a input
- UICTable Fixes when using local data
- Fix Partial reload

# 2025-02-19 (V1.3.21)
- Condition for UICTable button
- UICGraph fix for graph loaded as disabled
- UICTable center cell no padding

# 2025-01-28 (V1.3.20)
- Table infinite loading triggers 'uic-dataLoaded
- css tabs in tabs
- Table column button supports async function on click

# 2025-01-22 (V1.3.19)
- Css Mixins
- Minor fixes
- Fix UICGeneratorInputMultiline permission check
- Fix UICOptions.FormReadonly
- Select2 set value on change
- Sidepanel add class to sidebar-header-toolbar
- partial.js replace triggerHandler('uic-reloaded') to trigger('uic-reloaded')
- UIC Taghelper does not invoke by default, except when using the "i" property

# 2025-01-13 (V1.3.18.1)
- Fix float min-max val
- Minor Fixes

# 2025-01-09 (V1.3.18)
- Functions for referenced defaults instead of plain objects.
- Fix for Min / Max double values as validation

# 2025-01-06 (V1.3.17)
- IconDefaults for create, edit button etc
- Generator methods to create a button
- Adding Synchronous Generators in GeneratorHelper

# 2024-12-18 (V1.3.16)
- Support for ushort, uint and ulong
- Validation on numbers Min & Max values


# 2024-12-12 (V1.3.14)
- Fix required

# 2024-12-11 (V1.3.13)
- TranslationDefaults Support FluendProperties
- TranslateProperty support MemberInfo
- Fix Validation Required
- ScriptCollection Outside doc ready
- TranslatableSaver does not save in a single line anymore
- Option to ignore clientside validation
- Table has CanInsert, CanUpdate, CanDelete
- ToastR
- ButtonPositions can be selected individual
- ButtonOrder can be selected
- Custom buttons can be added


# 2024-11-28 (V1.3.12)
- Option to enable clientside validation in generators
- Adding scripts without $(document).ready(()=> ...)

# 2024-11-28 (V1.3.12)
- Single-row now has css variables
- IUICAskUserToTranslate


# 2024-11-21 (V1.3.11)
- UICTabs Singletab now accounts for unrendered tabs
- Css fix for .btn-toolbar
- UICTaghelper
- Minor css fix for tabs in tabs


# 2024-11-14 (1.3.10)
- Fix responses in partial
- Minor fix in file-explorer
- File-explorer img-viewer now supports skipping files & arrow buttons

# 2024-11-13 (1.3.09)
- Minor fix sorting jsgrid
- loading script from javascript
- loading style from javascript
- Css rework
- File-Explorer contextmenu support in jstree


# 2024-11-08 (1.3.08)
- Minor fix selectlistitems from htmlstorage
- Minor fix on disposing partial


# 2024-11-07 (1.3.07)
- Adding readonly on edit permission
- GroupGenerator can handle multiple tasks async
- UICTooltipAttribute support for jsGrid table
- Minor SignalR fix
- Fix UICTable Editing Date
- Some minor changes in UICTable

# 2024-10-25 (1.3.04 -> 1.3.06)
- SignalR Fixes

# 2024-10-25 (1.3.03)
- Partial is disposable
- preventing multiple signalR subscriptions
- prevent empty scripts
- bugfix delayed action


# 2024-10-17 (1.3.02)
- Bugfix Input list with array
- GeneratorHelper.PropertyInputGenerator added
- Renderconditions now take itself as argument
- UICForm checks for conflicts before submitting form
- TranslatableSaver included in javascript

# 2024-10-17 (1.3.00)
- Rename FetchComponent -> UICFetchComponent
- Rename ValidationErrors -> UICValidationErrors
- ValidationErrors supports checking the form Url
- Setting event after UICInputList is ready loading data
- JsGrid uses a full DelayAction instead of only a timer
- DelayedAction Only works when miliseconds is over 0, else it executes the action instantly
- Most default implementations have there methods now as Virtual
- FileExplorer in usable state
- Translations.xml changed to Translations.json
- Fix Input list initialised without items
- markchanges can now overwrite the clone function by using $('#id').on('uic-getClone', ()=>...)
- IQuestionService async

# 2024-10-10 (1.2.32)
- Fix dependency IUICValidatorService <-> IUIComponentGenerator
- Progress on fileExplorer
- Readme included in nuget

# 2024-10-08 (1.2.31)
- UIComponents.Defaults.RenderDefaults.OverwriteRenderLocation is possible
- Extension methods for Logger.BeginScopeKvp
- IValidationService available as property in UICConfig
- Adding UICOption Defaults
- UICOption for toolbar placement
- Hide readonly properties available as option
- UICActionGoBack can force reload of previous page
- Fix for select2 Placeholder item
- Css singleRow fix
- Form has events on Editing / readonly toggle

# 2024-09-27 (1.2.30)
- DelayedAction Changed
- BugFix Permissions
- Missing Namespace UIComponents.Abstractions
- Fix UICQuestionService

# 2024-09-24 (1.2.29)
- Mark Changes updated to modal
- Bugfix subcards
- IUICLanguageService has function to translate a object

# 2024-09-16 (1.2.28)
- InputSelectListHtmlStorage => Init with old data for faster response
- Multiline change lineheight on change
- InputGroup does not support singleRow if label or input is not rendered
- SingleRow has Support for defaults

# 2024-09-13 (1.2.27)
- Fixes for ChangeWatcher
- Fix for uic.compareObject
- UIC-Help is grouped and most of the code moved to javascript
- UICPartial moved to javascript & support for conditional loading
- Adding UICTooltipIcon attribute


# 2024-09-04 (1.2.26)
- Better stackoverflow message in IUIComponent.GetAllChildren()
- UICIgnoreGetChildrenFunctionAttribute now works on classes as well as properties
- Fix Changelog file

# 2024-09-03 (1.2.25)
- BugFix Multiline input new line in form that prevents submit on "enter"
- UICTable fix infinite scroll when scrollbar is a part of the page and not the full page
- UICTable remove the scrollbar when height is set to "auto"
- UICPartial support for Url
- UICPartial does not render Json(null)


# 2024-08-29 (1.2.24)
- BugFix StartInCard => Render = false

# 2024-08-20 (1.2.22)
- Adding UICCarousel

# 2024-08-07 (1.2.21)
- Fix UICTagHelper constructor
- Adding UICCached
- Bugfix UICButton => UICDropdownItem
- UICHtmlStorage

# 2024-07-23(1.2.20)
- Adding UICTaghelper Component
- UICTable .triggerHandler('uic-currentData')
- UICTable .triggerHandler('uic-getLastFilters')
- UICTable Infinite loading

# 2024-07-19 (1.2.19)
- Fix readonly InputList
- Fix dropdown buttons 
- GetPost uses Raw Url
- Fix parsing nullable enum

# 2024-07-15 (1.2.17)
- Bugfix UICInputSelectList
- Vertalingen aanpassen van NL-BE naar NL

# 2024-07-12 (1.2.16)
- Cannot find children in UICInputList anymore
- Performance fix for select2 selector
- UICInputSelectList has color support
- Toggle disabled in jsGrid outside edit row
- UICInputGroup InputAs options
- Css Fix jsGrid selecting togglebutton
- UICForm can prevent post on Enter Click
- UICTable fix filtering on date

# 2024-06-20 (1.2.15)
- Helpers for SelectListitems
- TranslatableSaver

# 2024-06-13 (1.2.13)
- JsGrid Control can filter edit or delete button
- Fix selectlist items for nullable enum

# 2024-06-10 (1.2.11)
- Select list renderer voor UICInputTime

# 2024-06-06 (1.2.10)
- Fix filter compabiliteit UICTable met oude JsGrid taghelper van CdcPortal

# 2024-05-31 (1.2.09)
- UICTable rowrenderer fix
- UICActionRefresh fix
- UICTableColumnPartial validation fix
- Fix Recurring date modelbinder
- bugfix UICSignalR this variable

# 2024-05-29 (1.2.08) 
- UICTableColumn checks for readonly validation
- UICTable automatically adds 'item' as variable for getpost in Insert, update or delete
- Adding a reload delay for UICTable
- Tabs will change when Url hash changes
- Bugfix: Implement TriggerReload on partial
- Bugfix: UICGroup render script collection
- UICTable control column

# 2024-05-22 (1.2.07)
- Bugfix: UICGeneratorPropertySetReadonly where inherit property does not have set or get

# 2024-05-17 (1.2.06)
- Property without get parameter are always readonly
- Tabs in tabs improvement. Support for diffrent tabstyles


# 2024-05-16 (1.2.05)
- CSS fixes tabs in tabs
- Defaults for UICTable
- Partial will reload on card or tab open if reload trigger was ignored when closed.
- UICOptions now has OnSuccessfullSubmit function


# 2024-05-14 (1.2.04)
- Fix navigatieknop in card buttons
- JsGrid pointer cursor on row click
- Fix exception _Scripts.UIC.cshtml
- Verwijderen reference SixLabors


# 2024-05-06 (1.2.03)
- UICActionMarkChanges does not require UIComponent, can also be a provided selector.
- Fix uic.translations
- Fix column visibility in UICTable
- Fix partial in UICTable

# 2024-05-02 (1.2.02)
- Fixes UICTable
- Support for tooltip & infospan in UICTable

# 2024-04-26 (1.2.00)
- Css Fix Readonly form textarea
- Add IUIComponentViewModel and UIComponentViewModel
- Fixes JsTree refresh
- UICFactory implemented
- Adding UICRedirectJson object
- Minor Fix UICActionRefresh
- Adding 2 permissions to IUICPermissionService
- Scriptloading changed
- UICTable component (JsGrid)

# 2024-04-15 (1.1.30)
- UICInputList support for readonly


# 2024-04-12 (1.1.29)
- Introducing IUICInitializeAsync
- await FormSubmit
- Adding changeWatcher in web


# 2024-04-11 (1.1.27)
- Bugfix sorting selectlistsource empty text
- Input Text Inherit DataType
- Fix don't show tabs that don't render content
- Label tooltip icon clickable for touch
- FileExplorer in progress



# 2024-04-02 (1.1.26)
- BugFixes Validator
- Support for ReadonlyAttribute
- Disable / Readonly support for Timespan



# 2024-04-02 (1.1.25)
- Bugfix permission
- InputTimeOnly



# 2024-03-28 (1.1.27)
- Fix mark changes
- Renderlocation can now end with .cshtml  (automatically add .cshtml if the last 7 characters do not contain a point)
- TryFind... extension methods
- Adding UICCustomTaghelper
- Including extra documentation

# 2024-03-28 (1.1.28)
- Bugfix non-UIC tabs
- Bugfix InternalHelper

# 2024-03-25 (1.1.23)
- Hide Sidepanel if no sidepanelcontent is available
- Fix Actions for Select2 input
- Always include defaultValidationErrorHandles in registration
- Generator for foreignKey type

# 2024-03-25 (1.1.22)
 - Adding Validators (See README.md)
 - Fix DeleteButton
 - Fix ContextMenu
 - Adding ListInput

# 2024-03-20 (1.1.21)
 - Fix stackoverflow

# 2024-03-20 (1.1.20)
 - Support for EditorTemplates / UIHint attribute

# 2024-03-20 (1.1.19)
- Fix for autogenerating translationkey for tooltip or infospan on properties with a displaynameattribute
- Css fixes (Input-group && swal)
- Defaults for UICModal
- Renaming SignalR Properties
- UICActionGetPost support for default data and dictionaries
- Overwritable renderlocation for datetime & defaults
- Serialize Id in UICButtonDelete



# 2024-03-15 (1.1.18)
 - Fixing bug of stackoverflow in subcards

# 2024-03-15 (1.1.17)
 - Fixing bug in getting value from empty object

# 2024-03-15 (1.1.16)
- Beter translation parsing for Html and Js
- Fix for autogenerating translationkey for tooltip or infospan on properties with a displaynameattribute
- Support for submodels as properties


### Adding defaults
- UICButtonToolbar



# 2024-03-03 (1.1.15)
- Bugfix: Namespaces with Render.cshtml
- ActionGetPost controller voor Url
- AddReadme option in config
- AddChangelog option in config
- Adding [AttributeUsage] to attributes
- Simplify IUICPermissionService
- :warning: Expects Url instead of controller in **UICButtonDelete**

# 2024-03-13 (1.1.14)
- Fix IUICSignalRServiceOptioneel

# 2024-03-13 (1.1.13)
- Readme file
- IUICQuestionService
- Renaming IuicLanguageService => IUICLanguageService
- Add 'col-form-label' class to labels in singleRow inputs


