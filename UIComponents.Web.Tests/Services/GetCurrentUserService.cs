using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIComponents.Abstractions.Interfaces.Services;

namespace UIComponents.Web.Tests.Services
{
    public class GetCurrentUserService : IUICGetCurrentUserId
    {
        public object? GetCurrentUserId() => 1;
    }
}
