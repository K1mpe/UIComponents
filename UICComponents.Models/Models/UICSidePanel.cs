namespace UIComponents.ComponentModels.Models;

public class UICSidePanel : UIComponent
{
    #region Fields

    public override string RenderLocation => this.CreateDefaultIdentifier(Position);
    #endregion


    #region Ctor

    public UICSidePanel(IUIComponent mainContent, IUIComponent sidepanelContent) : this()
    {
        MainContent = mainContent;
        SidePanelContent = sidepanelContent;


    }

    public UICSidePanel()
    {
        //The side panel cannot render if the main content does not render
        RenderConditions.Add(() => MainContent.HasValue());
    }

    #endregion

    #region Properties

    /// <summary>
    /// The content where the sidepanel overlaps
    /// </summary>
    public IUIComponent MainContent { get; set; }

    public IUIComponent SidePanelContent { get; set; }

    /// <summary>
    /// Optional: If this property is set, this will be used as a identifier to remember the previous state when reloading the page.
    /// </summary>
    public string SidePanelIdentifier { get; set; }

    /// <summary>
    /// The state of the sidepanel when loading the page. Will be overwriten by the saved state if <see cref="SidePanelIdentifier"/> is set
    /// </summary>
    public UICSidePanelState DefaultState { get; set; } = UICSidePanelState.Collapsed;


    public UICSidePanelPosition Position { get; set; } = UICSidePanelPosition.Left;


    #endregion

    #region Buttons
    /// <summary>
    /// A buttonToolbar that will automatically include the other buttons. Adding buttons to this toolbar will also add them in the sidebar.
    /// </summary>
    public UICButtonToolbar ButtonToolbar { get; set; } = new();

    public UICButton SetFixedButton { get; set; } = new UICButton()
    {
        Tooltip = new TranslationModel("Sidebar.SetFixed", "Pin this sidebar"),
        AppendButtonIcon = UICIcon.Pin()
    };

    public UICButton OpenSidebarButton { get; set; } = new UICButton()
    {
        Tooltip = new TranslationModel("Sidebar.Open", "Open the sidebar"),
        AppendButtonIcon = new UICIcon("fas fa-angles-right")
    };

    public UICButton CloseSidebarButton { get; set; } = new UICButton()
    {
        Tooltip = new TranslationModel("Sidebar.Close", "Close the sidebar"),
        AppendButtonIcon = new UICIcon("fas fa-angles-left")
    };
    #endregion

    #region Actions

    /// <summary>
    /// Action triggered before the sidePanel switches to Overlay state.
    /// </summary>
    public IUIAction BeforeOverlay { get; set; }

    /// <summary>
    /// Action triggered after the sidePanel has switched to OverlayState.
    /// </summary>
    public IUIAction AfterOverlay { get; set; }

    public IUIAction BeforeCollapsed { get; set; }
    public IUIAction AfterCollapsed { get; set; }

    public IUIAction BeforePinned { get; set; }
    public IUIAction AfterPinned { get; set; }

    #endregion

    #region Methods
    public void Initialize()
    {
        ButtonToolbar.AddAttribute("class", "btn-toolbar-sm");

        if (SetFixedButton != null)
        {
            SetFixedButton.AddAttribute("class", "btn-sidebar-fixed");
            if (SetFixedButton.OnClick == null)
                SetFixedButton.OnClick = new UICActionNavigate("#");
        }


        if (OpenSidebarButton != null)
        {
            OpenSidebarButton.AddAttribute("class", "btn-sidebar-open btn-sm position-absolute");
            if (SetFixedButton.OnClick == null)
                SetFixedButton.OnClick = new UICActionNavigate("#");
        }


        if (CloseSidebarButton != null)
        {
            CloseSidebarButton.AddAttribute("class", "btn-sidebar-close");
            if (SetFixedButton.OnClick == null)
                SetFixedButton.OnClick = new UICActionNavigate("#");
        }


        switch (Position)
        {
            case UICSidePanelPosition.Left:
                ButtonToolbar.Right.Add(SetFixedButton);
                ButtonToolbar.Right.Add(CloseSidebarButton);
                break;
            case UICSidePanelPosition.Top:
                break;
            case UICSidePanelPosition.Right:
                break;
            case UICSidePanelPosition.Bottom:
                break;
        }
    }
    #endregion

    public enum UICSidePanelState
    {
        Collapsed = 0,
        Overlay = 1,
        Fixed = 2
    }

    public enum UICSidePanelPosition
    {
        Left = 0,
        Top = 1,
        Right = 2,
        Bottom = 3
    }
}
