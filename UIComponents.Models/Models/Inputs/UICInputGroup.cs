using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Models;
using UIComponents.Models.Models.Texts;

namespace UIComponents.Models.Models.Inputs;

/// <summary>
/// A input group that has a <see cref="Input"/> and a <see cref="Label"/> and optionally a <see cref="Span"/>
/// <br>This is recommended to use for all input fields</br>
/// </summary>
public class UICInputGroup : UIComponent, IUISingleRowSupport
{
    #region Fields
    public override string RenderLocation => this.CreateDefaultIdentifier(Renderer);
    #endregion

    #region Ctor
    public UICInputGroup() : base()
    {

    }
    public UICInputGroup(ITranslateable label, UICInput input) : this(new UICLabel(label), input)
    {
    }
    public UICInputGroup(UICLabel label, UICInput input) : this()
    {
        Label = label;
        Input = input;
    }
    #endregion

    #region Properties
    public UICLabel Label { get; set; }

    public UICInput Input { get; set; }

    /// <summary>
    /// These components are placed before the input
    /// </summary>
    public List<UIComponent> PrependInput { get; set; } = new();

    /// <summary>
    /// These components are placed after the input
    /// </summary>
    public List<UIComponent> AppendInput { get; set; } = new();

    /// <summary>
    /// This is the text underneath the input.
    /// </summary>
    public UICSpan Span { get; set; }

    /// <summary>
    /// Set the label and input field on the same row
    /// </summary>
    /// <remarks>
    /// To prevent a form from automatically turning this on, set value to false.
    /// </remarks>
    public bool? LabelAndInputOnSingleLine { get; set; }


    /// <summary>
    /// This is the render page to process this object
    /// </summary>
    /// <remarks>
    /// /Views/Shared/Component/UIComponent/InputGroup/_Renderer.cshtml
    /// </remarks>
    public InputGroupRenderer Renderer { get; set; } = InputGroupRenderer.Default;

    #endregion

    #region Methods
    public override string ToString()
    {
        return "Input-Group " + Input?.ToString();
    }

    public void TransformToSingleRow()
    {
        if (LabelAndInputOnSingleLine != false)
            Renderer = InputGroupRenderer.Grid;
    }

    public bool RendersInSingleRow()
    {
        return Renderer == InputGroupRenderer.Grid;
    }
    #endregion
}
public enum InputGroupRenderer
{
    Default,
    Grid
}
