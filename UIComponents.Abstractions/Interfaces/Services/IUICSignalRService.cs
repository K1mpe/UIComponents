using UIComponents.Abstractions.Models.HtmlResponse;

namespace UIComponents.Abstractions.Interfaces.Services;


public interface IUICSignalRService
{

   
    /// <summary>
    /// Handled by uic.signalR.handleUIComponentFetch
    /// </summary>
    public Task SendUIComponentToUser(UICFetchComponent fetchComponent, string userId);

    /// <summary>
    /// Triggers $('#id').trigger('uic-remove') clientside
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task RemoveUIComponentWithId(string id);

    /// <summary>
    /// A C# event converted to a signalR trigger
    /// </summary>
    /// <param name="key"></param>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public Task EventHandler(string key, object sender,  EventArgs args);

}
