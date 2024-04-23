using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIComponents.Abstractions.Interfaces.Services;

namespace UIComponents.Web.Tests.Services
{
    public class PermissionService : IUICPermissionService
    {
        public Task<bool> CanCreateType(Type type)
        {
            return Task.FromResult(true);
        }

        public Task<bool> CanDeleteObject<T>(T obj) where T : class
        {
            return Task.FromResult(true);
        }

        public Task<bool> CanEditObject<T>(T oldObject) where T : class
        {
            return Task.FromResult(true);
        }

        public Task<bool> CanEditProperty<T>(T obj, string propertyName) where T : class
        {
            return Task.FromResult(true);
        }

        public Task<bool> CanEditPropertyOfType(Type type, string propertyName)
        {
            return Task.FromResult(true);
        }

        public Task<bool> CanViewObject<T>(T obj) where T : class
        {
            return Task.FromResult(true);
        }

        public Task<bool> CanViewProperty<T>(T obj, string propertyName) where T : class
        {
            return Task.FromResult(true);
        }

        public Task<bool> CanViewPropertyOfType(Type type, string propertyName)
        {
            return Task.FromResult(true);
        }
    }
}
