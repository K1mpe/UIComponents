
namespace UIComponents.Models.Models.Inputs;

public class UICInputObject : UICInput<object>, IUICHasChildren<IUIComponent>
{

    public override bool HasClientSideValidation => false;

    public List<IUIComponent> Children { get; set; } = new();

    public UICInputObject() : base(null)
    {
            
    }
    public UICInputObject(string propertyName) : base(propertyName)
    {
    }

}
