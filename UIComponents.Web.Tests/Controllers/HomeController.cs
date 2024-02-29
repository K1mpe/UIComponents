using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using UIComponents.Abstractions;
using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Interfaces;
using UIComponents.Abstractions.Interfaces.ExternalServices;
using UIComponents.Abstractions.Models;
using UIComponents.Abstractions.Models.RecurringDates;
using UIComponents.Defaults;
using UIComponents.Defaults.Models.Graphs;
using UIComponents.Generators.Interfaces;
using UIComponents.Models.Extensions;
using UIComponents.Models.Models;
using UIComponents.Models.Models.Actions;
using UIComponents.Models.Models.Buttons;
using UIComponents.Models.Models.Card;
using UIComponents.Models.Models.Graphs.TimeLineGraph;
using UIComponents.Models.Models.Icons;
using UIComponents.Models.Models.Inputs;
using UIComponents.Models.Models.Texts;
using UIComponents.Models.Models.Tree;
using UIComponents.Web.Tests.Factory;
using UIComponents.Web.Tests.Models;

namespace UIComponents.Web.Tests.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUIComponentService _uic;
        private readonly IUicLanguageService _languageService;
        public HomeController(ILogger<HomeController> logger, IUIComponentService uic, IUicLanguageService languageService)
        {
            _logger = logger;
            _uic = uic;
            _languageService = languageService;
        }

        public async Task<IActionResult> Index()
        {
            return View();
            var testModel = new TestModel();
            var component = await _uic.CreateComponentAsync(testModel, new()
            {
                CheckboxRenderer = UIComponents.Models.Models.Inputs.CheckboxRenderer.ToggleSwitch,
                SelectlistSearableForItems = 2,
                ShowCardHeaders = true,
                FormToolbarInCardFooter = true,
                PostForm= new UICActionGetPost(UICActionGetPost.ActionTypeEnum.Post, "Home", "Post", new {Test="blub"})
            });
            (component as UIComponent).AddAttribute("min-width", "400px").AddAttribute("max-width", "1000px");

            component.FindInputGroupByPropertyName(nameof(TestModel.Decimal)).AppendInput.Add(new UICSpan(new Untranslated("km")));

            await Task.Delay(5000);
            return ViewOrPartial(component);
        }

        public async Task<IActionResult> Test()
        {
            await Task.Delay(500);
            if(IsAjaxRequest(Request))
                return PartialView();
            return View();
        }

        public async Task<IActionResult> JsTreeItems(string id, bool initial)
        {
            var treeItems = new UICTreeItems();
            treeItems.Add(new("ajax1"));
            treeItems.Add(new("ajax2"), (item) =>
            {
                item.HasAjaxChildren = false;
            });
            return ViewOrPartial(treeItems);
        }

        [HttpPost]
        public IActionResult Post(TestModel post)
        {
            try
            {
                var nextOccurences = post.RecurringDate.GetNextDates(30);

                var serialized = post.RecurringDate.Serialize();
                var deserialized = RecurringDate.Deserialize(serialized);

                var nextOccurences2 = deserialized.GetNextDates(15);
                return Json(true);
            }
            catch(Exception ex)
            {
                throw;
            }
            
        }

        [HttpPost]
        public IActionResult GetTimelineChartData(RequestLineGraphDataModel request)
        {
            var data =(request.LineGraphId=="blub")? TimelineDataFactory.GetPoint1(request.StartLocal, request.EndLocal): TimelineDataFactory.GetPoint2(request.StartLocal, request.EndLocal);
            
            var x = request.AveragePerTimespan(data);

            Console.WriteLine($"LoadLineGraphData from {request.StartLocal} to {request.EndLocal}, datapoints: {data.Count} => {x.Count()}");
            return Json(x);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult ViewOrPartial(IUIComponent component)
        {
            if (IsAjaxRequest(Request))
                return PartialView("ComponentRender", component);
            return View("ComponentRender", component);
        }

        public static bool IsAjaxRequest(HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Headers != null)
                return request.Headers["X-Requested-With"] == "XMLHttpRequest";

            return false;
        }

        public IActionResult SelectList()
        {
            var group = new UICGroup();
            group.Add(out var multiselect, new UICInputMultiSelect()
            {
                Color = new UICColor("Green"),//Colors.Green
                AllowDynamicOptions = false,
                ClearInputAfterSelecting = true
            }) ;
            
            
            multiselect.SelectListItems.AddRange(new List<UICSelectListItem>()
            {
                new UICSelectListItem(){Value = "1", Text = "one", Group = new(){Name="Group1"}, SearchTag="blub a b c "}.AddPrepend(UICIcon.Delete()),
                new(){Value = "2", Text = "two", Group = new(){Name="Group1"}},
                new(){Value = "3", Text = "one", Group = new UICSelectListGroup(){Name="Group2", Disabled=true}.AddAppend(new UICIcon("fas fa-user"))},
                new(){Value = "4", Text = "four", Group = new(){Name="Group2"}},
                new(){Value = "5", Text = "five", Disabled = true},

            });
            return ViewOrPartial(group);
        }
    }
}