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

    #endregion

    #region Properties
    public UICInput SingleInstanceInput { get; set; }
    public Type ItemType { get; set; }


    public bool ShowMoveButtons { get; set; } = true;
    public UICButton MoveUpButton { get; set; } = new UICButton()
    {
        PrependButtonIcon = new UICIcon("fas fa-arrow-up"),
        Tooltip = new("UICInputList.MoveUp", "Move this item up")
    }.AddClass("hidden-readonly");
    public UICButton MoveDownButton { get; set; } = new UICButton()
    {
        PrependButtonIcon = new UICIcon("fas fa-arrow-down"),
        Tooltip = new("UICInputList.MoveDown", "Move this item down")
    }.AddClass("hidden-readonly");

    public UICButton AddButton { get; set; } = new UICButton()
    {
        ButtonText = new("Button.Add"),
        PrependButtonIcon = new UICIcon(IconDefaults.Add.Icon)
    }.AddClass("hidden-readonly");

    public UICButton RemoveButton { get; set; } = new UICButton()
    {
        PrependButtonIcon = new UICIcon(IconDefaults.Delete.Icon),
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
}
