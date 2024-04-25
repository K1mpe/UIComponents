using UIComponents.Abstractions.Interfaces.Tables;

namespace UIComponents.Models.Models.Tables.TableColumns;

public class UICTableColumnButton : IUICTableColumn, IUICHasScriptCollection
{
    #region Fields
    public string RenderLocation => UIComponent.DefaultIdentifier(nameof(UICTableColumnButton));
    public string Type => "uic-button";
    #endregion

    #region Ctor
    public UICTableColumnButton()
    {
        
    }

    public UICTableColumnButton(UICButton button)
    {
        Button = button;
        if(button.OnClick is not UICActionNavigate)
        {
            OnClick = button.OnClick;
            Button.OnClick = null;
        }
    }



    #endregion

    #region Properties
    public string Width { get; set; } = "auto";
    public Translatable Title { get; set; }
    public UICHorizontalAlignment Alignment { get; set; } = UICHorizontalAlignment.Center;
    public UICButton Button { get; set; }
    public IUICAction OnClick { get; set; }
    public IUICScriptCollection ScriptCollection { get; set; } = new UICScriptCollection();

    public bool Render { get; set; } = true;

    #endregion

}
