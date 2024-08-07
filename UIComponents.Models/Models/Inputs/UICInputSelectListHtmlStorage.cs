using UIComponents.Abstractions.Extensions;
using UIComponents.Abstractions.Interfaces.Inputs;

namespace UIComponents.Models.Models.Inputs;

public class UICInputSelectListHtmlStorage : IUICAction
{

    public string RenderLocation => this.CreateDefaultIdentifier();

    #region Ctor
    public UICInputSelectListHtmlStorage(UICInputSelectList inputSelectlist, UICHtmlStorage htmlStorage)
    {
        InputSelectList = inputSelectlist;
        HtmlStorage = htmlStorage;
    }
    public UICInputSelectListHtmlStorage(UICInputMultiSelect inputMultiSelect, UICHtmlStorage htmlStorage)
    {
        InputSelectList = inputMultiSelect;
        HtmlStorage = htmlStorage;
    }

    public UICInputSelectListHtmlStorage()
    {
            
    }
    #endregion



    #region Properties
    [UICIgnoreGetChildrenFunction]
    public IUICInputSelectList InputSelectList { get; set; }

    /// <summary>
    /// The get or post request that gets the selectlistitems
    /// </summary>
    public UICHtmlStorage HtmlStorage { get; set; }

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
    public IUICAction? MapToSelectListItems { get; set; } = null;

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
    public IUICAction TriggerRefresh(){
        if (InputSelectList == null)
            return null;

        var id = InputSelectList.GetId();
        return new UICCustom($"$('#{id}').trigger('uic-reload');");
    }
    #endregion

    public enum ItemSorting
    {
        NoSorting = 0,
        Text_Ascending = 1,
        Text_Decending = 2,
        Value_Ascending = 3,
        Value_Decending = 4,
    }
}
