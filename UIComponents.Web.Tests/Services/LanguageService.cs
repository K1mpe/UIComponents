using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIComponents.Abstractions.Interfaces;
using UIComponents.Abstractions.Interfaces.ExternalServices;

namespace UIComponents.Web.Tests.Services
{
    public class LanguageService : ILanguageService
    {
        public Task<string> Translate(ITranslationModel translationModel)
        {
            if (string.IsNullOrEmpty(translationModel.DefaultValue))
                return Task.FromResult(translationModel.ResourceKey.Split(".").Last());

            return Task.FromResult(string.Format(translationModel.DefaultValue, translationModel.Arguments));
        }
    }
}
