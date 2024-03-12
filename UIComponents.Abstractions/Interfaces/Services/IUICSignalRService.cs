using UIComponents.Abstractions.Models.HtmlResponse;

namespace UIComponents.Abstractions.Interfaces.Services;


public interface IUICSignalRService
{

    /// <summary>
    /// Handled by uic.signalR.handleUIComponentFetch
    /// </summary>
    public Task SendUIComponentToUser(FetchComponent fetchComponent, string userId);

    /// <summary>
    /// Triggers $('#id').trigger('uic-remove') clientside
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public Task RemoveUIComponentWithId(string id);

}
