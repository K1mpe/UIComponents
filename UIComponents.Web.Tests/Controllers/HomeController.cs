using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Reflection;
using UIComponents.Abstractions;
using UIComponents.Abstractions.DataTypes;
using UIComponents.Abstractions.DataTypes.RecurringDates;
using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Interfaces;
using UIComponents.Abstractions.Interfaces.FileExplorer;
using UIComponents.Abstractions.Interfaces.Services;
using UIComponents.Abstractions.Models;
using UIComponents.Abstractions.Models.HtmlResponses;
using UIComponents.Abstractions.Varia;
using UIComponents.Defaults;
using UIComponents.Defaults.Models.Graphs;
using UIComponents.Generators.Interfaces;
using UIComponents.Generators.Services;
using UIComponents.Models.Extensions;
using UIComponents.Models.Models;
using UIComponents.Models.Models.Actions;
using UIComponents.Models.Models.Buttons;
using UIComponents.Models.Models.Card;
using UIComponents.Models.Models.FileExplorer;
using UIComponents.Models.Models.Graphs.TimeLineGraph;
using UIComponents.Models.Models.Icons;
using UIComponents.Models.Models.Inputs;
using UIComponents.Models.Models.Questions;
using UIComponents.Models.Models.Texts;
using UIComponents.Models.Models.Tree;
using UIComponents.Web.Tests.Factory;
using UIComponents.Web.Tests.Models;
using UIComponents.Web.Tests.Services;
using UIComponents.Web.Tests.Validators;

namespace UIComponents.Web.Tests.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUIComponentGenerator _uic;
        private readonly IUICLanguageService _languageService;
        private readonly SignalRService _signalRService;
        private readonly IUICQuestionService _uicQuestionService;
        private readonly TestModelValidator _validator;
        private readonly TestService _testService;
        private readonly IUICFileExplorerPathMapper _pathMapper;
        private readonly IUICAskUserToTranslate _uICAskUserToTranslate;
        private readonly IUICStoredComponents _storedComponents;
        public HomeController(ILogger<HomeController> logger, IUIComponentGenerator uic, IUICLanguageService languageService, SignalRService signalRService, IUICQuestionService uicQuestionService, TestModelValidator validator, TestService testService, IUICFileExplorerPathMapper pathMapper, IUICAskUserToTranslate uICAskUserToTranslate, IUICStoredComponents storedComponents)
        {
            _logger = logger;
            _uic = uic;
            _languageService = languageService;
            _signalRService = signalRService;
            _uicQuestionService = uicQuestionService;
            _validator = validator;
            _testService = testService;
            _pathMapper = pathMapper;
            _uICAskUserToTranslate = uICAskUserToTranslate;
            _storedComponents = storedComponents;
        }

        public static int Counter { get; set; } = 0;

        public async Task<IActionResult> Index()
        {
            return View();
            var testModel = new TestModel();
            var component = await _uic.CreateComponentAsync(testModel, new()
            {
                CheckboxRenderer = UIComponents.Models.Models.Inputs.CheckboxRenderer.ToggleSwitch,
                SelectlistSearchableForItems = 2,
                ShowCardHeaders = true,
                FormToolbarInCardFooter = true,
                PostForm= new UICActionGetPost(UICActionGetPost.ActionTypeEnum.Post, "Home", "Post", new {Test="blub"})
            });
            (component as UIComponent).AddAttribute("min-width", "400px").AddAttribute("max-width", "1000px");

            component.FindInputGroupByPropertyName(nameof(TestModel.Decimal)).AppendInput.Add(new UICSpan(new Untranslated("km")));

            var test2Model = new TestModel2();

            await Task.Delay(5000);
            return ViewOrPartial(component);
        }

        public async Task<IActionResult> Test()
        {
            //var component = await _uic.CreateComponentAsync(new TestModel());
            //return ViewOrPartial(component);
            await Task.Delay(500);
            await new UICToastr(IUICToastNotification.ToastType.Success, "message", "title")
            {
                Position = UICToastr.ToastPosition.BottomFullWidth
            }.SendToUser(_storedComponents, 1);
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
        public async Task<IActionResult> Post(TestModel post)
        {
            try
            {
                _testService.IncreaseNumberByOne();
                var validation = _validator.Validate(post);
                if (!validation.IsValid)
                {
                    return this.ValidationErrors(validation);
                    //var errors = validation.ValidationErrors();
                    //return Json(errors);
                }

                var yesNo = UICQuestionYesNo.Create("Test Ja / nee", "Wilt u deze vraag beantwoorden?", _uicQuestionService, question => question.Icon = QuestionIconType.Warning);

                var answered = await _uicQuestionService.TryAskQuestion(yesNo, TimeSpan.FromMinutes(10), 1);
                if (answered.IsValid && answered.Result)
                {
                    var dayOfWeek = UICQuestionSelectEnum<DayOfWeek>.Create("Favorite day", "What is your favorite day?", _uicQuestionService, question =>
                    {
                        question.Icon = QuestionIconType.Info;
                        question.CanCancel = false;
                    });
                    var answered2 =  await _uicQuestionService.TryAskQuestion(dayOfWeek, TimeSpan.FromMinutes(1),  1);
                    if (answered2.IsValid)
                    {
                        if(answered2.Result == DayOfWeek.Saturday ||answered2.Result == DayOfWeek.Sunday)
                        {
                            Console.WriteLine("In weekend");
                        }

                        var question3 = UICQuestionText.Create("Test Texkt", "Type something", _uicQuestionService);
                        var answered3 = await _uicQuestionService.TryAskQuestionToCurrentUser(question3, TimeSpan.FromMinutes(1));
                        if (answered3.IsValid)
                        {

                        }
                    }
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
            group.Add(out var storage, new UICHtmlStorage("SelectList",
                new UICActionGet("Home", "SelectListData"),
                new UICActionGet("Home", "GetSelectListTime")));

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

            var selectStorage = new UICInputSelectList()
            {
                Color = new UICColor("orange"),
            }.AddSource(storage);
            group.Add(selectStorage);

            group.Add(new UICInputMultiSelect().AddSource(storage));
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

        public IActionResult GetSelectListTime()
        {
            var now = DateTime.Now;
            return Json(now.ToString("yyyyMMddHH"));
        }


        [HttpGet]
        public async Task<IActionResult> TestSubClass()
        {
            var item = new TestModel();
            item.Number = 15;
            item.Checkbox = true;
            item.IntList = new();
            item.ObjectList = new()
            {
                new(){ Number = 1},
                new(){
                    Number = 2,
                },
            };
            var component = await _uic.CreateComponentAsync(item, new()
            {
                PostObjectAsDefault = true,
                SubClassesInCard = new(),
                ShowEditButton = false,
                StartInCard = new UICCard("blub"),
                PostForm= new UICActionPost("/home/Post"),
            });
            component.TryFindInputByPropertyName<UICInputDatetime>(nameof(TestModel.MyDateTime), input =>
            {
                input.ValidationMinimumDate = new DateTime(2024, 12, 1);
                input.ValidationMaximumDate = new DateTime(2024, 12, 31);
                input.ValidationRequired = true;
            });
            component.TryFindInputByPropertyName<UICInputMultiline>(nameof(TestModel.Description), input =>
            {
                input.ValidationMinLength = 3;
                input.ValidationMaxLength = 8;
                input.ValidationRequired = true;
            });
            return ViewOrPartial(component);
        }


        [HttpGet]
        public IActionResult Files()
        {
            var fileBrowser = new UICFileExplorer()
            {
                RootDirectory = "C:\\Jonas",
            }.AddAllAddons();
            _pathMapper.RegisterPath("C:");

            UICFileExplorerService.IsDirectory("C:\\Jonas", false);
            UICFileExplorerService.IsDirectory("C:\\Jonas\\blub", false);

            var a1 = new DirectoryInfo("C:\\Jonas\\");
            var a2 = new DirectoryInfo("C:\\Jonas");

            return ViewOrPartial(fileBrowser);
        }

        [HttpGet]
        public async Task<IActionResult> GetNavLeft()
        {
            await Task.Delay(TimeSpan.FromSeconds(10));
            return PartialView("/Views/Shared/Navigation/_LeftStored.cshtml");
        }

        [HttpGet]
        public IActionResult GetNavLeftTime()
        {
            var now = DateTime.Now;
            return Json(now.ToString("yyyyMMddHH"));
        }

        [HttpGet]
        public async Task<IActionResult> Translate()
        {
            await _uICAskUserToTranslate.AskCurrentUserToTranslate(TranslatableSaver.UICTranslationFilePath);
            //var translations = await TranslatableSaver.LoadFromUICAsync();
            //await translations.AskCurrentUserToTranslate(_uicQuestionService, "NL");

            //var dir = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.FullName;
            //await TranslatableSaver.SaveToFileAsync(translations, $"{dir}\\UIComponents.Web\\UIComponents\\Translations.json", false, false);

            return Json(true);
        }
    }
}