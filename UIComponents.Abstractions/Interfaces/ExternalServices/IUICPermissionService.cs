namespace UIComponents.Abstractions.Interfaces.ExternalServices;


public interface IUicPermissionService
{
    /// <summary>
    /// Check if the user can view this object
    /// </summary>
    Task<bool> CanView<T>(T obj) where T : class;


    /// <summary>
    /// Use the <see cref="CanViewProperties{T}(T, int?)"/> for a specific property
    /// </summary>
    Task<bool> CanViewProperty<T>(T obj, string propertyName) where T : class;


    /// <summary>
    /// Check if the user can view this object
    /// <br>the properties of <paramref name="obj"/> could be used for specific validations</br>
    /// </summary>
    /// <remarks>
    /// returns false if <paramref name="obj"/> is null
    /// </remarks>
    Task<bool> CanCreate<T>(T obj) where T : class;

    Task<bool> CanCreate(Type type);


    /// <summary>
    /// Check if the user can edit this object. 
    /// <br>If <paramref name="oldObject"/> is not null, comparison with newObject can be used for specific validations</br>
    /// </summary>
    Task<bool> CanEdit<T>(T newObject, T oldObject = null) where T : class;




    /// <summary>
    /// Use the <see cref="CanEditProperties{T}(T, int?)(Type, int?)"/> for a specific property
    /// </summary>
    Task<bool> CanEditProperty<T>(T obj, string propertyName) where T : class;

    /// <summary>
    /// Check if the user can delete this object
    /// </summary>
    Task<bool> CanDelete<T>(T obj) where T : class;

}
