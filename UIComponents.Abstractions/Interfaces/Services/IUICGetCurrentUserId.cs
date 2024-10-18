using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIComponents.Abstractions.Interfaces.Services
{
    /// <summary>
    /// This is a singleton service that can get the current userId
    /// </summary>
    /// <remarks>
    /// You can use the IHttpContextAccessor to get the current userid
    /// </remarks>
    public interface IUICGetCurrentUserId
    {
        /// <summary>
        /// Gets the Id of the current user, If null, there is no active user for this request
        /// </summary>
        /// <returns></returns>
        public Task<object?> GetCurrentUserId(); 
    }
}
