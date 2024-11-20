using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
namespace UIComponents.Web.Taghelpers;


/// <summary>
/// Render a component here unless <see cref="Invoke"/> is false.
/// <br>can load child content if <see cref="IUICSupportsTaghelperContent"/> is enabled on the element</br>
/// <br>applies all htlm attributes if <see cref="IUICHasAttributes"/> is enabled on the element</br>
/// </summary>
[HtmlTargetElement("uic")]
public class UICTaghelper : TagHelper
{
    private readonly ILogger _logger;
    private readonly IViewComponentHelper _viewComponentHelper;
    public UICTaghelper(IViewComponentHelper viewComponentHelper, ILogger<UICTaghelper> logger)
    {
        _viewComponentHelper = viewComponentHelper;
        _logger = logger;
    }


    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; }

    [HtmlAttributeName("id")]
    public string Id { get; set; }

    /// <summary>
    /// The component that is used
    /// </summary>
    [HtmlAttributeName("uic")]
    public IUIComponent UIC { get; set; }

    /// <summary>
    /// a short alias for <see cref="UIC"/>
    /// <inheritdoc cref="UIC"/>
    /// </summary>
    [HtmlAttributeName("c")]
    public IUIComponent C
    {
        get => UIC; 
        set => UIC = value;
    }

    /// <summary>
    /// Header is required for any component that tries to make a child card with the content
    /// </summary>
    [HtmlAttributeName("header")]
    public object Header { get; set; }

    /// <summary>
    /// If true, the component is rendered where the taghelper is used. If false, you need to invoke it somewhere else
    /// </summary>
    [HtmlAttributeName("invoke")]
    public bool Invoke { get; set; } = true;

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (UIC == null)
            return;

        output.TagName = "";
        output.TagMode = TagMode.StartTagAndEndTag;

        var content = await output.GetChildContentAsync();
        var contentString = content.GetContent().Trim();

        var attributes = context.AllAttributes.ToDictionary(x => x.Name, x => x.Value);

        if (UIC is IUICSupportsTaghelperContent supportContent)
        {
            if (contentString.Length > 0 || supportContent.CallWithEmptyContent)
            {
                await supportContent.SetTaghelperContent(contentString, attributes);
            }
        }
        else if (contentString.Length > 0)
        {
            _logger.LogWarning("{0} does not support taghelper content {1}", UIC, contentString);
        }

        if (UIC is IUICHasAttributes hasAttributes)
        {
            if (!string.IsNullOrWhiteSpace(Id))
                hasAttributes.SetId(Id);

            foreach (var attr in attributes)
            {
                switch (attr.Key)
                {
                    case "id":
                    case "c":
                    case "invoke":
                        continue;
                }
                hasAttributes.AddAttribute(attr.Key, attr.Value.ToString());
            }
        }
        output.Content.Clear();
        if (Invoke)
        {
            ((IViewContextAware)_viewComponentHelper).Contextualize(ViewContext);
            var componentResult = await _viewComponentHelper.InvokeAsync(UIC);
            output.Content.SetHtmlContent(componentResult);
        }

        await base.ProcessAsync(context, output);
    }
}
