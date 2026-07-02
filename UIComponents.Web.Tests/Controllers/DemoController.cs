using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.ComponentModel.DataAnnotations;
using UIComponents.Abstractions.Interfaces.FileExplorer;
using UIComponents.Abstractions.Models.HtmlResponses;
using UIComponents.Models.Models.FileExplorer;

namespace UIComponents.Web.Tests.Controllers;

public class DemoController : BaseController
{
    private readonly IUIComponentGenerator _uiGenerator;
    private readonly IUICFileExplorerPathMapper _explorerMapper;

    public DemoController(IUIComponentGenerator uiGenerator, IUICFileExplorerPathMapper explorerMapper)
    {
        _uiGenerator = uiGenerator;
        _explorerMapper = explorerMapper;
    }

    public async Task<IActionResult> Index()
    {
        var demoModel = new DemoModel();

        var ui = await _uiGenerator.CreateComponentAsync(demoModel, new()
        {
            StartInCard = new(),
            PostForm = new UICActionPost("Demo/Post")
        });

        return ViewOrPartial(ui);
    }

    public IActionResult Post(DemoModel demo)
    {
        return Ok();
        var toast = new UICToastResponse()
        {
            Notification = new UICToastRNotification()
            {
                Message = $"Post for {demo.Name} successfull",
                Type = IUICToastNotification.ToastType.Success
            }
        };
        return Json(toast);
    }
}

public class DemoModel
{
    public string Name { get; set; }
    public string Color { get; set; }
    public int Age { get; set; } = 10;

    public DateTime Date { get; set; } = DateTime.Today;

    public List<User> Users { get; set; } = new();

}


public enum TestEnum
{
    Option1,
    Option2
}


public class User
{
    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [UICPrecisionDate(UICDatetimeStep.Date)]
    public DateTime DateOfBirth { get; set; } = DateTime.Today;

}