using Microsoft.AspNetCore.Razor.TagHelpers;
namespace UIComponents.Web.Taghelpers;

/// <summary>
/// A custom block of html or javascript that is converted to a UIComponent
/// </summary>
[HtmlTargetElement("uic-custom")]
public class UICCustomTaghelper : TagHelper
{
    [HtmlAttributeName("uic")]
    public IUIComponent UIC { get; set; }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        output.TagName = "";
        output.TagMode = TagMode.StartTagAndEndTag;
        var content = await output.GetChildContentAsync();
        var contentString = content.GetContent().Trim();
        output.Content.Clear();
        if (UIC == null || !UIC.GetType().IsAssignableTo(typeof(UICCustom)))
        {
            throw new Exception("Before using a UIC in the UIComponentTaghelper, first assign it as a new UICCustom()");
        }
        if (UIC is UICCustom custom)
        {
            custom.Content = contentString;
        }

        await base.ProcessAsync(context, output);
    }
}
