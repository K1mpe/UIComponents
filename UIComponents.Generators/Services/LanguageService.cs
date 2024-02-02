using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIComponents.Abstractions.Interfaces;
using UIComponents.Abstractions.Interfaces.ExternalServices;

namespace UIComponents.Generators.Services.Internal
{
    internal class LanguageService : IUicLanguageService
    {
        public Task<string> Translate(Translatable translationModel)
        {
            if (translationModel == null)
                return Task.FromResult(string.Empty);
            return Task.FromResult(translationModel.ToString());
        }
    }
}
