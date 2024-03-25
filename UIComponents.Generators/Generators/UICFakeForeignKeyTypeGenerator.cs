using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIComponents.Generators.Helpers;

namespace UIComponents.Generators.Generators;

public class UICFakeForeignKeyTypeGenerator : UICGeneratorBase<PropertyInfo, Type?>
{
    public override double Priority { get; set; } = 1000;

    public override async Task<IUICGeneratorResponse<Type?>> GetResponseAsync(PropertyInfo args, Type? existingResult)
    {
        if (existingResult != null)
            return GeneratorHelper.Next<Type?>();

        var fakeForeignKeyAttr = args.GetCustomAttribute<FakeForeignKeyAttribute>();
        if (fakeForeignKeyAttr == null)
            return GeneratorHelper.Next<Type?>();

        return GeneratorHelper.Success<Type?>(fakeForeignKeyAttr.Type, true);
    }
}
