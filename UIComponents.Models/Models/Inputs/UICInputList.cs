using System.Text.Json;

namespace UIComponents.Models.Models.Inputs;

public class UICInputList : UICInput<object[]>
{
    #region Fields
    public override bool HasClientSideValidation => false;
    #endregion

    #region Ctor

    public UICInputList(string propertyName) : base(propertyName)
    {
    }
    public UICInputList() : base(null)
    {
        
    }
    #endregion

    #region Properties
    [UICIgnoreGetChildrenFunction]
    public UICInput SingleInstanceInput { get; set; } 
    public Type ItemType { get; set; }

    /// <summary>
    /// When clicking the <see cref="AddButton"/>, a new instance with these default values.
    /// </summary>
    public object DefaultValueAdd { get; set; }

    public CreateNewInstanceEnum CreateNewInstanceMethod { get; set; } = CreateNewInstanceEnum.ReplaceIds;
    public bool ShowMoveButtons { get; set; } = true;
    public UICButton MoveUpButton { get; set; } = new UICButton()
    {
        PrependButtonIcon = new UICIcon("fas fa-arrow-up"),
        Tooltip = TranslatableSaver.Save("UICInputList.MoveUp", "Move this item up")
    }.AddClass("hidden-readonly");
    public UICButton MoveDownButton { get; set; } = new UICButton()
    {
        PrependButtonIcon = new UICIcon("fas fa-arrow-down"),
        Tooltip = TranslatableSaver.Save("UICInputList.MoveDown", "Move this item down")
    }.AddClass("hidden-readonly");

    public UICButton AddButton { get; set; } = new UICButton()
    {
        ButtonText = TranslatableSaver.Save("Button.Add"),
        PrependButtonIcon = IconDefaults.Add?.Invoke()
    }.AddClass("hidden-readonly");

    public UICButton RemoveButton { get; set; } = new UICButton()
    {
        PrependButtonIcon = IconDefaults.Delete?.Invoke(),
        Tooltip = TranslationDefaults.ButtonDelete
    }.AddClass("hidden-readonly");

    public ButtonOrientationEnum ButtonOrientation { get; set; }
    #endregion


    #region ClientSideMethods
    public IUICAction TriggerAddInstance(object value = null)
    {
        return new UICCustom($"$('#{this.GetId()}').trigger('uic-add', {JsonSerializer.Serialize(value)});");
    }
    public IUICAction TriggerRemoveInstance(string instanceSelector)
    {
        return new UICCustom($"$('{instanceSelector}').trigger('uic-remove');");
    }
    #endregion
    public enum ButtonOrientationEnum
    {
        Auto,
        Horizontal,
        Vertical
    }

    public enum CreateNewInstanceEnum
    {
        /// <summary>
        /// Store the <see cref="SingleInstanceInput"/> clientside and replace the Ids using regex.
        /// </summary>
        ReplaceIds,

        /// <summary>
        /// Store the <see cref="SingleInstanceInput"/> serverside and generate a new instance on each add.
        /// </summary>
        GetRequest,
    }
}
