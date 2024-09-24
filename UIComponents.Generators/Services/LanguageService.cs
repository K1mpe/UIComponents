using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIComponents.Abstractions.Interfaces;
using UIComponents.Abstractions.Interfaces.Services;

namespace UIComponents.Generators.Services.Internal
{
    internal class LanguageService : IUICLanguageService
    {
        public Task<string> Translate(Translatable translationModel)
        {
            if (translationModel == null)
                return Task.FromResult(string.Empty);
            return Task.FromResult(translationModel.ToString());
        }

        public Task<string> TranslateObject(object obj)
        {
            return Task.FromResult(obj.ToString());
        }
    }
}
