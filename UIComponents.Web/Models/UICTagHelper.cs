using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Encodings.Web;

namespace UIComponents.Models.Models;

/// <summary>
/// This type takes any custom <see cref="ITagHelper"/> and allows you to use it as a component
/// </summary>
/// <remarks>
/// When creating a <see cref="ITagHelper"/>, you have to create a class that inherits <see cref="TagHelper"/>. This class can be used in this component.
/// <br> new <see cref="UICTagHelper"/>(myTaghelper)</br>
/// <br> myTaghelper.ToUIC()</br>
/// </remarks>
public class UICTagHelper : UICCustom
{
    #region Ctor
    public UICTagHelper()
    {
        
    }
    public UICTagHelper(ITagHelper taghelper)
    {
        
    }
    #endregion

    #region Properties
    public ITagHelper Taghelper { get; set; }

    public override bool Render
    {
        get
        {
            if (Taghelper == null)
                return false;
            return base._render;
        }
        set
        {
            _render = value;
        }
    }
    #endregion

    #region Methods
    protected override async Task InitializeAsync()
    {
        base.Content = await InvokeTaghelper();
        await base.InitializeAsync();
    }
    protected async Task<string> InvokeTaghelper()
    {

        var context = new TagHelperContext(
           new TagHelperAttributeList(),
           new Dictionary<object, object>(),
           Guid.NewGuid().ToString("N"));

        var htmlTarget = Taghelper.GetType().GetCustomAttribute<HtmlTargetElementAttribute>();

        var tagName = htmlTarget?.Tag ?? "custom-tag";

        var output = new TagHelperOutput(
            tagName,
            new TagHelperAttributeList(),
            (useCachedResult, encoder) =>
            {
                var tagHelperContent = new DefaultTagHelperContent();
                tagHelperContent.SetContent(string.Empty);
                return Task.FromResult<TagHelperContent>(tagHelperContent);
            });

        // Process the tag helper
        await Taghelper.ProcessAsync(context, output);

        using (var writer = new System.IO.StringWriter())
        {
            output.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }
    }
    #endregion

    public static implicit operator UICTagHelper(TagHelper taghelper) { return new UICTagHelper(taghelper); }
}

public static class UICTagHelperExtensions
{
    public static UICTagHelper ToUIC(this ITagHelper tagHelper) => new UICTagHelper(tagHelper);
}