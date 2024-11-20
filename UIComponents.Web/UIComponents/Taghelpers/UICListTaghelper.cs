using Microsoft.AspNetCore.Razor.TagHelpers;
namespace UIComponents.Web.Taghelpers;

/// <summary>
/// Add custom content to a list of components
/// </summary>
[HtmlTargetElement("uic-list")]
public class UICListTaghelper : TagHelper
{
    [HtmlAttributeName("uic")]
    public List<IUIComponent> UIC { get; set; }

    /// <summary>
    /// a short alias for <see cref="UIC"/>
    /// <inheritdoc cref="UIC"/>
    /// </summary>
    [HtmlAttributeName("c")]
    public List<IUIComponent> C
    {
        get => UIC;
        set => UIC = value;
    }
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "";
        output.TagMode = TagMode.StartTagAndEndTag;
        var content = await output.GetChildContentAsync();
        var contentString = content.GetContent().Trim();
        output.Content.Clear();
        if(contentString.Length > 0)
        {
            UIC.Add(new UICCustom(contentString));
        }

        await base.ProcessAsync(context, output);
    }
}
