
namespace UIComponents.Abstractions.Interfaces
{
    public interface IHasIcon
    {
        public IUICIcon Icon { get;}
    }
    public interface IHasIcon<T> : IHasIcon where T : class, IUICIcon
    {
        public new T Icon { get; }
        IUICIcon IHasIcon.Icon => Icon;


    }
}
