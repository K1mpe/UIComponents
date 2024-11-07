using Microsoft.AspNetCore.Mvc;
using UIComponents.Abstractions.Interfaces.Services;
using UIComponents.Models.Models.Questions;
using UIComponents.Web.Tests.Models;

namespace UIComponents.Web.Tests.Controllers;

public class TestModelController : Controller
{
    private readonly IUICQuestionService _uICQuestionService;

    public TestModelController(IUICQuestionService uICQuestionService)
    {
        _uICQuestionService = uICQuestionService;
    }

    private static List<TestModel> Items { get; } = new List<TestModel>()
    {
        new(){TestString = "Item1", MyDateTime= DateTime.Now, Decimal = 4, Color= "#ff0000"},
        new(){TestString = "Item2", Checkbox = true, ThreeStateBool = true},
        new(){TestString = "Item3", TimeSpan = TimeSpan.FromHours(2).Add(TimeSpan.FromMinutes(14))},
        new(){ TestString = "RandomItem1", MyDateTime = DateTime.Now.AddDays(-1), Decimal = 7.8, Color = "#00ff00" },
        new(){ TestString = "RandomItem2", Checkbox = false, ThreeStateBool = null, Number = 15, Decimal = 3.14, TimeSpan = TimeSpan.FromDays(1) },
        new(){ TestString = "RandomItem3", Description = "This is a random description.", Number = 18, Decimal = 9.99, MyDateTime = DateTime.Now.AddHours(3), Date = DateTime.Today.AddDays(5) },
        new(){ TestString = "RandomItem4", Checkbox = true, ThreeStateBool = false, Number = 12, Decimal = 2.5, TimeSpan = TimeSpan.FromMinutes(30) },
        new(){ TestString = "RandomItem5", Description = "Lorem ipsum dolor sit amet.", Number = 20, Decimal = 6.78, MyDateTime = DateTime.Now.AddDays(-3), Color = "#0000ff" },
        new(){ TestString = "RandomItem6", Checkbox = false, ThreeStateBool = true, Number = 16, Decimal = 5.55, TimeSpan = TimeSpan.FromHours(1) },
        new(){ TestString = "RandomItem7", Description = "Consectetur adipiscing elit.", Number = 11, Decimal = 1.23, MyDateTime = DateTime.Now.AddMonths(1), Date = DateTime.Today.AddDays(10) },
        new(){ TestString = "RandomItem8", Checkbox = true, ThreeStateBool = null, Number = 19, Decimal = 8.88, TimeSpan = TimeSpan.FromMinutes(45) },
        new(){ TestString = "RandomItem9", Description = "Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.", Number = 13, Decimal = 4.56, MyDateTime = DateTime.Now.AddDays(-5), Color = "#ffff00" },
        new(){ TestString = "RandomItem10", Checkbox = false, ThreeStateBool = false, Number = 14, Decimal = 3.33, TimeSpan = TimeSpan.FromDays(2) },
        new(){ TestString = "RandomItem11", Description = "Ut enim ad minim veniam.", Number = 17, Decimal = 7.77, MyDateTime = DateTime.Now.AddHours(6), Date = DateTime.Today.AddDays(3) },
        new(){ TestString = "RandomItem12", Checkbox = true, ThreeStateBool = true, Number = 10, Decimal = 9.87, TimeSpan = TimeSpan.FromHours(2) },
        new(){ TestString = "RandomItem13", Description = "Quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.", Number = 15, Decimal = 2.34, MyDateTime = DateTime.Now.AddDays(2), Color = "#ff00ff" }

    };


    public IActionResult LoadData(IFormCollection formCollection)
    {
        //Thread.Sleep(3000);
        List<TestModel> items = new();
        for(int i=0; i < 1000; i++)
        {
            items.AddRange(Items);
        }
        for(int j= 0; j < items.Count; j++)
        {
            items[j].Number = j;
        }
        return Json(items);
    }

    public IActionResult Insert(TestModel testModel)
    {
        Items.Add(testModel);
        return Json(true);
    }
    public IActionResult Update(TestModel testModel)
    {
        var item = Items.Where(x=>x.TestString == testModel.TestString).FirstOrDefault();
        if (item == null)
            return Json(false);
        
        item.Description = testModel.Description;
        item.Checkbox = testModel.Checkbox;
        item.ThreeStateBool = testModel.ThreeStateBool;
        item.Number = testModel.Number;
        item.Decimal = testModel.Decimal;
        item.Description = testModel.Description;
        item.TimeSpan = testModel.TimeSpan;
        item.Enum = testModel.Enum;
        item.Color = testModel.Color;
        item.MyDateTime = testModel.MyDateTime;
        item.Date = testModel.Date;
        return Json(true);
    }

    public IActionResult Delete(TestModel testModel)
    {
        var matching = Items.Where(x => x.TestString == testModel.TestString).FirstOrDefault();
        Items.Remove(matching);
     
        return Json(true);
    }
}
