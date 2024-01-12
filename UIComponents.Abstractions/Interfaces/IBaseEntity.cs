namespace UIComponents.Abstractions.Interfaces;


public interface IDbEntity
{
    public object Id { get; }
}
public interface IDbEntity<T> : IDbEntity
{
    public new T Id { get; }
}
