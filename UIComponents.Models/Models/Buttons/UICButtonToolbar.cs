using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Models;

namespace UIComponents.Models.Models.Buttons
{
    /// <summary>
    /// A toolbar used to get a consistent distance between buttons and you can choose to place buttons left, right or center.
    /// </summary>
    /// <remarks>
    /// If <see cref="Distance"/> is none, buttons will be placed in buttongroups</remarks>
    public class UICButtonToolbar : UIComponent
    {
        #region Ctor
        public UICButtonToolbar() : base()
        {

        }
        #endregion

        #region Properties
        public ButtonDistance Distance { get; set; } = UIComponents.Defaults.Models.Buttons.UICButtonToolbar.Distance;

        public List<IUIComponent> Left { get; set; } = new();
        public List<IUIComponent> Center { get; set; } = new();
        public List<IUIComponent> Right { get; set; } = new();


        #endregion

        #region Methods
        public UICButtonToolbar AddLeft(IUIComponent button)
        {
            button.AssignParent(this);
            Left.Add(button);
            return this;
        }
        public UICButtonToolbar AddLeft<T>(out T addedButton, T button) where T : class, IUIComponent
        {
            addedButton = button;
            return AddLeft(button);
        }

        public UICButtonToolbar AddCenter(IUIComponent button)
        {
            button.AssignParent(this);
            Center.Add(button);
            return this;
        }
        public UICButtonToolbar AddCenter<T>(out T addedButton, T button) where T : class, IUIComponent
        {
            addedButton = button;
            return AddCenter(button);
        }

        public UICButtonToolbar AddRight(IUIComponent button)
        {
            button.AssignParent(this);
            Right.Add(button);
            return this;
        }
        public UICButtonToolbar AddRight<T>(out T addedButton, T button) where T : class, IUIComponent
        {
            addedButton = button;
            return AddRight(button);
        }
        #endregion
    }
}

namespace UIComponents.Defaults.Models.Buttons
{
    public static class UICButtonToolbar
    {
        public static ButtonDistance Distance { get; set; } = ButtonDistance.Medium;
    }
}

