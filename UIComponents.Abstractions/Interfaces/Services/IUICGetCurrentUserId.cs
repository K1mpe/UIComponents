using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIComponents.Abstractions.Interfaces.Services
{
    public interface IUICGetCurrentUserId
    {
        /// <summary>
        /// Gets the Id of the current user, If null, there is no active user for this request
        /// </summary>
        /// <returns></returns>
        public object? GetCurrentUserId(); 
    }
}
