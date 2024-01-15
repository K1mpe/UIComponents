using System.Drawing;

namespace UIComponents.Abstractions;

public interface IColor
{
    public string Name { get; set; }
    public string ToLower()
    {
        return Name.ToLower();
    }
}

public class UICColor : IColor
{
    public UICColor(string name)
    {
        Name = name;
    }

    public string Name { get; set; }

    
}
