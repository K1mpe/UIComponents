namespace UIComponents.Defaults;

public static class RenderDefaults
{
    /// <summary>
    /// Any <see cref="IUIComponent"/> passes here before rendering, overwrite the renderlocation if you do not return null
    /// </summary>
    public static Func<IUIComponent, string?> OverwriteRenderLocation { get; set; } = null;
}
