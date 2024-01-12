namespace UIComponents.Abstractions.Interfaces;


public interface IUICHasChildren<TChildItem>
{
    List<TChildItem> Children { get; set; }

}