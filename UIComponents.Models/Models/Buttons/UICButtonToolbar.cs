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

        public ButtonPosition DefaultPosition { get; set; } = Defaults.Models.Buttons.UICButtonToolbar.DefaultPosition;

        public List<IUIComponent> Left { get; set; } = new();
        public List<IUIComponent> Center { get; set; } = new();
        public List<IUIComponent> Right { get; set; } = new();


        #endregion

        #region Methods
        /// <summary>
        /// Add a button in the <see cref="DefaultPosition"/>
        /// </summary>
        /// <remarks>
        /// Changing the <see cref="DefaultPosition"/> after adding a button does not move the button!
        /// </remarks>
        public UICButtonToolbar Add(IUIComponent component) => Add(component, DefaultPosition);

        public UICButtonToolbar Add(IUIComponent component, ButtonPosition position)
        {
            switch (position)
            {
                case ButtonPosition.Left:
                    return AddLeft(component);
                case ButtonPosition.Center:
                    return AddCenter(component);
                case ButtonPosition.Right:
                    return AddRight(component);
                default:
                    throw new NotImplementedException();
            }
        }


        ///<inheritdoc cref="AddCenter(IUIComponent)"/>
        public UICButtonToolbar Add<T>(out T addedButton, T button) where T : class, IUIComponent
        {
            addedButton = button;
            return Add(button);
        }

        ///<inheritdoc cref="AddCenter(IUIComponent)"/>
        public UICButtonToolbar Add<T>(T button, Action<T> action) where T: class, IUIComponent
        {
            action(button);
            return Add(button);
        }

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
        public UICButtonToolbar AddLeft<T>(T button, Action<T> action) where T : class, IUIComponent
        {
            action(button);
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
        public UICButtonToolbar AddCenter<T>(T button, Action<T> action) where T : class, IUIComponent
        {
            action(button);
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
        public UICButtonToolbar AddRight<T>(T button, Action<T> action) where T : class, IUIComponent
        {
            action(button);
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
        public static ButtonPosition DefaultPosition { get; set; } = ButtonPosition.Right;
    }
}

