using UIComponents.Abstractions.Extensions;

namespace UIComponents.Models.Models.Buttons;

public class UICButtonRefreshPartial : UICButton
{
	public UICButtonRefreshPartial(UICPartial partial)
	{
		PrependButtonIcon = IconDefaults.RefreshIcon;
		ButtonText = TranslationDefaults.ButtonRefresh;
		PrependButtonIcon.GetOrGenerateId();
		OnClick = new UICActionRefreshPartial(partial, PrependButtonIcon);
	}
}
