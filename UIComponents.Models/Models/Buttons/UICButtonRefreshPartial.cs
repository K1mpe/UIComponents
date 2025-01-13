namespace UIComponents.Models.Models.Buttons;

/// <summary>
/// Create a button to update a partial. If no partial is provided, the closest parent partial will be used.
/// </summary>
public class UICButtonRefreshPartial : UICButton
{
	public UICButtonRefreshPartial(UICPartial partial)
	{
		PrependButtonIcon = IconDefaults.RefreshIcon?.Invoke();
		ButtonText = TranslationDefaults.ButtonRefresh;
		PrependButtonIcon.GetId();
		OnClick = new UICActionRefreshPartial(partial, PrependButtonIcon);
	}
}
