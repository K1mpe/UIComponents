namespace UIComponents.Models.Models
{

    /// <summary>
    /// A form that can be posted. This is required to use the <see cref="UICActionSubmit"/>
    /// </summary>
    public class UICForm : UIComponent, IUICHasChildren<IUIComponent>, IUICHasAttributesAndChildren
    {

        #region Ctor
        public UICForm() : base()
        {

        }
        public UICForm(ISubmitAction submitAction) : base()
        {
            Submit = submitAction;
        }
        #endregion

        #region Properties

        public List<IUIComponent> Children { get; set; } = new();

        /// <summary>
        /// On loading, set the focus on the first input field
        /// </summary>
        public bool SetFocusOnFirstInput { get; set; } = UIComponents.Defaults.Models.UICForm.SetFocusOnFirstInput;

        /// <summary>
        /// Disable posting the current form when the user presses the "Enter" key on the keyboard.
        /// </summary>
        public bool DisablePostOnEnterClick { get; set; } = UIComponents.Defaults.Models.UICForm.DisablePostOnEnterClick;

        /// <summary>
        /// Set the form as readonly. This will disable the submit buttons
        /// </summary>
        public bool Readonly { get; set; }


        public ISubmitAction Submit { get; set; }

        /// <summary>
        /// ClientSide: Triggers the form to post
        /// </summary>
        /// <returns></returns>
        public IUICAction? TriggerSubmit() => Readonly ? null : new UICCustom($"$('#{this.GetId()}').trigger('submit');");

        /// <summary>
        /// ClientSide: Triggers the form to return the current value of all properties
        /// </summary>
        /// <returns></returns>
        public IUICAction TriggerGetValue() => new UICActionGetValue(this);
        #endregion


    }
}

namespace UIComponents.Defaults.Models
{
    public static class UICForm
    {

        public static bool SetFocusOnFirstInput { get; set; } = true;

        public static bool DisablePostOnEnterClick { get; set; }
    }
}

