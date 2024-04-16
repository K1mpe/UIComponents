namespace UIComponents.Abstractions.Models;


public class UIComponentViewModel : IUIComponentViewModel
{
    public UIComponentViewModel(string renderlocation,object viewModel)
    {
        RenderLocation = renderlocation;
        ViewModel = viewModel;
    }

    public object ViewModel { get; set; }

    public string RenderLocation { get; set; }
}
