namespace UIComponents.Abstractions.Interfaces.Services;


public interface IUICPermissionService
{
    /// <summary>
    /// Check if the user can view this object
    /// </summary>
    Task<bool> CanViewObject<T>(T obj) where T : class;

    /// <summary>
    /// CHeck if the current user can view this property
    /// </summary>
    Task<bool> CanViewProperty<T>(T obj, string propertyName) where T : class;

    /// <summary>
    /// Check if the current user can create a new instance of this type
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    Task<bool> CanCreateType(Type type);

    /// <summary>
    /// Check if the user can edit this object. 
    /// </summary>
    Task<bool> CanEditObject<T>(T oldObject) where T : class;

    /// <summary>
    /// Check if the current user can edit a property of this object
    /// </summary>
    Task<bool> CanEditProperty<T>(T obj, string propertyName) where T : class;

    /// <summary>
    /// Check if the user can delete this object
    /// </summary>
    Task<bool> CanDeleteObject<T>(T obj) where T : class;

}
