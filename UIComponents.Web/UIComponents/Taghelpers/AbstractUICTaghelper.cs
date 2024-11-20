using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UIComponents.Models.Models;

namespace UIComponents.Web.UIComponents.Taghelpers
{
    public abstract class AbstractUICTaghelper : TagHelper
    {
        private readonly ILogger _logger;
        private readonly IViewComponentHelper _viewComponentHelper;
        public AbstractUICTaghelper(IViewComponentHelper viewComponentHelper, ILogger<AbstractUICTaghelper> logger)
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
        /// If true, the component is rendered where the taghelper is used. If false, you need to invoke it somewhere else
        /// </summary>
        [HtmlAttributeName("invoke")]
        public bool Invoke { get; set; } = true;

        public abstract IUIComponent BuildComponent();
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "";
            output.TagMode = TagMode.StartTagAndEndTag;
            var content = await output.GetChildContentAsync();
            var contentString = content.GetContent().Trim();

            

            var attributes = context.AllAttributes.ToDictionary(x => x.Name, x => x.Value);

            var component = BuildComponent();
            if (component is IUICSupportsTaghelperContent supportContent)
            {
                if (contentString.Length > 0 || supportContent.CallWithEmptyContent)
                {
                    await supportContent.SetTaghelperContent(contentString, attributes);
                }
            }
            else if (contentString.Length > 0)
            {
                _logger.LogWarning("{0} does not support taghelper content {1}", component, contentString);
            }

            if (component is IUICHasAttributes hasAttributes)
            {
                if (!string.IsNullOrWhiteSpace(Id))
                    hasAttributes.SetId(Id);

                foreach (var attr in attributes)
                {
                    switch (attr.Key)
                    {
                        case "id":
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
                var componentResult = await _viewComponentHelper.InvokeAsync(component);
                output.Content.SetHtmlContent(componentResult);
            }

            await base.ProcessAsync(context, output);
        }

        //public static async Task CreateTaghelperForComponent<T>(string destinationPath) where T : class, IUIComponent
        //{
        //    var name = typeof(T).Name;
        //    if (name.ToUpper().StartsWith("UIC"))
        //        name = name.Substring(3);

        //    if (destinationPath.EndsWith("\\"))
        //        destinationPath = Path.Join(destinationPath, $"{name}.cs");

        //    if(File.Exists(destinationPath))
        //        File.Delete(destinationPath);

        //    using(var textStream = new StreamWriter(new MemoryStream()))
        //    {
        //        await textStream.WriteLineAsync($"using Microsoft.AspNetCore.Mvc;");
        //        await textStream.WriteLineAsync($"using Microsoft.AspNetCore.Mvc.Rendering;");
        //        await textStream.WriteLineAsync($"using Microsoft.AspNetCore.Razor.TagHelpers;");
        //        await textStream.WriteLineAsync($"using Microsoft.Extensions.Logging;");
        //        await textStream.WriteLineAsync($"using UIComponents.Web.UIComponents.Taghelpers;");
        //        await textStream.WriteLineAsync($"namespace UIComponents.Web.UIComponents.Taghelpers;");
        //        await textStream.WriteLineAsync($"public class {name} : {nameof(AbstractUICTaghelper)}");
        //        await textStream.WriteLineAsync($"{{");
        //        await textStream.WriteLineAsync($"   public {name}(IViewComponentHelper viewComponentHelper, ILogger<AbstractUICTaghelper> logger) : base(viewComponentHelper, logger)");
        //        await textStream.WriteLineAsync($"   {{");
        //        await textStream.WriteLineAsync($"   }}");
        //        await textStream.WriteLineAsync($"");

        //        foreach(var property in typeof(T).GetProperties())
        //        {
        //            if(!property.CanWrite)
        //                continue;

        //            await textStream.WriteLineAsync($"[HtmlAttributeName(\"{property.Name.ToLower()}\")]");
        //            await textStream.WriteLineAsync($"[HtmlAttributeName(\"{property.Name.ToLower()}\")]");
        //        }

        //        await textStream.WriteLineAsync($"}}");

        //    }


        //    return Task.CompletedTask;
        //}
    }
}
