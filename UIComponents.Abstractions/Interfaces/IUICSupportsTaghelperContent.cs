using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIComponents.Abstractions.Interfaces
{
    public interface IUICSupportsTaghelperContent
    {

        /// <summary>
        /// Assign if the <see cref="SetTaghelperContent(string, Dictionary{string, object})"/> may be called when the content is empty
        /// </summary>
        public bool CallWithEmptyContent { get; }

        /// <summary>
        /// Add the content from the taghelper to this component. You may also use the attributes from taghelper
        /// </summary>
        /// <remarks>
        /// Remove used attributes from this dictionary if you want to prevent them to be used by <see cref="IUICHasAttributes"/>
        /// </remarks>
        public Task SetTaghelperContent(string taghelperContent, Dictionary<string, object> attributes);
    }

    /// <summary>
    /// This interface has a property that may be used as a <see cref="IUICSupportsTaghelperContent"/>
    /// </summary>
    public interface IUICSupportsTaghelperContentPassThrough : IUICSupportsTaghelperContent
    {
        public object PassThroughToChild { get; }

        bool IUICSupportsTaghelperContent.CallWithEmptyContent
        {
            get
            {
                if(PassThroughToChild is IUICSupportsTaghelperContent support)
                    return support.CallWithEmptyContent;
                return false;
            }
        }

        async Task IUICSupportsTaghelperContent.SetTaghelperContent(string taghelperContent, Dictionary<string, object> attributes)
        {
            if (PassThroughToChild is IUICSupportsTaghelperContent support)
                await support.SetTaghelperContent(taghelperContent, attributes);
        }
    }
}
