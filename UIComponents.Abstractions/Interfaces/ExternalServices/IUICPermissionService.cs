namespace UIComponents.Abstractions.Interfaces.ExternalServices;


public interface IUicPermissionService : IViewPermissionCurrentUserService, ICreatePermissionCurrentUserService, IEditPermissionCurrentUserService, IDeletePermissionCurrentUserService
{
}


/// <summary>
/// Interface for <see langword="CanView"/> permission methods
/// </summary>
public interface IViewPermissionCurrentUserService
{
    /// <summary>
    /// Check if the user can view this object
    /// </summary>
    Task<bool> CanView<T>(T obj) where T : class;


    /// <summary>
    /// Use the <see cref="CanViewProperties{T}(T, int?)"/> for a specific property
    /// </summary>
    Task<bool> CanViewProperty<T>(T obj, string propertyName) where T : class;


}


/// <summary>
/// Interface for <see langword="CanCreate"/> permission methods
/// </summary>
public interface ICreatePermissionCurrentUserService
{
    /// <summary>
    /// Check if the user can view this object
    /// <br>the properties of <paramref name="obj"/> could be used for specific validations</br>
    /// </summary>
    /// <remarks>
    /// returns false if <paramref name="obj"/> is null
    /// </remarks>
    Task<bool> CanCreate<T>(T obj) where T : class;

    Task<bool> CanCreate(Type type);
}


/// <summary>
/// Interface for <see langword="CanEdit"/> permission methods
/// </summary>
public interface IEditPermissionCurrentUserService
{

    /// <summary>
    /// Check if the user can edit this object. 
    /// <br>If <paramref name="oldObject"/> is not null, comparison with newObject can be used for specific validations</br>
    /// </summary>
    Task<bool> CanEdit<T>(T newObject, T oldObject = null) where T : class;




    /// <summary>
    /// Use the <see cref="CanEditProperties{T}(T, int?)(Type, int?)"/> for a specific property
    /// </summary>
    Task<bool> CanEditProperty<T>(T obj, string propertyName) where T : class;


}


/// <summary>
/// Interface for <see langword="CanDelete"/> permission methods
/// </summary>
public interface IDeletePermissionCurrentUserService
{

    /// <summary>
    /// Check if the user can delete this object
    /// </summary>
    Task<bool> CanDelete<T>(T obj) where T : class;


}
