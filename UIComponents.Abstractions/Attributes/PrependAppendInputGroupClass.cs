namespace UIComponents.Abstractions.Attributes;


/// <summary>
/// Use this attribute to overwrite the default input-group-text class for appends or prepends in input groups
/// </summary>
public class PrependAppendInputGroupClass : Attribute
{
    public PrependAppendInputGroupClass(string setClass)
    {
        PrependClass = setClass;
        AppendClass = setClass;
    }

    public PrependAppendInputGroupClass(string prependClass, string appendClass)
    {
        PrependClass = prependClass;
        AppendClass = appendClass;
    }




    public string PrependClass { get; set; } = "input-group-text";
    public string AppendClass { get; set; } = "input-group-text";
}
