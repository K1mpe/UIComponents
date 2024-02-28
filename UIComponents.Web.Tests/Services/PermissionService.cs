using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIComponents.Abstractions.Interfaces.ExternalServices;

namespace UIComponents.Web.Tests.Services
{
    public class PermissionService : IUicPermissionService
    {
        public Task<bool> CanCreate<T>(T obj) where T : class
        {
            return Task.FromResult(true);
        }

        public Task<bool> CanCreate(Type type)
        {
            return Task.FromResult(true);
        }

        public Task<bool> CanDelete<T>(T obj) where T : class
        {
            return Task.FromResult(true);
        }

        public Task<bool> CanEdit<T>(T newObject, T oldObject = null) where T : class
        {
            return Task.FromResult(true);
        }

        public Task<bool> CanEditProperty<T>(T obj, string propertyName) where T : class
        {
            return Task.FromResult(true);
        }

        public Task<bool> CanView<T>(T obj) where T : class
        {
            return Task.FromResult(true);
        }

        public Task<bool> CanViewProperty<T>(T obj, string propertyName) where T : class
        {
            return Task.FromResult(true);
        }
    }
}
