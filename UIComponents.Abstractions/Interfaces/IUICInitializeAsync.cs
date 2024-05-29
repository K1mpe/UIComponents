using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIComponents.Abstractions.Interfaces
{
    /// <summary>
    /// Use this interface on <see cref="IUIComponent"/> that can initialize before rendering
    /// </summary>
    public interface IUICInitializeAsync : IUIComponent
    {
        /// <summary>
        /// This task is triggered just before the component gets rendered
        /// </summary>
        /// <remarks>
        /// This function comes after the <see cref="IUICConditionalRender"/> check. So if the rendering is set to false, this method is not triggered.
        /// You can still use the InitializeAsync to set the <see cref="IUICConditionalRender.Render"/> to false.
        /// </remarks>
        /// <returns></returns>
        Task InitializeAsync();
    }
}
