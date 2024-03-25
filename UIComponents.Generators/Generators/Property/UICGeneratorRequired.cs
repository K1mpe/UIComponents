using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using UIComponents.Generators.Helpers;

namespace UIComponents.Generators.Generators.Property;

public class UICGeneratorRequired : UICGeneratorBase<UICPropertyArgs, bool?>
{

    public override double Priority { get; set; } = 1000;

    public override async Task<IUICGeneratorResponse<bool?>> GetResponseAsync(UICPropertyArgs args, bool? existingResult)
    {
        await Task.Delay(0);
        var v1 = args.PropertyInfo.GetCustomAttribute<RequiredAttribute>();
        if (v1 != null)
            return GeneratorHelper.Success<bool?>(true, false);


        if (args.PropertyType.IsAssignableTo(typeof(Nullable<>)))
            return GeneratorHelper.Success<bool?>(false, false);

        var foreignKey = args.PropertyInfo.GetCustomAttribute<ForeignKeyAttribute>();
        if (foreignKey != null)
            return GeneratorHelper.Success<bool?>(true, false);

        var fakeForeignKey = args.PropertyInfo.GetCustomAttribute<FakeForeignKeyAttribute>();
        if (fakeForeignKey != null)
            return GeneratorHelper.Success<bool?>(fakeForeignKey.IsRequired, false);

        //if (args.PropertyInfo.PropertyType.IsClass && args.PropertyInfo.PropertyType != typeof(string))
        //    return GeneratorHelper.Success<bool?>(true, false);

        if(UICInheritAttribute.TryGetInheritPropertyInfo(args.PropertyInfo, out var inherit))
        {
            var v12 = inherit.GetCustomAttribute<RequiredAttribute>();
            if (v1 != null)
                return GeneratorHelper.Success<bool?>(true, false);


            if (inherit.PropertyType.IsAssignableTo(typeof(Nullable<>)))
                return GeneratorHelper.Success<bool?>(false, false);

            var foreignKey2 = inherit.GetCustomAttribute<ForeignKeyAttribute>();
            if (foreignKey2 != null)
                return GeneratorHelper.Success<bool?>(true, false);

            var fakeForeignKey2 = inherit.GetCustomAttribute<FakeForeignKeyAttribute>();
            if (fakeForeignKey2 != null)
                return GeneratorHelper.Success<bool?>(fakeForeignKey2.IsRequired, false);

            if (inherit.PropertyType.IsClass && inherit.PropertyType != typeof(string))
                return GeneratorHelper.Success<bool?>(true, false);
        }
        return GeneratorHelper.Success<bool?>(false, false);
    }
}
