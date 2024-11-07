using Microsoft.Extensions.Logging;
using System;
using System.Reflection.Emit;
using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Interfaces.Services;
using UIComponents.Generators.Configuration;
using UIComponents.Generators.Helpers;
using UIComponents.Models.Models.Tables;

namespace UIComponents.Generators.Generators;

public class UICGridColumnGenerator : UICGeneratorBase<UICTableColumn, UICTableColumn>
{
    private readonly ILogger _logger;
    private readonly UICConfig _config;
    private readonly IUICLanguageService _languageService;
    public UICGridColumnGenerator(ILogger<UICGridColumnGenerator> logger, UICConfig config, IUICLanguageService languageService)
    {
        _logger = logger;
        _config = config;
        _languageService = languageService;
    }

    public override double Priority { get; set; } = 1000;

    /// <summary>
    /// This function assumes that both the args and existingResult are the same.
    /// </summary>
    public override async Task<IUICGeneratorResponse<UICTableColumn>> GetResponseAsync(UICTableColumn args, UICTableColumn? existingResult)
    {
        if (args.PropertyInfo == null)
            return GeneratorHelper.Success(args, true);

        if (args.Title != null && !string.IsNullOrEmpty(args.Type))
        {
            _logger.LogDebug("GridColumnGenerator has been skipped for {0} since this already has a type and title", args.PropertyInfo.Name);
            return GeneratorHelper.Success(args, true);
        }

        

        var propType = await _config.GetPropertyTypeAsync(args.PropertyInfo, new())??UICPropertyType.String;

        UICInheritAttribute.TryGetInheritPropertyInfo(args.PropertyInfo, out var inheritPropInfo);
        if (args.Title == null)
            args.Title = UIComponents.Defaults.TranslationDefaults.TranslateProperty(inheritPropInfo, propType);

        UICInput input = null;
        
        try
        {
            var propertyArgs = new UICPropertyArgs(
                Activator.CreateInstance(args.PropertyInfo.ReflectedType),
                args.PropertyInfo,
                propType,
                new(),
                new(UICGeneratorPropertyCallType.PropertyInput, args, null),
                _config
            );
             input = await _config.GetGeneratedResultAsync<IUIComponent, UICInput>(UICGeneratorPropertyCallType.PropertyInput, null, propertyArgs);
        } catch(Exception ex)
        {
            _logger.LogError(ex, "Failed to get input for property {0}=>{1} \r\n{2}",
                args.PropertyInfo.ReflectedType?.Name,
                args.PropertyInfo.Name,
                ex.Message
                );
        }

        if (args.ParentTable?.EnableSpansAndTooltips ?? Defaults.Models.Table.UICTable.EnableSpansAndTooltips)
        {
            try
            {
                if (args.Tooltip == null)
                {
                    var tooltipArgs = new UICPropertyArgs(
                        Activator.CreateInstance(args.PropertyInfo.ReflectedType),
                        args.PropertyInfo,
                        propType,
                        new(),
                        new(UICGeneratorPropertyCallType.PropertyTooltip, args, null),
                        _config
                    );

                    var tooltip = await _config.GetToolTipAsync(tooltipArgs, args);
                    if (tooltip != null)
                    {
                        args.Tooltip = tooltip;
                        SetTooltipIcon();

                    }
                }
                if (args.Tooltip == null)
                {
                    var tooltipArgs2 = new UICPropertyArgs(
                        Activator.CreateInstance(args.PropertyInfo.ReflectedType),
                        args.PropertyInfo,
                        propType,
                        new(),
                        new(UICGeneratorPropertyCallType.PropertyTooltip, args, null),
                        _config
                    );

                    var span = await _config.GetPropertyGroupSpanAsync(tooltipArgs2, args);
                    if (span != null)
                    {
                        args.Tooltip = span.Text;
                        SetTooltipIcon();
                    }
                }
                if (args.Tooltip == null && (args.ParentTable?.EnableHeaderAsTooltip??Defaults.Models.Table.UICTable.EnableHeaderAsTooltip))
                {
                    args.Tooltip = args.Title;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to get Tooltip for property {0}=>{1} \r\n{2}",
                    args.PropertyInfo.ReflectedType?.Name,
                    args.PropertyInfo.Name,
                    ex.Message
                    );
            }
            
        }




        if (string.IsNullOrEmpty(args.Type))
        {
            args.Type = propType.ToString().ToLower();
            

            switch (propType)
            {
                case UICPropertyType.String:
                    args.Type = "text";
                    break;

                case UICPropertyType.MultilineText:
                    args.Type = "text";
                    break;

                case UICPropertyType.SelectList:
                    if ((args.SelectListItems == null || args.SelectListItems.Any()) && input is UICInputSelectList selectlist)
                        args.SelectListItems = selectlist.SelectListItems.Select(x => new SelectListItem(x.Text, x.Value?.ToString(), false, x.Disabled)).ToList();
                    break;

                case UICPropertyType.Number:
                    if(input is UICInputNumber numberinput)
                    {
                        if (args.MinValue == null && numberinput.ValidationMinValue != null)
                            args.MinValue = numberinput.ValidationMinValue;
                        if(args.MaxValue == null && numberinput.ValidationMaxValue != null)
                            args.MaxValue = numberinput.ValidationMaxValue;
                        args.Nullable = numberinput.ValidationRequired;
                    }
                    break;
                case UICPropertyType.Decimal:
                    if (args.Step == null)
                        args.Step = "any";
                    if (input is UICInputNumber decimalInput)
                    {
                        if (args.MinValue == null && decimalInput.ValidationMinValue != null)
                            args.MinValue = decimalInput.ValidationMinValue;
                        if (args.MaxValue == null && decimalInput.ValidationMaxValue != null)
                            args.MaxValue = decimalInput.ValidationMaxValue;
                        args.Nullable = decimalInput.ValidationRequired;
                    }
                    break;

                case UICPropertyType.DateOnly:
                case UICPropertyType.DateTime:
                    if (input is UICInputDatetime dateTime)
                    {
                        if (args.MinValue == null && dateTime.ValidationMinimumDate != null)
                            args.MinValue = dateTime.ValidationMinimumDate;
                        if (args.MaxValue == null && dateTime.ValidationMaximumDate != null)
                            args.MaxValue = dateTime.ValidationMaximumDate;
                        args.Nullable = dateTime.ValidationRequired;
                        switch (dateTime.Precision)
                        {
                            case UICDatetimeStep.Date:
                                args.Format ??= "L";
                                break;
                            case UICDatetimeStep.Minute:
                                args.Format ??=  "L LT";
                                args.Step ??= "60";
                                break;
                            case UICDatetimeStep.Second:
                                args.Format ??= "L LTS";
                                args.Step ??= "1";
                                break;
                            case UICDatetimeStep.Millisecond:
                                args.Format ??= "L HH:mm:ss.SSS";
                                args.Step ??= ".001";
                                break;
                        }
                    }
                    break;
                case UICPropertyType.TimeOnly:
                    if(input is UICInputTime time)
                    {
                        switch (time.Precision)
                        {
                            case UICTimeonlyEnum.Minute:
                                args.Step ??= (time.Step*60).ToString();
                                args.Format ??= "LT";
                                break;
                            case UICTimeonlyEnum.Second:
                                args.Step ??= (time.Step).ToString();
                                args.Format ??= "LTS";
                                break;
                            case UICTimeonlyEnum.Milliseconds:
                                args.Step ??= $".{time.Step}";
                                args.Format ??= "HH:mm:ss.SSS";
                                break;
                        }
                    }
                    break;
                case UICPropertyType.TimeSpan:
                    break;
                case UICPropertyType.Boolean:
                    args.Type = "toggle";
                    args.Nullable = false;
                    break;
                case UICPropertyType.ThreeStateBoolean:
                    args.Type = "toggle";
                    args.Nullable = true;
                    break;
                case UICPropertyType.HexColor:
                    break;
            }
        }
        return GeneratorHelper.Success(args, true);

        async Task SetTooltipIcon()
        {
            if (!args.AddTooltipIconInHeader)
                return;
            var icon = new UICIcon(IconDefaults.TooltipIcon.Icon);
            var tooltipClass = args.PropertyInfo.GetInheritAttribute<UICTooltipIconAttribute>();
            if (tooltipClass != null)
                icon = new(tooltipClass.IconClass);
            icon.AddClass("tooltip-icon");
            var headerText = await _languageService.Translate(args.Title);
            args.Title = $"{headerText} <i class=\"{icon.Icon} {icon.GetAttribute("class")}\"></i>";
        }
    }
}
