namespace UIComponents.Models.Models.Inputs;

public class UICInputTime : UICInput<TimeOnly?>
{
    public override bool HasClientSideValidation => ValidationRequired ||ValidationMinTime.HasValue || ValidationMaxTime.HasValue;

    public override string RenderLocation => UIComponent.DefaultIdentifier(nameof(UICInputTime), Renderer);

    #region Ctor

    public UICInputTime() : base(null)
    {
            
    }

    public UICInputTime(string propertyName) : base(propertyName) 
    {
        
    }

    #endregion

    #region Properties

    /// <summary>
    /// If Step is 15 and <see cref="Precision"/> is minutes, you can only choose 0, 15, 30 or 45 minutes.
    /// </summary>
    public int Step { get; set; } = 1;

    /// <summary>
    /// Setting the precision of the input, this also infects the <see cref="Step"/>
    /// </summary>
    public UICTimeonlyEnum Precision { get; set; } = UICTimeonlyEnum.Minute;

    public bool ValidationRequired { get; set; }
    public TimeOnly? ValidationMinTime { get; set; }
    public TimeOnly? ValidationMaxTime { get; set; }

    public InputTimeRenderer Renderer { get; set; } = InputTimeRenderer.Default;
    #endregion

    public enum InputTimeRenderer
    {
        Default,
        SelectLists
    }
}
