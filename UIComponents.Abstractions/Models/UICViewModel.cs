namespace UIComponents.Abstractions.Models;


public class UICViewModel : IUICViewModel, IUICAction
{
    public UICViewModel(string renderlocation,object viewModel)
    {
        RenderLocation = renderlocation;
        ViewModel = viewModel;
    }

    public object ViewModel { get; set; }

    public string RenderLocation { get; set; }
}
