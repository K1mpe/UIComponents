using System.Collections;
using UIComponents.Abstractions.Interfaces.Services;
using static UIComponents.Abstractions.Interfaces.IUICToastNotification;
using static UIComponents.Models.Models.UICToastr;

namespace UIComponents.Models.Models;

/// <summary>
/// The settings for a popup notification using Toastr.
/// More info can be found at https://github.com/CodeSeven/toastr
/// </summary>
public class UICToastr :  IUIComponent
{

    public string RenderLocation => UIComponent.DefaultIdentifier(nameof(UICToastr));
    public UICToastr ()
    {
    }
    public UICToastr (ToastType type, Translatable message, Translatable title = null)
    {
        Type = type;
        Message = message;
        Title = title;
    }
    public Translatable Title { get; set; }
    public Translatable Message { get; set; }

    public ToastType Type { get; set; }

    /// <summary>
    /// Where the toast notification should be displayed
    /// </summary>
    public ToastPosition Position { get; set; } = ToastPosition.TopRight;

    /// <summary>
    /// The time in miliseconds until the notification disapears
    /// </summary>
    /// <remarks>
    /// Set to 0 to make notification persist
    /// </remarks>
    public TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// The time in miliseconds until the notification disapears after hovering away from the notification
    /// </summary>
    /// <remarks>
    /// Set to 0 to make notification persist after hover
    /// </remarks>
    public TimeSpan ExtendDuration { get; set; } = TimeSpan.FromSeconds(1);


    /// <summary>
    /// Show or hide the close button
    /// </summary>
    public bool CloseButton { get; set; }

    /// <summary>
    /// Display a progressbar under the notification to visualize how long the notification will persist
    /// </summary>
    public bool ProgressBar { get; set; } = true;

    /// <summary>
    /// If multiple identical notifications are requested, only display one
    /// </summary>
    public bool PreventDuplicates { get; set; } = true;

    /// <summary>
    /// Show the newest notification on top in case of multiple notifications
    /// </summary>
    public bool NewestOnTop { get; set; } = true;


    /// <summary>
    /// The javascript code that is run when you click on this notification
    /// </summary>
    public IUICAction OnClick { get; set; }

    /// <summary>
    /// Other options, should be supported by <see href="https://github.com/CodeSeven/toastr"/>
    /// </summary>
    public Dictionary<string, object> Options { get; set; } = new();


    public Task SendToUser(IUICStoredComponents storedComponents, object userId) => storedComponents.SendComponentToUserSignalR(this, userId);
    public Task SendToUsers(IUICStoredComponents storedComponents, IEnumerable<object> userIds) => storedComponents.SendComponentToUsersSignalR(this, userIds);




    #region Enums

    public enum ToastPosition
    {
        TopRight = 1,
        TopLeft = 2,
        BottomRight = 3,
        BottomLeft = 4,
        TopCenter = 5,
        BottomCenter = 6,
        TopFullWidth = 7,
        BottomFullWidth = 8,
    }

        #endregion
}

