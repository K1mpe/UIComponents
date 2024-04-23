namespace UIComponents.Abstractions.Enums;

public enum UICTableColumnVisibility
{

    /// <summary>
    /// Show
    /// </summary>
    VisibleOnAll = 0,

    /// <summary>
    /// Hide
    /// </summary>
    HiddenOnAll = 1,

    /// <summary>
    /// Hide on screens smaller than the SM breakpoint
    /// </summary>
    HideSmallerThanSm = 2,

    /// <summary>
    /// Hide on screens smaller than the MD breakpoint
    /// </summary>
    HideSmallerThanMd = 3,

    /// <summary>
    /// Hide on screens smaller than the LG breakpoint
    /// </summary>
    HideSmallerThenLg = 4,

    /// <summary>
    /// Hide on screens smaller than the XL breakpoint
    /// </summary>
    HideSmallerThenXl = 5,

    /// <summary>
    /// Show on screens smaller than the sm breakpoint
    /// </summary>
    VisibleSmallerThenSm = 6,

    /// <summary>
    /// Show on screens smaller than the md breakpoint
    /// </summary>
    VisibleSmallerThenMd = 7,

    /// <summary>
    /// Show on screens smaller than the lg breakpoint
    /// </summary>
    VisibleSmallerThenLg = 8,

    /// <summary>
    /// Show on screens smaller than the xl breakpoint
    /// </summary>
    VisibleSmallerThenXl = 9,
}
