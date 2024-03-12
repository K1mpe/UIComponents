using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIComponents.Abstractions.Interfaces.Services;

namespace UIComponents.Models.Models.Questions
{
    public class UICQuestionSelectEnum<T> : UICQuestionSelectList, IUIQuestionComponent<T> where T : Enum
    {

        #region Ctor
        public UICQuestionSelectEnum() : base()
        {
            foreach(var value in Enum.GetNames(typeof(T)))
            {
                SelectListItems.Add(new(value, value));
            }
        }
        public UICQuestionSelectEnum(Translatable title, Translatable message) : this()
        {
            Title = title;
            Message = message;
        }
        #endregion

        #region Methods

        public static UICQuestionSelectEnum<T> Create(Translatable title, Translatable message, IUICQuestionService questionService, Action<UICQuestionSelectEnum<T>> action = null)
        {
            var instance = new UICQuestionSelectEnum<T>(title, message);
            instance.AssignClickEvents(questionService);
            action?.Invoke(instance);
            return instance;
        }

        public T MapResponse(string response)
        {
            return (T)Enum.Parse(typeof(T), response);
        }

        #endregion
    }
}
