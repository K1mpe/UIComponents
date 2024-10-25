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
        RenderConditions.Add((self) => ((UICInputGroup)self).RenderWithoutInput || ((UICInputGroup)self).Input.HasValue());
    }
    public UICInputGroup(Translatable label, UICInput input) : this(new UICLabel(label), input)
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
    /// This is the text underneath the input
    /// </summary>
    public UICSpan Span { get; set; }

    /// <summary>
    /// If the <see cref="Input"/> is not rendered, do you still want to render the inputgroup?
    /// </summary>
    public bool RenderWithoutInput { get; set; }

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

    /// <summary>
    /// Return the Input as the assigned type. This will throw a <see cref="Exception"></see> if the type does not match
    /// </summary>
    public T InputAs<T>() where T: UICInput
    {
        try
        {
            return (T)Input;
        }
        catch
        {
            throw new Exception($"Cannot parse {Input.GetType().Name} to {typeof(T).Name}");
        }
    }

    /// <summary>
    /// If the Input is this type, apply this action
    /// </summary>
    public UICInputGroup InputAs<T>(Action<T> action) where T: UICInput
    {
        TryInputAs(action);
        return this;
    }

    /// <summary>
    /// Try to map the <see cref="Input"/> to this type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="input"></param>
    /// <returns></returns>
    public bool TryInputAs<T>(out T input) where T: UICInput
    {
        input = default;
        if(Input is T convertedInput)
        {
            input = convertedInput;
            return true;
        }
        return false;
    }
    public bool TryInputAs<T>(Action<T> action) where T : UICInput
    {
        if(TryInputAs<T>(out var input))
        {
            action(input);
            return true;
        }
        return false;
    }


    public void TransformToSingleRow()
    {
            Renderer = InputGroupRenderer.Grid;
    }

    public bool RendersInSingleRow()
    {
        if (!Label.HasValue() || !Input.HasValue())
            return false;
        return Renderer == InputGroupRenderer.Grid;
    }
    #endregion
}
public enum InputGroupRenderer
{
    Default,
    Grid
}
