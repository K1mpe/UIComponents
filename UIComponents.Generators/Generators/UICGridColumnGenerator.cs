using Microsoft.Extensions.Logging;
using UIComponents.Generators.Configuration;
using UIComponents.Generators.Helpers;
using UIComponents.Models.Models.Tables;

namespace UIComponents.Generators.Generators;

public class UICGridColumnGenerator : UICGeneratorBase<UICTableColumn, UICTableColumn>
{
    private readonly ILogger _logger;
    private readonly UICConfig _config;
    public UICGridColumnGenerator(ILogger<UICGridColumnGenerator> logger, UICConfig config)
    {
        _logger = logger;
        _config = config;
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
        if(args.Title == null)
            args.Title = UIComponents.Defaults.TranslationDefaults.TranslateProperty(inheritPropInfo, propType);

        if(string.IsNullOrEmpty(args.Type))
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
                    args.Sorter = "number";
                    if (args.SelectListItems == null || args.SelectListItems.Any())
                        args.SelectListItems = await _config.GetSelectListItems(inheritPropInfo, true);
                    break;

                case UICPropertyType.Number:
                    if (string.IsNullOrEmpty(args.MaxWidth))
                        args.MaxWidth = "3rem";
                    if (args.HorizontalAlignment == null)
                        args.HorizontalAlignment = UICHorizontalAlignment.Center;
                    break;
                case UICPropertyType.Decimal:
                    if (string.IsNullOrEmpty(args.MaxWidth))
                        args.MaxWidth = "3rem";
                    if (args.HorizontalAlignment == null)
                        args.HorizontalAlignment = UICHorizontalAlignment.Center;
                    break;

                case UICPropertyType.DateOnly:
                    break;
                case UICPropertyType.DateTime:
                    break;
                case UICPropertyType.TimeOnly:
                    break;
                case UICPropertyType.Timespan:
                    break;
                case UICPropertyType.Boolean:
                    break;
                case UICPropertyType.ThreeStateBoolean:
                    break;
                case UICPropertyType.HexColor:
                    break;
            }
            if (string.IsNullOrEmpty(args.Sorter))
                args.Sorter = "string";
        }
        return GeneratorHelper.Success(args, true);
    }
}
