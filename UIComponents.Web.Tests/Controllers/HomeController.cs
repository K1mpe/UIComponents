using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using UIComponents.Abstractions;
using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Interfaces;
using UIComponents.Abstractions.Interfaces.Services;
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
using UIComponents.Models.Models.Questions;
using UIComponents.Models.Models.Texts;
using UIComponents.Models.Models.Tree;
using UIComponents.Web.Tests.Factory;
using UIComponents.Web.Tests.Models;
using UIComponents.Web.Tests.Services;

namespace UIComponents.Web.Tests.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUIComponentService _uic;
        private readonly IUICLanguageService _languageService;
        private readonly SignalRService _signalRService;
        private readonly IUICQuestionService _uicQuestionService;
        public HomeController(ILogger<HomeController> logger, IUIComponentService uic, IUICLanguageService languageService, SignalRService signalRService, IUICQuestionService uicQuestionService)
        {
            _logger = logger;
            _uic = uic;
            _languageService = languageService;
            _signalRService = signalRService;
            _uicQuestionService = uicQuestionService;
        }

        public static int Counter { get; set; } = 0;

        public async Task<IActionResult> Index()
        {
            var translatable = new Translatable("blub", "test {0}", new Translatable("foo", "foo {1}", "abc"));
            var serialised = translatable.Serialize();
            var x = (Translatable)serialised;
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
                var yesNo = UICQuestionYesNo.Create("Test Ja / nee", "Wilt u deze vraag beantwoorden?", _uicQuestionService, question => question.Icon = QuestionIconType.Warning);

                var answered = _uicQuestionService.TryAskQuestion(yesNo, TimeSpan.FromMinutes(1), 1, out bool boolean);
                if (boolean)
                {
                    var dayOfWeek = UICQuestionSelectEnum<DayOfWeek>.Create("Favorite day", "What is your favorite day?", _uicQuestionService, question =>
                    {
                        question.Icon = QuestionIconType.Info;
                        question.CanCancel = false;
                    });
                    answered = _uicQuestionService.TryAskQuestion(dayOfWeek, TimeSpan.FromMinutes(1),  1, out DayOfWeek favoriteDay);

                }


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
            Counter++;
            if (Counter % 20 > 5 && request.LineGraphId == "blub")
                throw new Exception("Test");
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
            var multiselect = new UICInputMultiSelect()
            {
                Color = new UICColor("Green"),//Colors.Green
                AllowDynamicOptions = false,
                ClearInputAfterSelecting = true
            };
            multiselect.AddSource(out var source, new UICActionGet("Home", "SelectListData"));

            group.Add(new UICInputGroup("Test", multiselect), inputgroup =>
            {
                inputgroup.PrependInput.Add(new UICButton("Refresh")
                {
                    OnClick = source.TriggerRefresh()
                });
                inputgroup.AppendInput.Add(new UICIcon("fas fa-user"));
            });
            
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

        [HttpGet]
        public IActionResult SelectListData()
        {
            Counter++;
            if(Counter %2 == 0)
            {
                var items = new List<UICSelectListItem>()
                {
                    new UICSelectListItem(){Value = "1", Text = "one", Group = new(){Name="Group1"}, SearchTag="blub a b c "}.AddPrepend(UICIcon.Delete()),
                    new UICSelectListItem(){Value = "2", Text = "two", Group = new(){Name="Group1"}}.AddAttribute("data-test", "abc"),
                    new UICSelectListItem(){Value = "3", Text = "one", Group = new UICSelectListGroup(){Name="Group2", Disabled=true}.AddAppend(new UICIcon("fas fa-user"))},
                    new UICSelectListItem(){Value = "4", Text = "four", Group = new(){Name="Group2"}, SearchTag = "4"},
                    new UICSelectListItem(){Value = "5", Text = "five", Disabled = true},
                };
                return Json(items);
            }
            else
            {
                var items = new List<SelectListItem>()
                {
                    new(){Value="6", Text="Six"},
                    new(){Value="7", Text="Seven"},
                    new(){Value="8", Text="Eight"},
                    new(){Value="9", Text="Nine"},
                    new(){Value="10", Text="Ten"},
                };
                return Json(items);
            }
        }


        [HttpGet]
        public async Task<IActionResult> TestSubClass()
        {
            var item = new TestModel();
            item.Number = 15;
            item.Checkbox = true;

            var component = await _uic.CreateComponentAsync(item, new()
            {
                PostObjectAsDefault = true,
                IncludedProperties = "TestString, Description, Date",
                //SubClassesInCard = new(),
                //InputGroupSingleRow = false,
                //ReplaceSaveButtonWithCreateButton = true,
                PostForm= new UICActionPost("/home/Post", new {Number = 3})
            });

            return ViewOrPartial(component);
        }
    }
}