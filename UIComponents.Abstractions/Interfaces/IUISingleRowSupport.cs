namespace UIComponents.Abstractions.Interfaces;

public interface IUISingleRowSupport
{
    /// <summary>
    /// When this is a child from UICSingleRow, this function will be called so you can change the behavior of this component to be in 2 columns.
    /// </summary>
    /// <remarks>
    /// This transformation can be conditional, make sure the RenderInSingleRow response correctly</remarks>
    /// <returns></returns>
    public void TransformToSingleRow();

    /// <summary>
    /// Before placing this component in a single row, this function is called to check if the renderer actually renders single row.
    /// </summary>
    /// <returns></returns>
    public bool RendersInSingleRow();
}
