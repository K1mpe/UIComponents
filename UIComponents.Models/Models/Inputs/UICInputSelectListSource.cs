using UIComponents.Abstractions.Extensions;

namespace UIComponents.Models.Models.Inputs;

public class UICInputSelectListSource : UIComponent
{
    #region Fields
    #endregion

    #region Ctor
    public UICInputSelectListSource(UICInputSelectlist inputSelectlist, UICActionGetPost getItemsMethod) : this()
    {
        InputSelectList = inputSelectlist;
        GetSelectListItems = getItemsMethod;
    }

    public UICInputSelectListSource() : base()
    {
        RenderConditions.Add(() => InputSelectList.HasValue());

    }
    #endregion



    #region Properties
    public UICInputSelectlist InputSelectList { get; set; }

    /// <summary>
    /// The get or post request that gets the selectlistitems
    /// </summary>
    public UICActionGetPost GetSelectListItems { get; set; }

    /// <summary>
    /// Sort the selectlistitems after fetching
    /// </summary>
    public ItemSorting Sorting { get; set; } = ItemSorting.Text_Ascending;

    /// <summary>
    /// Optional: This method takes all values from the request, and converts them to selectListItems
    /// <br>If null, assumes values are already selectListItems.</br>
    /// <br>This can also be used for sorting or filtering the items</br>
    /// </summary>
    /// <remarks>
    /// function(data){
    /// <br>    let results = [];</br>
    /// <br>    data.forEach((item) =>{</br>
    /// <br>        results.push({Value: item.Id, Text: item.Name})</br>
    /// <br>    })</br>
    /// <br>    return results</br>
    /// </remarks>
    public IUIAction? MapToSelectListItems { get; set; } = null;

    /// <summary>
    /// When page is loaded, do not load the select list
    /// </summary>
    public bool SkipInitialLoad { get; set; }

    /// <summary>
    /// Reload the selectlistitems when the dropdown opens
    /// </summary>
    public bool ReloadOnOpen { get; set; }
    #endregion

    #region Methods
    /// <summary>
    /// Clientside: Trigger the selectlistitems to update
    /// </summary>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public IUIAction UpdateItems(){
        if (InputSelectList == null)
            return null;

        var id = InputSelectList.GetId();
        return new UICCustom($"$('#{id}').trigger('uic-reload');");
    }
    #endregion

    public enum ItemSorting
    {
        NoSorting,
        Text_Ascending,
        Text_Decending,
        Value_Ascending,
        Value_Decending,
    }
}
