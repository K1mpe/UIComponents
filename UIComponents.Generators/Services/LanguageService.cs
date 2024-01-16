using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIComponents.Abstractions.Interfaces;
using UIComponents.Abstractions.Interfaces.ExternalServices;

namespace UIComponents.Generators.Services
{
    internal class LanguageService : IUicLanguageService
    {
        public Task<string> Translate(ITranslateable translationModel)
        {
            if (translationModel == null)
                return Task.FromResult(string.Empty);
            if (string.IsNullOrEmpty(translationModel.DefaultValue))
                return Task.FromResult(translationModel.ResourceKey.Split(".").Last());

            return Task.FromResult(string.Format(translationModel.DefaultValue, translationModel.Arguments));
        }
    }
}
