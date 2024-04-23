using System.Linq.Expressions;
using System.Reflection;

namespace UIComponents.Generators.Helpers;

public static class InternalGeneratorHelper
{

    /// <summary>
    /// Check if the type matches T, if not this will throw a exception
    /// </summary>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    public static void CheckType<T>(Type type)
    {
        if(type == null)
            throw new ArgumentNullException();

        if (!type.IsAssignableTo(typeof(T)))
            throw new ArgumentException($"{type.Name} is not assignable to {nameof(T)}");
    }


}
