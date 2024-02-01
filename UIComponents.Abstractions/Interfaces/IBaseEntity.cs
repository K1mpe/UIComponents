namespace UIComponents.Abstractions.Interfaces;

/// <summary>
/// This interface is used for UIComponents to mark a entity as a database entity
/// </summary>
public interface IDbEntity
{
    public object Id { get; }
}


/// <summary>
/// This interface is used for UIComponents to mark a entity as a database entity
/// </summary>
public interface IDbEntity<T> : IDbEntity
{
    public new T Id { get; }

    object IDbEntity.Id { get => Id; }
}
