using Microsoft.AspNetCore.Mvc.Rendering;

namespace UIComponents.Models.Models.Inputs;

public class UICSelectListItem : UIComponent
{
    #region Ctor
    public UICSelectListItem()
    {

    }

    public UICSelectListItem(string text, object value)
    {
        Text = text;
        Value = value?.ToString() ?? null;
    }
    #endregion


    #region DefaultProps

    public string Text { get; set; }
    public object Value { get; set; }
    public bool Selected { get; set; }

    public bool Disabled { get; set; }
    public UICSelectListGroup Group { get; set; }

    #endregion

    #region ExtendedProps
    /// <summary>
    /// Items with a sortorder will be displayed first. Lower number on top.
    /// </summary>
    public int SortOrder { get; set; }
        
    /// <summary>
    /// If the selectList supports searching, this item can be found on this tag
    /// </summary>
    public string SearchTag { get; set; }

    /// <summary>
    /// Elements that should be placed before the text (Example: Icons)
    /// </summary>
    public List<IUIComponent> PrependText { get; set; } = new();

    /// <summary>
    /// Elements that should be placed before the text (Example: Icons)
    /// </summary>
    public List<IUIComponent> AppendText { get; set; } = new();

    public string Tooltip { get; set; }

    #endregion

    #region Methods
    public UICSelectListItem AddPrepend(IUIComponent component)
    {
        component.AssignParent(this);
        PrependText.Add(component);
        return this;
    }
    public UICSelectListItem AddPrepend<T>(out T added, T component) where T: IUIComponent
    {
        component.AssignParent(this);
        PrependText.Add(component);
        added = component;
        return this;
    }
    public UICSelectListItem AddPrepend<T>(T component, Action<T> configure) where T: IUIComponent
    {
        component.AssignParent(this);
        PrependText.Add(component);
        configure(component);
        return this;
    }

    public UICSelectListItem AddAppend(IUIComponent component)
    {
        component.AssignParent(this);
        AppendText.Add(component);
        return this;
    }
    public UICSelectListItem AddAppend<T>(out T added, T component) where T : IUIComponent
    {
        component.AssignParent(this);
        AppendText.Add(component);
        added = component;
        return this;
    }
    public UICSelectListItem AddAppend<T>(T component, Action<T> configure) where T : IUIComponent
    {
        component.AssignParent(this);
        AppendText.Add(component);
        configure(component);
        return this;
    }
    #endregion

    #region Converters
    public static implicit operator UICSelectListItem(SelectListItem item) => item == null ? null: new()
    {
        Text = item.Text,
        Value = item.Value,
        Selected = item.Selected,
        Disabled = item.Disabled,
        Group = item.Group,
    };
    

    #endregion

}
public class UICSelectListGroup : UIComponent
{
    #region Ctor
    public UICSelectListGroup() { }
    #endregion

    #region DefaultProperties
    public string Name { get; set; }
    public bool Disabled { get; set; }
    #endregion

    #region ExtendedProps
    /// <summary>
    /// Items with a sortorder will be displayed first. Lower number on top.
    /// </summary>
    public int SortOrder { get; set; }

    /// <summary>
    /// if the selectList supports searching, this tags are included in the results
    /// </summary>
    public string SearchTag { get; set; }

    /// <summary>
    /// Elements that should be placed before the text (Example: Icons)
    /// </summary>
    public List<IUIComponent> PrependText { get; set; } = new();

    /// <summary>
    /// Elements that should be placed before the text (Example: Icons)
    /// </summary>
    public List<IUIComponent> AppendText { get; set; } = new();


    public Translatable Tooltip { get; set; }

    #endregion


    #region Methods
    public UICSelectListGroup AddPrepend(IUIComponent component)
    {
        component.AssignParent(this);
        PrependText.Add(component);
        return this;
    }
    public UICSelectListGroup AddPrepend<T>(out T added, T component) where T : IUIComponent
    {
        component.AssignParent(this);
        PrependText.Add(component);
        added = component;
        return this;
    }
    public UICSelectListGroup AddPrepend<T>(T component, Action<T> configure) where T : IUIComponent
    {
        component.AssignParent(this);
        PrependText.Add(component);
        configure(component);
        return this;
    }

    public UICSelectListGroup AddAppend(IUIComponent component)
    {
        component.AssignParent(this);
        AppendText.Add(component);
        return this;
    }
    public UICSelectListGroup AddAppend<T>(out T added, T component) where T : IUIComponent
    {
        component.AssignParent(this);
        AppendText.Add(component);
        added = component;
        return this;
    }
    public UICSelectListGroup AddAppend<T>(T component, Action<T> configure) where T : IUIComponent
    {
        component.AssignParent(this);
        AppendText.Add(component);
        configure(component);
        return this;
    }
    #endregion

    public static implicit operator UICSelectListGroup(SelectListGroup group) => group == null? null:new()
    {
        Name = group.Name,
        Disabled = group.Disabled,
    };
}