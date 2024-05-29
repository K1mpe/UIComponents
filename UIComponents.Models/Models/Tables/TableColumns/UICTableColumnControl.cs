
namespace UIComponents.Models.Models.Tables.TableColumns
{
    public class UICTableColumnControl : UICTableColumn, IUICInitializeAsync
    {
        public UICTableColumnControl()
        {
            Type = UIComponents.Defaults.Models.Table.TableColumns.UICTableColumnControl.Type;
            HeaderTemplate = null;
        }

        /// <summary>
        /// Show the Insert button in the header
        /// </summary>
        /// <remarks>
        /// If the value is null, it is automatically determined by the <see cref="UICTable.EnableInsert"/> and <see cref="UICTable.EnableInsert"/> or <see cref="UICTable.OnInsertButtonClick"></see>
        /// </remarks>
        public bool? Inserting 
        { 
            get
            {
                if(Options.TryGetValue("inserting", out var inserting))
                {
                    return (bool) inserting;
                }
                return null;
            }
            set
            {
                if (value == null)
                    Options.Remove("inserting");
                else
                    Options["inserting"] = value;
            }
        }

        /// <summary>
        /// Show the edit button in each row
        /// </summary>
        /// <remarks>
        /// If the value is null, it is automatically determined by the <see cref="UICTable.EnableUpdate"/> and <see cref="UICTable.OnUpdateItem"/>
        /// </remarks>
        public bool? EditButton
        {
            get
            {
                if (Options.TryGetValue("editButton", out var editButton))
                {
                    return (bool)editButton;
                }
                return null;
            }
            set
            {
                if (value == null)
                    Options.Remove("editButton");
                else
                    Options["editButton"] = value;
            }
        }

        /// <summary>
        /// Show the delete button in each row
        /// </summary>
        /// <remarks>
        /// If the value is null, it is automatically determined by the <see cref="UICTable.EnableDelete"/> and <see cref="UICTable.OnDeleteItem"/>
        /// </remarks>
        public bool? DeleteButton
        {
            get
            {
                if (Options.TryGetValue("deleteButton", out var deleteButton))
                {
                    return (bool)deleteButton;
                }
                return null;
            }
            set
            {
                if (value == null)
                    Options.Remove("deleteButton");
                else
                    Options["deleteButton"] = value;
            }
        }

        /// <summary>
        /// Used for adding buttons before the default buttons in the control cell
        /// </summary>
        /// <remarks>
        /// Available args: item
        /// </remarks>
        public IUICAction BeforeButtons { get; set; } = new UICCustom();

        /// <summary>
        /// Used for adding buttons after the default buttons in the control cell
        /// </summary>
        /// <remarks>
        /// Available args: item
        /// </remarks>
        public IUICAction AfterButtons { get; set; } = new UICCustom();

        public Task InitializeAsync()
        {
            if (ItemTemplate.HasValue())
                return Task.CompletedTask;

            if(!BeforeButtons.HasValue() && !AfterButtons.HasValue())
                return Task.CompletedTask;

            ItemTemplate = new UICTableColumnControlItemTemplate()
            {
                ControlColumn = this,
                BeforeButtons = BeforeButtons,
                AfterButtons = AfterButtons
            };
            return Task.CompletedTask;
        }

        public class UICTableColumnControlItemTemplate : IUICAction
        {
            public string RenderLocation => UIComponent.DefaultIdentifier(nameof(UICTableColumnControlItemTemplate));
            
            public UICTableColumnControlItemTemplate()
            {
                
            }

            public UICTableColumnControl ControlColumn { get; set; }

            public IUICAction BeforeButtons { get; set; }
            public IUICAction AfterButtons { get; set; }
        }
    }
}

namespace UIComponents.Defaults.Models.Table.TableColumns
{
    public static class UICTableColumnControl
    {
        public static string Type { get; set; } = "control";
    }
}

