using UIComponents.Abstractions.Interfaces.Services;
using UIComponents.Models.Models.Questions;
using static UIComponents.Abstractions.Varia.TranslatableSaver;

namespace UIComponents.Models.Extensions
{
    public static class UICQuestionServiceExtensions
    {
        public static async Task<UICQuestionResult<TResult>> TryAskQuestionToCurrentUser<TResult>(this IUICQuestionService questionService, UICQuestionBase<TResult> question, TimeSpan timeout)
        {
            return await questionService.TryAskQuestionToCurrentUser<UICQuestionBase<TResult>, TResult>(question, timeout);
        }

        public static async Task<UICQuestionResult<TResult>> TryAskQuestion<TResult>(this IUICQuestionService questionService, UICQuestionBase<TResult> question, TimeSpan timeout, object userId)
        {
            return await questionService.TryAskQuestion<UICQuestionBase<TResult>, TResult>(question, timeout, userId);
        }

        public static async Task<UICQuestionResult<TResult>> TryAskQuestion<TResult>(this IUICQuestionService questionService, UICQuestionBase<TResult> question, TimeSpan timeout, List<object> userIds)
        {
            return await questionService.TryAskQuestion<UICQuestionBase<TResult>, TResult>(question, timeout, userIds);
        }
        public static async Task<UICQuestionResult<TResult>> TryAskQuestionToCurrentUser<TResult>(this IUICQuestionService questionService, UICQuestionSelectEnum<TResult> question, TimeSpan timeout) where TResult: Enum
        {
            return await questionService.TryAskQuestionToCurrentUser<UICQuestionSelectEnum<TResult>, TResult>(question, timeout);
        }

        public static async Task<UICQuestionResult<TResult>> TryAskQuestion<TResult>(this IUICQuestionService questionService, UICQuestionSelectEnum<TResult> question, TimeSpan timeout, object userId) where TResult : Enum
        {
            return await questionService.TryAskQuestion<UICQuestionSelectEnum<TResult>, TResult>(question, timeout, userId);
        }

        public static async Task<UICQuestionResult<TResult>> TryAskQuestion<TResult>(this IUICQuestionService questionService, UICQuestionSelectEnum<TResult> question, TimeSpan timeout, List<object> userIds) where TResult : Enum
        {
            return await questionService.TryAskQuestion<UICQuestionSelectEnum<TResult>, TResult>(question, timeout, userIds);
        }

        /// <summary>
        /// Ask the current user to translate the missing translations
        /// </summary>
        public static Task AskCurrentUserToTranslate(this List<TranslatableXmlField> translatables, IUICQuestionService questionService, string languageCode)
        {
            return translatables.TranslateMissing("NL", async (translatable) =>
            {
                var question = UICQuestionText.Create(translatable.Key, translatable.TranslationsList.FirstOrDefault()?.Translation ?? string.Empty, questionService);
                var response = await questionService.TryAskQuestionToCurrentUser(question, TimeSpan.FromMinutes(1));
                if (response.IsValid)
                    return response.Result;
                return null;
            });
        }
    }
}
