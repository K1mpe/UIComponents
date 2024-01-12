namespace UIComponents.Abstractions.Interfaces.ExternalServices;


public interface IPermissionCurrentUserService : IViewPermissionCurrentUserService, ICreatePermissionCurrentUserService, IEditPermissionCurrentUserService, IDeletePermissionCurrentUserService
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
    /// Check if the user can view this type.
    /// <br>If the <paramref name="id"/> is provided, this will overload to <see cref="CanView(object, int?)"/></br>
    /// </summary>
    Task<bool> CanView(Type type, int id = 0);



    /// <summary>
    /// Use the <see cref="CanViewProperties{T}(T, int?)"/> for a specific property
    /// </summary>
    Task<bool> CanViewProperty<T>(T obj, string propertyName) where T : class;

    /// <summary>
    /// Use the <see cref="CanViewProperties(Type, int?)"/> for a specific property
    /// </summary>
    /// <remarks>
    /// Use <see cref="CanViewProperty{T}(T, string, int?)"/> instead if possible
    /// </remarks>
    Task<bool> CanViewProperty(Type type, string propertyName);



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

    /// <summary>
    /// Check if the user can create a object of this type
    /// </summary>
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
    /// Check if the user can edit this type.
    /// <br>If the <paramref name="id"/> is provided, this will overload to <see cref="CanEdit(object, object, int?)"/></br>
    /// <br>The entity will be used as <see langword="oldObject"/> while <see langword="newObject"/> will be null </br>
    /// </summary>
    Task<bool> CanEdit(Type type, int id = 0);




    /// <summary>
    /// Use the <see cref="CanEditProperties{T}(T, int?)(Type, int?)"/> for a specific property
    /// </summary>
    Task<bool> CanEditProperty<T>(T obj, string propertyName) where T : class;

    /// <summary>
    /// Use the <see cref="CanEditProperties(Type, int?)"/> for a specific property
    /// </summary>
    /// <remarks>
    /// Use <see cref="CanEditProperty{T}(T, string, int?)"/> instead if possible
    /// </remarks>
    Task<bool> CanEditProperty(Type type, string propertyName);

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

    /// <summary>
    /// Check if the user can delete this type.
    ///  <br>If the <paramref name="id"/> is provided, this will overload to <see cref="CanDelete(object, int?)"/></br>
    /// </summary>
    Task<bool> CanDelete(Type type, int id = 0);


}
