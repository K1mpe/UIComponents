using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIComponents.Abstractions.Interfaces.Services;
using UIComponents.Models.Models.Questions;

namespace UIComponents.Generators.Services
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
    }
}
