namespace UIComponents.Models.Models.Inputs;

public class UICInputEditorTemplate : UICInput<Object>
{
    public override bool HasClientSideValidation => true;

    public UICInputEditorTemplate(string propertyName, string templateFor, object data = null) : this(propertyName)
    {
        TemplateFor = templateFor;
        AdditionalData = data;
    }
    public UICInputEditorTemplate(string propertyName) : base(propertyName)
    {

    }
    public UICInputEditorTemplate() : base(null)
    {
            
    }


    public string Expression { get; set; }
    public string TemplateFor { get; set; }
    public object AdditionalData { get; set; }
}
