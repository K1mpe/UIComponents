using Microsoft.AspNetCore.Mvc.Rendering;
using UIComponents.Abstractions.Interfaces.Services;

namespace UIComponents.Models.Models.Questions;

public class UICQuestionSelectList : UICQuestionBase
{
    #region Fields

    public override string RenderLocation => UIComponent.DefaultIdentifier(nameof(UICQuestionSelectList));

    #endregion

    #region Ctor
    public UICQuestionSelectList() : base()
    {

    }
    public UICQuestionSelectList(Translatable title, Translatable message, List<SelectItem> items) : base(title, message)
    {
        SelectListItems = items;
    }
    #endregion

    #region Properties

    public List<SelectItem> SelectListItems { get; set; } = new();

    public Translatable EmptyText { get; set; }

    #endregion

    #region Methods

    public static UICQuestionSelectList Create(Translatable title, Translatable message, List<SelectItem> items, IUICQuestionService questionService)
    {
        var instance = new UICQuestionSelectList(title, message, items);
        return instance.AssignClickEvents(questionService);

    }
    public UICQuestionSelectList AssignClickEvents(IUICQuestionService questionService)
    {
        base.AssignClickEvents(questionService);
        
        return this;
    }


    #endregion

    public class SelectItem
    {
        public object Value { get; set; }
        public Translatable Text { get; set; }

        public static implicit operator SelectItem(SelectListItem item) => new() { Value = item.Value, Text = item.Text };
    }
}
