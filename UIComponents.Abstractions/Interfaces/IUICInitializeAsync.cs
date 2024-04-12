using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIComponents.Abstractions.Interfaces
{
    public interface IUICInitializeAsync
    {
        /// <summary>
        /// This task is triggered just before the component gets rendered
        /// </summary>
        /// <remarks>
        /// This function comes after the <see cref="IConditionalRender"/> check. So if the rendering is set to false, this method is not triggered.
        /// You can still use the InitializeAsync to set the <see cref="IConditionalRender.Render"/> to false.
        /// </remarks>
        /// <returns></returns>
        Task InitializeAsync();
    }
}
