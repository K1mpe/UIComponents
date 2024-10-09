using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIComponents.Models.Models
{
    public class UICHtmlStorage : IUIComponent
    {
        #region Fields
        public string RenderLocation => UIComponent.DefaultIdentifier(nameof(UICHtmlStorage));
        #endregion

        #region Ctor
        public UICHtmlStorage(string id, IUICAction getValue, IUICAction validateTimestamp)
        {
            Id = id;
            GetValue = getValue;
            ValidateTimestamp = validateTimestamp;
        }
        public UICHtmlStorage()
        {
            
        }
        #endregion

        #region Properties
        public string Id { get; set; }

        /// <summary>
        /// This function has value that must be checked with the service to validate if the data may still be used
        /// </summary>
        public IUICAction ValidateTimestamp { get; set; } = new UICCustom();

        /// <summary>
        /// When the value is empty or <see cref="ValidateTimestamp"/> returns false, this function gets the data
        /// </summary>
        public IUICAction GetValue { get; set; } = new UICCustom();

        #endregion

        #region Triggers
        /// <summary>
        /// await uic.htmlStorage.getValueAsync('{Id}')
        /// </summary>
        /// <returns></returns>
        public IUICAction TriggerGetValue() => new UICCustom($"await uic.htmlStorage.getValueAsync('{Id}')");

        /// <summary>
        /// This creates a div that will load the content here.
        /// This will also auto update itself if <see cref="TriggerExpired"/> is called (from signalR or something).
        /// </summary>
        /// <param name="loadOutdatedFirst">If true, the old value is placed here without validating if it is up to date. Once the validation is completed and new data is loaded, this will still be replaced</param>
        /// <returns></returns>
        public IUIComponent LoadHere(bool loadOutdatedFirst)
        {

            var div = new UICGroup().AddAttribute("html-storage", Id).AddCss("display", "contents")
                .Add(new UICGroup()
                {
                    RenderWithoutContent = true,
                    Renderer = UICGroupRenderer.Div,
                }.AddCss("display", "contents"));

            var script = new UICCustom()
                .AddLine("<script>")
                .AddLine("$(document).ready(()=>{");
            if (loadOutdatedFirst)
                script
                    .AddLine($"let outdatedVal = uic.htmlStorage.getValueNoValidation('{Id}');")
                    .AddLine($"$('#{div.GetId()} div').html(outdatedVal);");

            script.AddLine($"uic.htmlStorage.onInitOrValueChanged('{Id}', async ()=>{{")
                .AddLine($"let value = await uic.htmlStorage.getValueAsync('{Id}');")
                .AddLine($"$('#{div.GetId()} div').html(value);")
                .AddLine($"}});")
                .AddLine("});")
                .AddLine("</script>");
            return div.Add(script);
        }

        /// <summary>
        /// uic.htmlStorage.dispose('{Id}')
        /// </summary>
        /// <returns></returns>
        public IUICAction TriggerExpired() => new UICCustom($"uic.htmlStorage.triggerExpired('{Id}')");

        
        #endregion
    }
}
