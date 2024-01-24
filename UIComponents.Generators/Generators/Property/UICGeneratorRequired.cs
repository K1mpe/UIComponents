using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using UIComponents.Generators.Helpers;

namespace UIComponents.Generators.Generators.Property;

public class UICGeneratorRequired : UICGeneratorBase<UICPropertyArgs, bool>
{

    public override double Priority { get; set; } = 1000;

    public override async Task<IUICGeneratorResponse<bool>> GetResponseAsync(UICPropertyArgs args, bool existingResult)
    {
        await Task.Delay(0);
        var v1 = args.PropertyInfo.GetCustomAttribute<RequiredAttribute>();
        if (v1 != null)
            return GeneratorHelper.Success(true, false);


        if (args.PropertyInfo.PropertyType.IsAssignableTo(typeof(Nullable<>)))
            return GeneratorHelper.Success(false, false);

        var foreignKey = args.PropertyInfo.GetCustomAttribute<ForeignKeyAttribute>();
        if (foreignKey != null)
            return GeneratorHelper.Success(true, false);

        var fakeForeignKey = args.PropertyInfo.GetCustomAttribute<FakeForeignKeyAttribute>();
        if (fakeForeignKey != null)
            return GeneratorHelper.Success(fakeForeignKey.IsRequired, false);

        if (args.PropertyInfo.PropertyType.IsClass && args.PropertyInfo.PropertyType != typeof(string))
            return GeneratorHelper.Success(true, false);


        return GeneratorHelper.Success(false, false);
    }
}
