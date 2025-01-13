using UIComponents.Models.Models.Card;

namespace UIComponents.Models.Models.Buttons;

public class UICButtonCollapseCard : UICToggleButton
{
	public override string RenderLocation => this.CreateDefaultIdentifier();

    public UICButtonCollapseCard(UICCard? card= null)
	{
		ButtonTrue = new UICButton()
		{
			PrependButtonIcon = IconDefaults.ButtonCardExpend?.Invoke(),
			ButtonText = TranslationDefaults.ButtonCardExpand
		};
		ButtonFalse = new UICButton()
		{
			PrependButtonIcon = IconDefaults.ButtonCardCollapse?.Invoke(),
			ButtonText = TranslationDefaults.ButtonCardCollapse
		};
		Card = card;
	}

	/// <summary>
	/// The card that uses this button.
	/// <br>If null, the closest parent is used.</br>
	/// </summary>
	[UICIgnoreGetChildrenFunction]
	public UICCard? Card { get; set; }
}
